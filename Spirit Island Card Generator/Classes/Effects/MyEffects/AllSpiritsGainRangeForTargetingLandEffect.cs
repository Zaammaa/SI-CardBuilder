using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.MyEffects
{
    [LandEffect]
    [CustomEffect(3)]
    internal class AllSpiritsGainRangeForTargetingLandEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air }; } }
        public override double effectStrength => 0.2;

        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex => throw new NotImplementedException();

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 0.8 },
            {3, 0.6 }
        };

        [AmountValue]
        public int rangeAmount = 1;

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change amounts", 30, IncreaseAmount, DecreaseAmount)
        ];

        public override IPowerLevel Duplicate()
        {
            AllSpiritsGainRangeForTargetingLandEffect effect = new AllSpiritsGainRangeForTargetingLandEffect();
            effect.rangeAmount = rangeAmount;
            effect.Context = Context?.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;
            else
                return true;
        }

        public override string Print()
        {
            return "All Spirits gain +{range-" + rangeAmount + "} for targeting this land";
        }

        public override bool Scan(string description)
        {
            return false;
        }

        protected override void InitializeEffect()
        {
            rangeAmount = Context.rng.Next(1, 4);
        }
    }
}
