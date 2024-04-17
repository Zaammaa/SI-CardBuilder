using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class TargetLandConditionEffect : Effect, IParentEffect
    {
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return .2; } set { } }
        public override int Complexity { get { return 6; } }

        private LandTypeCondition? condition;
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }

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
            if (context.chain.Count > 0)
                return false;
            return true;
        }

        public override int PrintOrder()
        {
            return 6;
        }

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change Effect Strength", 80, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose Different Condition", 20, ChooseEasierCondition, ChooseHarderCondition), 
        ];

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
           
            condition = (LandTypeCondition?)Context.effectGenerator.ChooseGeneratorOption<LandTypeCondition>(UpdateContext());
            condition.Initialize(UpdateContext());
            Target target = Context.card.Target.CreateShallowCopy();
            target.landConditions.Add(condition.landCondition);
            Context.target = target;
            
            //List<Effect> effects = ReflectionManager.GetInstanciatedSubClasses<Effect>();
            //List<Effect> validEffects = new List<Effect>();
            //foreach (Effect landEffect in effects)
            //{
            //    if (landEffect.GetType() == typeof(TargetLandConditionEffect) || landEffect.GetType() == typeof(LandElementalThresholdEffect))
            //        continue;

            //    if (landEffect.IsValidGeneratorOption(UpdateContext()))
            //    {
            //        landEffect.InitializeEffect(UpdateContext());
            //        validEffects.Add(landEffect);
            //    }
            //}
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
            TargetLandConditionEffect dupEffect = new TargetLandConditionEffect();
            dupEffect.condition = (LandTypeCondition?)condition?.Duplicate();
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
            TargetLandConditionEffect strongerThis = (TargetLandConditionEffect)Duplicate();

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
            TargetLandConditionEffect weakerThis = (TargetLandConditionEffect)Duplicate();
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
            TargetLandConditionEffect strongerThis = (TargetLandConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            if (CalculatePowerLevel() >= 0 && strongerThis.condition.ChooseEasierCondition(UpdateContext()))
            {
                return strongerThis;
            } else if (CalculatePowerLevel() < 0 && strongerThis.condition.ChooseHarderCondition(UpdateContext()))
            {
                return strongerThis;
            }
            return null;
        }

        protected Effect? ChooseHarderCondition()
        {
            TargetLandConditionEffect weakerThis = (TargetLandConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            if (CalculatePowerLevel() >= 0 && weakerThis.condition.ChooseHarderCondition(UpdateContext()))
            {
                return weakerThis;
            } else if (CalculatePowerLevel() < 0 && weakerThis.condition.ChooseEasierCondition(UpdateContext()))
            {
                return weakerThis;
            }
            return null;
        }


        #endregion
    }
}
