using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
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
    [SpiritEffect]
    [CustomEffect(4)]
    internal class ElementalInvocationEffect : AmountEffect
    {
        public override string Name => "Elemental Invocation";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Moon }; } }
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 6; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int markerAmount = 1;

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        ];

        public override double effectStrength => 0.5;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            if (Context.target.SpiritTarget)
            {
                if (markerAmount > 1)
                    return $"{Context.GetTargetString(TargetType)} may place {markerAmount} elemental markers (from the scenario \"Elemental Invocation\") on corners of their lands for 1 less total energy cost each.";
                else
                    return $"{Context.GetTargetString(TargetType)} may place {markerAmount} elemental marker (from the scenario \"Elemental Invocation\") on corners of their lands for 1 less total energy cost.";
            } else
            {
                if (markerAmount > 1)
                    return $"place {markerAmount} elemental markers (from the scenario \"Elemental Invocation\") on the corners of {Context.GetTargetString(TargetType)} for 1 less total energy cost each.";
                else
                    return $"place an elemental marker (from the scenario \"Elemental Invocation\") on the corners of {Context.GetTargetString(TargetType)} for 1 less total energy cost";
            }
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                markerAmount = Context.rng.Next(1, 3);
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
            ElementalInvocationEffect effect = new ElementalInvocationEffect();
            effect.markerAmount = markerAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
