using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    internal class TargetLandConditionEffect : LandEffect
    {
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return .2; } set { } }
        public override int Complexity { get { return 6; } }

        private Condition? condition;
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(condition.ConditionText, RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            List<Condition> conditions = ReflectionManager.GetInstanciatedSubClasses<Condition>();
            List<Condition> validConditions = new List<Condition>();
            foreach (Condition condition in conditions)
            {
                condition.Initialize(card, settings);

                if (condition.IsValid(card, settings))
                {
                    validConditions.Add(condition);
                }
            }
            condition = (Condition)ChooseEffect(card, settings, validConditions);
            //Effects check cards for if they are valid or not, but not this condition. As a small hack, we can generate a new card with our condition as the target and use that
            Card fakeCard = (Card)card.Duplicate();

            List<LandEffect> effects = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
            List<LandEffect> validEffects = new List<LandEffect>();
            foreach (LandEffect landEffect in effects)
            {
                if (landEffect.GetType() == typeof(TargetLandConditionEffect) || landEffect.GetType() == typeof(LandElementalThresholdEffect))
                    continue;

                landEffect.InitializeEffect(card, settings);
                if (landEffect.IsValid(card, settings) && landEffect.IsValid(fakeCard, settings))
                {
                    validEffects.Add(landEffect);
                }
            }
            Effects.Add((LandEffect)ChooseEffect(card, settings, validEffects));
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
            dupEffect.condition = (Condition?)condition?.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            TargetLandConditionEffect strongerThis = (TargetLandConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            double roll = settings.rng.NextDouble() * 100;
            if (roll < 50 && strongerThis.condition.ChooseEasierCondition(card, settings))
            {
                return strongerThis;
            }
            else
            {
                Effect effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, settings.rng);

                Effect strongerEffect = effectToStrengthen.Strengthen(card, settings);
                if (strongerEffect != null)
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
        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            TargetLandConditionEffect weakerThis = (TargetLandConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            double roll = settings.rng.NextDouble() * 100;
            if (roll < 50 && weakerThis.condition.ChooseHarderCondition(card, settings))
            {
                return weakerThis;
            }
            else
            {
                Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, settings.rng);

                Effect? weakerEffect = effectToWeaken?.Weaken(card, settings);
                if (weakerEffect != null)
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
        }
    }
}
