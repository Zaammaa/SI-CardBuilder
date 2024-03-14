using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects
{
    [LandEffect]
    [SpiritEffect]
    internal class OrEffect : Effect
    {
        public override double BaseProbability { get { return .26; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { 
            get {
                int complexity = 4;
                complexity += choice1.Complexity;
                complexity += choice2.Complexity;
                return complexity;
            } 
        }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"-or-", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone { get { return false; } }
        public override bool TopLevelEffect()
        {
            return true;
        }

        public Effect choice1;
        public Effect choice2;

        //Writes what goes on the card
        public override string Print()
        {
            if (Context.target.SpiritTarget)
            {
                return $"Target Spirit chooses to either:\n{choice1.Print()}\n"+"{or}"+$"\n{choice2.Print()}";
            } else
            {
                return $"{choice1.Print()}\n"+ "{or}" + $"\n{choice2.Print()}";
            }
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.card.ContainsSameEffectType(this))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            do
            {
                choice1 = (Effect)Context.effectGenerator.ChooseEffect(UpdateContext());
                choice2 = (Effect)Context.effectGenerator.ChooseEffect(UpdateContext());
            } while (choice1 != null && choice2 != null && choice1.GetType() == choice2.GetType());
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            double higher = Math.Max(choice1.CalculatePowerLevel(), choice2.CalculatePowerLevel());
            double modifier = 1 / (Math.Abs(choice1.CalculatePowerLevel() - choice2.CalculatePowerLevel()) + 1);
            double normalizer = 1 / (higher + 1);
            return higher * (modifier / normalizer);
        }

        public override Effect? Strengthen()
        {
            Effect weakerEffect = choice1.CalculatePowerLevel() <= choice2.CalculatePowerLevel() ? choice1 : choice2;
            Effect? newEffect = weakerEffect.Strengthen();
            if (newEffect != null)
            {
                return newEffect;
            } else
            {
                return Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), weakerEffect.CalculatePowerLevel());
            }
        }

        public override Effect? Weaken()
        {
            Effect strongerEffect = choice1.CalculatePowerLevel() >= choice2.CalculatePowerLevel() ? choice1 : choice2;
            Effect? newEffect = strongerEffect.Weaken();
            if (newEffect != null)
            {
                return newEffect;
            }
            else
            {
                return Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), strongerEffect.CalculatePowerLevel());
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
            OrEffect effect = new OrEffect();
            effect.Context = Context;
            effect.choice1 = (Effect)choice1.Duplicate();
            effect.choice2 = (Effect)choice2.Duplicate();
            return effect;
        }
    }
}
