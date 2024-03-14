using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
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
    internal class SpiritWithPresenceMayEffect : Effect
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
            if (context.card.ContainsSameEffectType(this) || context.card.Target.SpiritTarget)
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

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public override Effect? Strengthen()
        {
            Effect newEffect = effect.Strengthen();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseStrongerEffect(CreateNewContext(), effect.CalculatePowerLevel());
            }
            return newEffect;
        }

        public override Effect? Weaken()
        {
            Effect newEffect = effect.Weaken();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseWeakerEffect(CreateNewContext(), effect.CalculatePowerLevel());
            }
            return newEffect;
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
            SpiritWithPresenceMayEffect effect = new SpiritWithPresenceMayEffect();
            effect.effect = effect.Duplicate();
            effect.Context = Context;
            return effect;
        }
    }
}
