using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class GainPowerCardEffect : Effect
    {
        public override double BaseProbability { get { return .07; } }
        public override double AdjustedProbability { get { return .07; } set { } }
        public override int Complexity { get { return 5; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit gains a (Minor|Major)? Power Card", RegexOptions.IgnoreCase);
            }
        }

        public PowerCardType cardType;

        //Writes what goes on the card
        public override string Print()
        {
            if (cardType == PowerCardType.Unspecified)
            {
                return "Target Spirit gains a Power Card.";
            }
            else
            {
                return $"Target Spirit gains a {cardType} Power Card.";
            }
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.card.Target.SpiritTarget || context.card.ContainsSameEffectType(this))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                List<PowerCardType> validTypes = new List<PowerCardType>() { PowerCardType.Minor, PowerCardType.Major, PowerCardType.Unspecified };
                cardType = Utils.ChooseRandomListElement(validTypes, Context.rng);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            switch (cardType)
            {
                case PowerCardType.Unspecified:
                    return 2.5;
                case PowerCardType.Major:
                    return 2;
                case PowerCardType.Minor:
                    return 1.2;
                default:
                    throw new NotImplementedException("Invalid card type");
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
            if (cardType == PowerCardType.Minor)
            {
                GainPowerCardEffect newEffect = (GainPowerCardEffect)Duplicate();
                newEffect.cardType = PowerCardType.Major;
                return newEffect;
            }
            else if (cardType == PowerCardType.Major)
            {
                GainPowerCardEffect newEffect = (GainPowerCardEffect)Duplicate();
                newEffect.cardType = PowerCardType.Unspecified;
                return newEffect;
            }
            else
            {
                return null;
            }
        }

        public override Effect? Weaken()
        {
            if (cardType == PowerCardType.Major)
            {
                GainPowerCardEffect newEffect = (GainPowerCardEffect)Duplicate();
                newEffect.cardType = PowerCardType.Minor;
                return newEffect;
            }
            else if (cardType == PowerCardType.Unspecified)
            {
                GainPowerCardEffect newEffect = (GainPowerCardEffect)Duplicate();
                newEffect.cardType = PowerCardType.Major;
                return newEffect;
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
                if (match.Groups[1].Value.Equals(""))
                {
                    cardType = PowerCardType.Unspecified;
                }
                else
                {
                    cardType = (PowerCardType)Enum.Parse(typeof(PowerCardType), match.Groups[1].Value, true);
                }
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            GainPowerCardEffect effect = new GainPowerCardEffect();
            effect.cardType = cardType;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
