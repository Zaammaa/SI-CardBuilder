using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    [SpiritEffect]
    internal class CostConditionEffect : Effect, IParentEffect
    {
        public override double BaseProbability { get { return .15; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 4; } }

        private Condition costCondition;
        public List<Effect> Effects = new List<Effect>();
        public override bool HasMinMaxPowerLevel => true;
        public override double MinPowerLevel => (double)Context?.settings.TargetPowerLevel * 0.3;
        public override double MaxPowerLevel => (double)Context?.settings.TargetPowerLevel * 0.75;

        public override bool Standalone { get { return false; } }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(costCondition.ConditionText, RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change condition", 20, MakeConditionEasier, MakeConditionHarder),
            new DifficultyOption("Change effect", 60, MakeEffectStronger, MakeEffectWeaker),
            new DifficultyOption("Add/Remove effect", 20, NewEffect, RemoveEffect),
        ];

        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.chain.Count > 0)
                return false;
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.target.SpiritTarget)
            {
                costCondition = (Condition?)Context.effectGenerator.ChooseGeneratorOption<Condition>(new List<Attribute>() { new CostConditionAttribute() }, new List<Attribute>() { new LandConditionAttribute() }, UpdateContext());
            } else
            {
                costCondition = (Condition?)Context.effectGenerator.ChooseGeneratorOption<Condition>(new List<Attribute>() { new CostConditionAttribute() }, new List<Attribute>() { new SpiritConditionAttribute() }, UpdateContext());
            }
            
            costCondition.Initialize(UpdateContext());
            Context.conditions.Add(costCondition);
            Effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
            while (!AcceptablePowerLevel())
            {
                if (CalculatePowerLevel() < MinPowerLevel)
                {
                    if (!UnDuplicate((CostConditionEffect?)Strengthen()))
                    {
                        break;
                    }
                } else
                {
                    if (!UnDuplicate((CostConditionEffect?)Weaken()))
                    {
                        break;
                    }
                }
            }
        }
        //Takes another CostConditionEffect as input and sets this property to match
        //This deep clones the fields since I'm just doing it this way to get around strengthen/weaken returning a new object
        //I would set this = otherEffect if I could
        protected bool UnDuplicate(CostConditionEffect? otherEffect)
        {
            if (otherEffect == null)
                return false;

            this.Context = otherEffect.Context;
            this.costCondition = otherEffect.costCondition;
            this.Effects = otherEffect.Effects;
            return true;
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel() * costCondition.DifficultyMultiplier;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string conditionText = costCondition.ConditionText;
            string effectText = "";
            bool first = true;
            foreach (Effect effect in Effects)
            {
                if (!first)
                {
                    effectText += " and ";
                }
                else
                {
                    first = false;
                }
                effectText += effect.Print();
            }

            return conditionText + " " + effectText;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                //TODO: make the scan work
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            CostConditionEffect dupEffect = new CostConditionEffect();
            dupEffect.costCondition = (Condition?)costCondition?.Duplicate();
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        #region difficultyChangers

        protected Effect? MakeConditionEasier()
        {
            CostConditionEffect strongerThis = (CostConditionEffect)Duplicate();
            if (strongerThis.costCondition.ChooseEasierCondition(UpdateContext()))
            {
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }
            return null;
        }

        protected Effect? MakeConditionHarder()
        {
            CostConditionEffect weakerThis = (CostConditionEffect)Duplicate();
            if (weakerThis.costCondition.ChooseHarderCondition(UpdateContext()))
            {
                if (weakerThis.AcceptablePowerLevel())
                    return weakerThis;
            }
            return null;
        }

        public Effect? MakeEffectStronger()
        {
            CostConditionEffect strongerThis = (CostConditionEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = effectToStrengthen?.Strengthen();
            if (strongerEffect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }
            return null;
        }

        public Effect? MakeEffectWeaker()
        {
            CostConditionEffect weakerThis = (CostConditionEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

            Effect? weakerEffect = effectToWeaken?.Weaken();
            if (weakerEffect != null && effectToWeaken != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(weakerEffect);
                if (weakerThis.AcceptablePowerLevel())
                    return weakerThis;
            }

            return null;
        }

        public Effect? NewEffect()
        {
            CostConditionEffect strongerThis = (CostConditionEffect)Duplicate();
            Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (effect != null)
            {
                strongerThis.Effects.Add(effect);
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }

            return null;
        }

        public Effect? RemoveEffect()
        {
            CostConditionEffect weakerThis = (CostConditionEffect)Duplicate();
            Effect? effect = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);
            weakerThis.Effects.Remove(effect);
            if (weakerThis.AcceptablePowerLevel())
                return weakerThis;

            return null;
        }

        #endregion

        public bool AcceptablePowerLevel()
        {
            double powerLevel = CalculatePowerLevel();
            return powerLevel >= MinPowerLevel && powerLevel <= MaxPowerLevel;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return Effects;
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (this.Effects.Contains(effect))
            {
                Effects.Remove(effect);
                Effects.Add(newEffect);
            } else
            {
                throw new Exception("Replace called without the effect existing");
            }
        }
    }
}
