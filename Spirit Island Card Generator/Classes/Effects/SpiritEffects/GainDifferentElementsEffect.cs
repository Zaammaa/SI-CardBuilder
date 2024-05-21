using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class GainDifferentElementsEffect : AmountEffect
    {
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Moon }; } }
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int differentElementAmounts = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.25;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 0.9 },
            {3, 0.8 },
            {4, 0.6 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            if (differentElementAmounts == 1)
            {
                return $"{Context.GetTargetString(TargetType)} gains {differentElementAmounts} " + "{any}";
            }
            return $"{Context.GetTargetString(TargetType)} gains {differentElementAmounts} different Elements of their choice";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                differentElementAmounts = Context.rng.Next(1, 3);
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
            GainDifferentElementsEffect effect = new GainDifferentElementsEffect();
            effect.differentElementAmounts = differentElementAmounts;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
