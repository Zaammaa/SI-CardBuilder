using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    public class DefendEffect : LandEffect
    {
        public override double BaseProbability { get { return .16; } }
        public override double AdjustedProbability { get { return .16; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Defend (\d{1,2})", RegexOptions.IgnoreCase);
            }
        }

        public int defendAmount = 1;

        //Writes what goes on the card
        public override string Print()
        {
            return $"Defend {defendAmount}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            if (card.ContainsEffect(this.GetType()) || card.Target.SpiritTarget || !card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            //TODO: Care about power level
            if (card.CardType == Card.CardTypes.Minor)
            {
                defendAmount = settings.rng.Next(1,11);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return (double)defendAmount * 0.2;
        }

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public override Effect? Strengthen(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                DefendEffect newEffect = (DefendEffect)Duplicate();
                newEffect.defendAmount += 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            if (defendAmount > 1)
            {
                DefendEffect effect = (DefendEffect)Duplicate();
                effect.defendAmount -= 1;
                return effect;
            } else
            {
                return null;
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                defendAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DefendEffect effect = new DefendEffect();
            effect.defendAmount = defendAmount;
            return effect;
        }
    }
}
