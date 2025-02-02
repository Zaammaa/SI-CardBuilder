using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.MyEffects;
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
    [CustomEffect(2)]
    internal class AdditionalDamageToAddBlightEffect : AmountEffect
    {
        public override string Name => "Extra Land Health";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Earth }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Plant }; } }
        public override double BaseProbability { get { return .005; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int additionalAmount = 1;

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        ];

        public override double effectStrength => 0.12;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1},
            {3, 1},
            {4, 0.9},
            {5, 0.9},
            {6, 0.8},
            {7, 0.7},
            {8, 0.6},
        };

        //Writes what goes on the card
        public override string Print()
        {
            return $"it takes an additional {additionalAmount} damage to add " + "{blight}" + $" to {Context.GetTargetString(TargetType)}";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;

            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                additionalAmount = Context.rng.Next(2, 8);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override bool Scan(string description)
        {
            return false;
        }

        public override Effect Duplicate()
        {
            AdditionalDamageToAddBlightEffect effect = new AdditionalDamageToAddBlightEffect();
            effect.additionalAmount = additionalAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
