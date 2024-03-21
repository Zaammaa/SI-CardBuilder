using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class SpiritWithPresenceMayEffect : Effect, IParentEffect
    {
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"A Spirit with presence in Target Land may", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change Effect Strength", 80, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose Different Effect", 20, ChooseBetterEffect, ChooseWorseEffect),
        };

        public Effect effect;

        //Writes what goes on the card
        public override string Print()
        {
            return "A spirit with {presence} in target land " + effect.Print();
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            //I think this is a rare case where we do want to look at the card's target instead of the contextual target.
            if (context.card.Target.SpiritTarget)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            effect = (Effect)Context.effectGenerator.ChooseEffect(CreateNewContext());
        }

        protected Context CreateNewContext()
        {
            Context newContext = UpdateContext();
            Target newTarget = new Target();
            newTarget.SpiritTarget = true;
            newContext.target = newTarget;
            return newContext;
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //The effect is slightly weaker due to having to get the targeting right.
            return effect.CalculatePowerLevel() * 0.8;
        }

        #region DifficultyOptions

        protected Effect? StrengthenEffect()
        {
            SpiritWithPresenceMayEffect strongerThis = (SpiritWithPresenceMayEffect)Duplicate();
            Effect? newEffect = strongerThis.effect.Strengthen();
            if (newEffect != null)
            {
                strongerThis.effect = newEffect;
                return strongerThis;
            }
            return null;
        }

        protected Effect? ChooseBetterEffect()
        {
            SpiritWithPresenceMayEffect strongerThis = (SpiritWithPresenceMayEffect)Duplicate();
            Effect? newEffect = (Effect?)Context?.effectGenerator.ChooseStrongerEffect(CreateNewContext(), strongerThis.effect.CalculatePowerLevel());
            if (newEffect != null)
            {
                strongerThis.effect = newEffect;
                return strongerThis;
            }
            return null;
        }

        protected Effect? WeakenEffect()
        {
            SpiritWithPresenceMayEffect weakerThis = (SpiritWithPresenceMayEffect)Duplicate();
            Effect? newEffect = weakerThis.effect.Weaken();
            if (newEffect != null)
            {
                weakerThis.effect = newEffect;
                return weakerThis;
            }
            return null;
        }
        protected Effect? ChooseWorseEffect()
        {
            SpiritWithPresenceMayEffect weakerThis = (SpiritWithPresenceMayEffect)Duplicate();
            Effect? newEffect = (Effect?)Context?.effectGenerator.ChooseWeakerEffect(CreateNewContext(), weakerThis.effect.CalculatePowerLevel());
            if (newEffect != null)
            {
                weakerThis.effect = newEffect;
                return weakerThis;
            }
            return null;
        }


        #endregion

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
            SpiritWithPresenceMayEffect newEffect = new SpiritWithPresenceMayEffect();
            newEffect.effect = (Effect)effect.Duplicate();
            newEffect.Context = Context.Duplicate();
            return newEffect;
        }

        public List<Effect> GetChildren()
        {
            return new List<Effect> { effect };
        }
    }
}
