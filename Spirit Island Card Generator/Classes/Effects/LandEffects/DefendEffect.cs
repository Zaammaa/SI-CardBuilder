using OpenQA.Selenium.DevTools.V120.Input;
using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    public class DefendEffect : AmountEffect , ITrackedStat
    {
        public static string TrackedName => "Defend";
        public static int TargetAmount => 17;
        public bool ExactTarget => false;
        public ITrackedStat.Pool pool => ITrackedStat.Pool.None;
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Earth }; } }
        public override double BaseProbability { get { return .30; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Defend (\d{1,2})", RegexOptions.IgnoreCase);
            }
        }

        [AmountValue]
        public int defendAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.15;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 0.4 },
            {2, 0.6},
            {3, 1},
            {4, 0.9},
            {5, 0.8},
            {6, 0.7},
            {7, 0.6},
            {8, 0.6},
            {9, 0.5},
            {10, 0.4},
        };

        //Writes what goes on the card
        public override string Print()
        {
            return $"Defend {defendAmount}";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.ContainsSameEffectType(this) || context.target.SpiritTarget || !context.card.Fast || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoBuildings))
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
                defendAmount = 4;
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
            return (double)defendAmount * 0.18;
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
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
