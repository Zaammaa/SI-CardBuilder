using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class InOneOfSpiritLandsEffect : Effect, IParentEffect
    {
        public override string Name => "In One of Spirit's Lands";
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity
        {
            get
            {
                int complexity = 2;
                complexity += GetChildren().Sum((effect) => effect.Complexity);
                return complexity;
            }
        }

        public override bool MentionsTarget => true;

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"In one of Target Spirit's lands", RegexOptions.IgnoreCase);
            }
        }

        public EffectGeneratorSettings effectSettings
        {
            get
            {
                EffectGeneratorSettings effectSettings = EffectGeneratorSettings.GetStandardEffectSettings(CreateNewContext());
                effectSettings.bannedAttributes.Add(new UnspecificLandAttribute());
                return effectSettings;
            }
        }

        public Effect effect;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
{
            new DifficultyOption("Change effect strength", 80, StrengthenEffect, WeakenEffect),
};

        //Writes what goes on the card
        public override string Print()
        {
            if (Context.targetMentioned)
            {
                return $"In one of their lands, " + effect.Print();
            } else
            {
                return $"In one of {Context.GetTargetString(TargetType)}'s lands, " + effect.Print();
            }
            
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.HasEffectAbove(typeof(SpiritWithPresenceMayEffect)))
                return false;
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            Context newContext = CreateNewContext();
            effect = (Effect)Context.effectGenerator.ChooseEffect(effectSettings);
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //slight increase in power for the range boost?
            return effect.CalculatePowerLevel() * 1.1;
        }

        protected Context CreateNewContext()
        {
            Target newTarget = new Target();
            newTarget.targetType = Target.TargetType.Land;
            Context newContext = UpdateContext();
            newContext.target = newTarget;
            return newContext;
        }
        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        protected Effect? StrengthenEffect()
        {
            InOneOfSpiritLandsEffect strongerThis = (InOneOfSpiritLandsEffect)Duplicate();
            Effect? newEffect = (Effect?)strongerThis.effect.Strengthen();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseStrongerEffect(effectSettings, strongerThis.effect.CalculatePowerLevel());
            }
            if (newEffect != null)
            {
                strongerThis.effect = newEffect;
                return strongerThis;
            } else
            {
                return null;
            }

        }

        protected Effect? WeakenEffect()
        {
            InOneOfSpiritLandsEffect weakerThis = (InOneOfSpiritLandsEffect)Duplicate();
            Effect? newEffect = (Effect?)weakerThis.effect.Weaken();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseWeakerEffect(effectSettings, weakerThis.effect.CalculatePowerLevel());
            }
            if (newEffect != null)
            {
                weakerThis.effect = newEffect;
                return weakerThis;
            } else
            {
                return null;
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            InOneOfSpiritLandsEffect newEffect = new InOneOfSpiritLandsEffect();
            newEffect.Context = Context.Duplicate();
            newEffect.effect = (Effect)effect.Duplicate();
            return newEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return new List<Effect>() { effect };
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (this.effect.Equals(effect))
            {
                this.effect = newEffect;
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }
    }
}
