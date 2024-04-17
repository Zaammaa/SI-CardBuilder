using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
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
    internal class RemoveBlightEffect : Effect, ITrackedStat
    {
        public override double BaseProbability { get { return .12; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Remove (\d{1,2}) blight", RegexOptions.IgnoreCase);
            }
        }

        public static string TrackedName => "Blight Removal";

        public static int TargetAmount => 12;

        public bool ExactTarget => false;

        public ITrackedStat.Pool pool => ITrackedStat.Pool.None;

        public int removeAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        };

        //Writes what goes on the card
        public override string Print()
        {
            return "Remove " + removeAmount + " {blight}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            //TODO: At some point This should avoid cards that have a blight add effect too
            if (context.target.SpiritTarget || context.target.landConditions.Contains(LandConditions.Noblight))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            removeAmount = 1;
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return (double)removeAmount * 1.5;
        }

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        protected Effect? IncreaseAmount()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                RemoveBlightEffect newEffect = (RemoveBlightEffect)Duplicate();
                newEffect.removeAmount += 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected Effect? DecreaseAmount()
        {
            if (removeAmount > 1)
            {
                RemoveBlightEffect newEffect = (RemoveBlightEffect)Duplicate();
                newEffect.removeAmount -= 1;
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
                removeAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            RemoveBlightEffect effect = new RemoveBlightEffect();
            effect.removeAmount = removeAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
