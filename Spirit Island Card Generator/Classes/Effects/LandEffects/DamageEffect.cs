using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    public class DamageEffect : Effect
    {
        public override double BaseProbability { get { return .20; } }
        public override double AdjustedProbability { get { return .20; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"(\d{1,2}) Damage", RegexOptions.IgnoreCase);
            }
        }

        public int damageAmount = 1;

        //Writes what goes on the card
        public override string Print()
        {
            return damageAmount + " Damage.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.card.ContainsSameEffectType(this) || context.target.SpiritTarget || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            //TODO: Care about power level
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                damageAmount = 1;
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
            return (double)damageAmount * 0.9;
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
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                DamageEffect newEffect = (DamageEffect)Duplicate();
                newEffect.damageAmount += 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Effect? Weaken()
        {
            if (damageAmount <= 1)
            {
                return null;
            }
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                DamageEffect newEffect = (DamageEffect)Duplicate();
                newEffect.damageAmount -= 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                damageAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DamageEffect effect = new DamageEffect();
            effect.damageAmount = damageAmount;
            effect.Context = Context;
            return effect;
        }
    }
}
