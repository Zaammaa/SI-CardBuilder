using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class InOriginLandEffect : Effect, IParentEffect
    {
        //It's usually more efficient to do something in an another land. Plus it means extra range.
        protected static double MODIFIER_FOR_ORIGIN_ONLY = 1.1;
        protected static double MODIFIER_FOR_ORIGIN_OR_TARGET = 1.2;
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 5; } }

        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }

        //If true, the power can choose to do the effect in the target land or the origin land
        public bool originOrTargetLand = false;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Strengthen/Weaken", 80, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose another effect", 100, ChooseStrongerEffect, ChooseWeakerEffect),
            new DifficultyOption("Origin/Target or Origin", 100, InOriginOrTarget, InOriginLandOnly),
        };

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex("In origin land", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Range.range == 0)
                return false;

            return true;
        }

        public override int PrintOrder()
        {
            return 7;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            originOrTargetLand = Context.rng.NextDouble() >= 0.75 ? true : false;
            Effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel();
            }

            if (originOrTargetLand)
            {
                return powerLevel * MODIFIER_FOR_ORIGIN_OR_TARGET;
            } else
            {
                return powerLevel * MODIFIER_FOR_ORIGIN_ONLY;
            }
            
        }

        public override string Print()
        {
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

            if (originOrTargetLand)
            {
                return "In target or origin land, " + effectText;
            } else
            {
                return "In the origin land, " + effectText;
            }
           
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
            InOriginLandEffect dupEffect = new InOriginLandEffect();
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        protected Effect? StrengthenEffect()
        {
            InOriginLandEffect strongerThis = (InOriginLandEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = effectToStrengthen?.Strengthen();
            if (strongerEffect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? WeakenEffect()
        {
            InOriginLandEffect weakerThis = (InOriginLandEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

            Effect? weakerEffect = effectToWeaken?.Weaken();
            if (weakerEffect != null && effectToWeaken != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(weakerEffect);
                return weakerThis;
            }
            return null;
        }

        protected Effect? ChooseStrongerEffect()
        {
            InOriginLandEffect strongerThis = (InOriginLandEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), effectToStrengthen.CalculatePowerLevel());
            if (effectToStrengthen != null && strongerEffect != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? ChooseWeakerEffect()
        {
            InOriginLandEffect weakerThis = (InOriginLandEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

            Effect? weakerEffect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), effectToWeaken.CalculatePowerLevel());
            if (effectToWeaken != null && weakerEffect != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(weakerEffect);
                return weakerThis;
            }
            return null;
        }

        protected Effect? InOriginOrTarget()
        {
            if (!originOrTargetLand)
            {
                InOriginLandEffect newEffect = (InOriginLandEffect)Duplicate();
                newEffect.originOrTargetLand = true;
                return newEffect;
            }
            return null;
        }

        protected Effect? InOriginLandOnly()
        {
            if (originOrTargetLand)
            {
                InOriginLandEffect newEffect = (InOriginLandEffect)Duplicate();
                newEffect.originOrTargetLand = false;
                return newEffect;
            }
            return null;
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
    }
}
