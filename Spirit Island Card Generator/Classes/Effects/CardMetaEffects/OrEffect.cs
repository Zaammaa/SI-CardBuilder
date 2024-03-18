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

namespace Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects
{
    [LandEffect]
    [SpiritEffect]
    internal class OrEffect : Effect, IParentEffect
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
        public override int PrintOrder()
        {
            return 8;
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
                return $"\n{choice1.Print()}\n"+ "{or}" + $"\n{choice2.Print()}";
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
            //Effect weakerEffect = choice1.CalculatePowerLevel() <= choice2.CalculatePowerLevel() ? choice1 : choice2;
            //Effect? newEffect = weakerEffect.Strengthen();
            //if (newEffect != null)
            //{
            //    return newEffect;
            //} else
            //{
            //    return Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), weakerEffect.CalculatePowerLevel());
            //}

            OrEffect strongerThis = (OrEffect)Duplicate();
            Effect weakerEffect = strongerThis.choice1.CalculatePowerLevel() <= strongerThis.choice2.CalculatePowerLevel() ? strongerThis.choice1 : strongerThis.choice2;
            Effect? newEffect = weakerEffect.Strengthen();
            if (newEffect != null)
            {
                if (weakerEffect.Equals(strongerThis.choice1))
                    strongerThis.choice1 = newEffect;
                else
                    strongerThis.choice2 = newEffect;
                return strongerThis;
            }
            else
            {
                newEffect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), weakerEffect.CalculatePowerLevel());
                if (newEffect != null)
                {
                    if (weakerEffect.Equals(strongerThis.choice1))
                        strongerThis.choice1 = newEffect;
                    else
                        strongerThis.choice2 = newEffect;
                    return strongerThis;
                }
            }
            return null;
        }

        public override Effect? Weaken()
        {
            OrEffect weakerThis = (OrEffect)Duplicate();
            Effect strongerEffect = weakerThis.choice1.CalculatePowerLevel() >= weakerThis.choice2.CalculatePowerLevel() ? weakerThis.choice1 : weakerThis.choice2;
            Effect? newEffect = strongerEffect.Weaken();
            if (newEffect != null)
            {
                if (strongerEffect.Equals(weakerThis.choice1))
                    weakerThis.choice1 = newEffect;
                else
                    weakerThis.choice2 = newEffect;
                return weakerThis;
            }
            else
            {
                newEffect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), strongerEffect.CalculatePowerLevel());
                if (newEffect != null)
                {
                    if (strongerEffect.Equals(weakerThis.choice1))
                        weakerThis.choice1 = newEffect;
                    else
                        weakerThis.choice2 = newEffect;
                    return weakerThis;
                }
            }
            return null;
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
            effect.Context = Context.Duplicate();
            effect.choice1 = (Effect)choice1.Duplicate();
            effect.choice2 = (Effect)choice2.Duplicate();
            return effect;
        }

        public List<Effect> GetChildren()
        {
            return new List<Effect>() { choice1, choice2 };
        }
    }
}
