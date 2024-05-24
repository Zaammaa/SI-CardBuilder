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
    [SpiritEffect]
    [CustomEffect(3)]
    internal class ExtraPiecesPushEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air }; } }
        public override double BaseProbability { get { return .01; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 4; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int extraPushAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.6;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 0.8 },
            {3, 0.6 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            if (extraPushAmount > 1)
            {
                return $"Once this turn when {Context.GetTargetString(TargetType)} pushes/gathers pieces, push/gather up to {extraPushAmount} more applicable pieces";
            }

            return $"Once this turn when {Context.GetTargetString(TargetType)} pushes/gathers pieces, push/gather an additional applicable piece";

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
                extraPushAmount = Context.rng.Next(1, 3);
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
            ExtraPiecesPushEffect effect = new ExtraPiecesPushEffect();
            effect.extraPushAmount = extraPushAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
