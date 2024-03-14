using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class GainEnergyEffect : Effect
    {
        public override double BaseProbability { get { return .25; } }
        public override double AdjustedProbability { get { return .25; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit gains (\d{1,2}) Energy", RegexOptions.IgnoreCase);
            }
        }

        public int energyAmount = 1;

        //Writes what goes on the card
        public override string Print()
        {
            return $"Target Spirit gains {energyAmount} Energy.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.target.SpiritTarget)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                energyAmount = Context.rng.Next(1, 3);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return (double)energyAmount / 2;
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
                GainEnergyEffect newEffect = (GainEnergyEffect)Duplicate();
                newEffect.energyAmount += 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Effect? Weaken()
        {
            GainEnergyEffect newEffect = (GainEnergyEffect)Duplicate();
            newEffect.energyAmount -= 1;
            if (newEffect.energyAmount <= 0)
                return null;
            else
                return newEffect;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                energyAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            GainEnergyEffect effect = new GainEnergyEffect();
            effect.energyAmount = energyAmount;
            effect.Context = Context;
            return effect;
        }
    }
}
