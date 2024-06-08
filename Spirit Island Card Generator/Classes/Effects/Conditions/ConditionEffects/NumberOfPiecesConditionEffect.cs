using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.LandStateConditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.ConditionEffects
{
    [LandEffect]
    internal class NumberOfPiecesConditionEffect : Effect, IParentEffect
    {
        public override string Name => condition.ConditionText;
        public override double BaseProbability { get { return .1; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 6; } }

        private NumberOfPiecesCondition? condition;
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }
        public override bool TopLevelEffect() { return true; }

        public override bool HasMinMaxPowerLevel => true;
        public override double MinPowerLevel => (double)Context?.settings.TargetPowerLevel / 5;
        public override double MaxPowerLevel => (double)Context?.settings.TargetPowerLevel / 2;

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(condition.ConditionText, RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override int PrintOrder()
        {
            return 7;
        }

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change Effect Strength", 80, StrengthenEffect, WeakenEffect),
            //new DifficultyOption("Choose Different Condition", 20, ChooseEasierCondition, ChooseHarderCondition),
        ];

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

            condition = (NumberOfPiecesCondition?)Context.effectGenerator.ChooseGeneratorOption<NumberOfPiecesCondition>(UpdateContext());
            condition.Initialize(UpdateContext());
            Target target = Context.card.Target.CreateShallowCopy();
            target.landConditions.AddRange(condition.Implications);
            Context.target = target;

            Effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel() * condition.DifficultyMultiplier;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string conditionText = condition.ConditionText;
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
            NumberOfPiecesConditionEffect dupEffect = new NumberOfPiecesConditionEffect();
            dupEffect.condition = (NumberOfPiecesCondition?)condition?.Duplicate();
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return Effects;
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (Effects.Remove(effect))
            {
                Effects.Add(newEffect);
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }

        #region DifficultyOptions

        protected Effect? StrengthenEffect()
        {
            NumberOfPiecesConditionEffect strongerThis = (NumberOfPiecesConditionEffect)Duplicate();

            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = effectToStrengthen?.Strengthen();
            if (strongerEffect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                return strongerThis;
            }
            else
            {
                return null;
            }
        }

        protected Effect WeakenEffect()
        {
            NumberOfPiecesConditionEffect weakerThis = (NumberOfPiecesConditionEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

            Effect? weakerEffect = effectToWeaken?.Weaken();
            if (weakerEffect != null && effectToWeaken != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(weakerEffect);
                return weakerThis;
            }
            else
            {
                return null;
            }
        }

        protected Effect? ChooseEasierCondition()
        {
            NumberOfPiecesConditionEffect strongerThis = (NumberOfPiecesConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            if (CalculatePowerLevel() >= 0 && strongerThis.condition.ChooseEasierCondition(UpdateContext()))
            {
                return strongerThis;
            }
            else if (CalculatePowerLevel() < 0 && strongerThis.condition.ChooseHarderCondition(UpdateContext()))
            {
                return strongerThis;
            }
            return null;
        }

        protected Effect? ChooseHarderCondition()
        {
            NumberOfPiecesConditionEffect weakerThis = (NumberOfPiecesConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            if (CalculatePowerLevel() >= 0 && weakerThis.condition.ChooseHarderCondition(UpdateContext()))
            {
                return weakerThis;
            }
            else if (CalculatePowerLevel() < 0 && weakerThis.condition.ChooseEasierCondition(UpdateContext()))
            {
                return weakerThis;
            }
            return null;
        }


        #endregion
    }
}
