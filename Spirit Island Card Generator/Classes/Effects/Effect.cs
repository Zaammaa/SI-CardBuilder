using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.Fixers;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    public abstract class Effect : IGeneratorOption, IPowerLevel
    {
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }

        public abstract Regex descriptionRegex { get; }

        public virtual List<ElementSet.Element> StronglyAssociatedElements { get { return new List<ElementSet.Element>(); } }
        public virtual List<ElementSet.Element> WeaklyAssociatedElements { get { return new List<ElementSet.Element>(); } }

        //Effects that cannot be on the same card as this effect
        public virtual List<Type> IncompatibleEffects {  get { return new List<Type>(); } }
        //Effects that this effect depends on. If those effects are removed, this effect is no longer valid
        public List<Effect> LinkedEffects = new List<Effect>();

        protected Context? Context;
        //Whether the effect can be skipped. If it can, it should say "may"
        public bool optional = false;

        //Writes what goes on the card
        public abstract string Print();
        //Initializes effect from a card description.
        public abstract bool Scan(string description);
        //Checks if this should be an option for the card generator
        public abstract bool IsValidGeneratorOption(Context context);
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected abstract void InitializeEffect();
        //Estimates the effects own power level
        public abstract double CalculatePowerLevel();

        public abstract IPowerLevel Duplicate();
        //If false, the card must have another effect.

        public IValidFixer? IsValid()
        {


            //Standard checks
            if (!WithinPowerLevel(this))
            {
                return new PowerLevelFixer(this);
            } else if (Context.card.GetAllEffects().Any((effect) => { return IncompatibleEffects.Contains(effect.GetType()); }))
            {
                return new IncompatableEffectsFixer(this);
            } else if (LinkedEffects.Any((effect) => { return !Context.card.GetAllEffects().Contains(effect); }))
            {
                return new LinkedEffectFixer(this);
            }

            IValidFixer? fixer = CustomIsValid();
            if (fixer != null)
                return fixer;

            return null;
        }

        protected virtual IValidFixer? CustomIsValid()
        {
            return null;
        }

        public virtual void Fix()
        {
            //To be overriden as needed
        }

        public virtual bool HasMinMaxPowerLevel { get { return false; } }

        public virtual double MinPowerLevel { get { return 0; }  }
        public virtual double MaxPowerLevel { get { return 0; }  }

        public virtual bool Standalone { get { return true; } }

        public virtual bool SelfReferencingPowerLevel {  get { return false; } }

        public virtual bool TopLevelEffect()
        {
            return false;
        }
        //Most effects can remain at 5 and just let fate decide the order
        //Certain effects should happen earlier or later though
        //Fear should be first
        //Elemental thresholds last (Though I think these will be in an entirely seperate spot once we create the file for the online builder)
        //Condions and 'or' should be later?
        public virtual int PrintOrder()
        {
            return 5;
        }

        public int StackPrintOrder()
        {
            int highestPrintOrder = 1;
            foreach(Effect effect in GetAllEffectsInChain())
            {
                if (effect.PrintOrder() > highestPrintOrder)
                    highestPrintOrder = effect.PrintOrder();
            }
            return highestPrintOrder;
        }
        public List<Effect> GetAllEffectsInChain()
        {
            List<Effect> effects = [this];
            if (this.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
            {
                IParentEffect parentEffect = (IParentEffect)this;
                foreach(Effect childEffects in parentEffect.GetChildren())
                {
                    effects.AddRange(childEffects.GetAllEffectsInChain());
                }
            }
            return effects;
        }


        public virtual bool MentionsTarget
        {
            get { return false; }
        }

        public int Level
        {
            get { return Context.chain.Count; }
        }

        public IParentEffect? Parent
        {
            get
            {
                if (Level == 0)
                    return null;
                
                return Context?.chain.Last();
            }
        }

        protected struct DifficultyOption
        {
            public string name;
            public int weight;
            public Func<Effect?> Upgrade;
            public Func<Effect?> Downgrade;

            public DifficultyOption(string n, int w, Func<Effect?> upgrade, Func<Effect?> downgrade)
            {
                name = n;
                weight = w;
                Upgrade = upgrade;
                Downgrade = downgrade;
            }

            public override string ToString()
            {
                return name;
            }
        }
        protected abstract DifficultyOption[] difficultyOptions { get; }

        //Set the context here so I don't have to worry about forgetting to do it in a million subclasses
        public void InitializeEffect(Context context)
        {
            Context = context;
            InitializeEffect();
        }

        

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public Effect? Strengthen()
        {
            List<DifficultyOption> untriedOptions = new List<DifficultyOption>(difficultyOptions);

            while(untriedOptions.Count > 0)
            {
                Dictionary<DifficultyOption, int> weightedOptions = new Dictionary<DifficultyOption, int>();
                foreach(DifficultyOption opt in untriedOptions)
                {
                    weightedOptions.Add(opt, opt.weight);
                }
                DifficultyOption option = Utils.ChooseWeightedOption<DifficultyOption>(weightedOptions, Context.rng);

                Effect? chosenEffect = option.Upgrade();
                if (chosenEffect != null && WithinPowerLevel(chosenEffect))
                {
                    if (chosenEffect.CalculatePowerLevel() <= CalculatePowerLevel())
                    {
                        Log.Warning("Strengthen failed to increase power level");
                        //throw new Exception("Strengthen failed to increase power level");
                    }
                    return chosenEffect;
                } else
                {
                    untriedOptions.Remove(option);
                }
            }

            return null;
        }

        public Effect? Weaken()
        {
            List<DifficultyOption> untriedOptions = new List<DifficultyOption>(difficultyOptions);

            while (untriedOptions.Count > 0)
            {
                Dictionary<DifficultyOption, int> weightedOptions = new Dictionary<DifficultyOption, int>();
                foreach (DifficultyOption opt in untriedOptions)
                {
                    weightedOptions.Add(opt, opt.weight);
                }
                DifficultyOption option = Utils.ChooseWeightedOption<DifficultyOption>(weightedOptions, Context.rng);

                Effect? chosenEffect = option.Downgrade();
                if (chosenEffect != null && WithinPowerLevel(chosenEffect))
                {
                    if (chosenEffect.CalculatePowerLevel() >= CalculatePowerLevel())
                    {
                        Log.Warning("Weaken failed to reduce power level");
                        //throw new Exception("Weaken failed to reduce power level");
                    }
                    return chosenEffect;
                }
                else
                {
                    untriedOptions.Remove(option);
                }
            }

            return null;
        }

        protected bool WithinPowerLevel(Effect chosenEffect)
        {
            if (!chosenEffect.HasMinMaxPowerLevel || chosenEffect.Context?.chain.Count > 0)
            {
                return true;
            } else
            {
                double powerLevel = chosenEffect.CalculatePowerLevel();
                return powerLevel >= chosenEffect.MinPowerLevel && powerLevel <= chosenEffect.MaxPowerLevel;
            }
        }

        protected Context UpdateContext()
        {
            Context newContext = Context.Duplicate();
            if (this.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
                newContext.chain.Add((IParentEffect)this);

            if (this.MentionsTarget)
                newContext.targetMentioned = true;

            return newContext;
        }

        public Context? GetSameContext()
        {
            return Context?.Duplicate();
        }
    }
}
