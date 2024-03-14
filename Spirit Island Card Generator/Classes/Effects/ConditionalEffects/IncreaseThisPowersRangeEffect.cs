using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffect]
    internal class IncreaseThisPowersRangeEffect : Effect
    {
        public override double BaseProbability { get { return .1; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Increase this Power's Range by (\d)", RegexOptions.IgnoreCase);
            }
        }
        protected int rangeIncrease = 1;

        public override bool Standalone { get { return false; } }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Increase this Power's Range by {rangeIncrease}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.target.SpiritTarget || context.card.Range.range >= 2)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            if (Context.card.Range.range == 0)
            {
                return 0.2 * rangeIncrease;
            } else if (Context.card.Range.range == 1) { 
                return 0.1 * rangeIncrease;
            } else
            {
                return 0.05 * rangeIncrease;
            }
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
            if (rangeIncrease <= 1)
            {
                IncreaseThisPowersRangeEffect effect = (IncreaseThisPowersRangeEffect)Duplicate();
                effect.rangeIncrease += 1;
                return effect;
            } else
            {
                return null;
            }
        }

        public override Effect? Weaken()
        {
            if (rangeIncrease > 1)
            {
                IncreaseThisPowersRangeEffect effect = (IncreaseThisPowersRangeEffect)Duplicate();
                effect.rangeIncrease -= 1;
                return effect;
            }
            else
            {
                return null;
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                rangeIncrease = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            IncreaseThisPowersRangeEffect effect = new IncreaseThisPowersRangeEffect();
            effect.rangeIncrease = rangeIncrease;
            effect.Context = Context;
            return effect;
        }
    }
}
