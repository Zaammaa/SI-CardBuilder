using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class RangeIncreaseEffect : AmountEffect
    {
        public override double BaseProbability { get { return .1; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit (?:gets|gains) \+(\d{1,2}) range", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int rangeAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.5;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, .7 },
            {3, .5 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            return "Target Spirit gets +{range-" + rangeAmount + "} Range with all their Powers.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.target.SpiritTarget || !context.card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            Card card = Context.card;
            if (card.CardType == Card.CardTypes.Minor)
            {
                rangeAmount = Context.rng.Next(1, 4);
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
                rangeAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            RangeIncreaseEffect effect = new RangeIncreaseEffect();
            effect.rangeAmount = rangeAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
