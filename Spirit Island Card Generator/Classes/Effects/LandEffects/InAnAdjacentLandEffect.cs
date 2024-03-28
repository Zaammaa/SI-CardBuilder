using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class InAnAdjacentLandEffect : Effect, IParentEffect
    {
        //It's usually more efficient to do something in an another land. Plus it means extra range.
        protected static double ADJACENCY_MODIFIER = 1.3;
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 5; } }

        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Strengthen/Weaken", 80, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose another effect", 100, ChooseStrongerEffect, ChooseWeakerEffect),
        };

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex("In an adjacent land", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            return true;
        }

        public override int PrintOrder()
        {
            return 7;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            Effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel() * ADJACENCY_MODIFIER;
            }
            return powerLevel;
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

            return "In an adjacent land, " + effectText;
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
            InAnAdjacentLandEffect dupEffect = new InAnAdjacentLandEffect();
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        protected Effect? StrengthenEffect()
        {
            InAnAdjacentLandEffect strongerThis = (InAnAdjacentLandEffect)Duplicate();
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
            InAnAdjacentLandEffect weakerThis = (InAnAdjacentLandEffect)Duplicate();
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
            InAnAdjacentLandEffect strongerThis = (InAnAdjacentLandEffect)Duplicate();
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
            InAnAdjacentLandEffect weakerThis = (InAnAdjacentLandEffect)Duplicate();
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

        public IEnumerable<Effect> GetChildren()
        {
            return Effects;
        }
    }
}
