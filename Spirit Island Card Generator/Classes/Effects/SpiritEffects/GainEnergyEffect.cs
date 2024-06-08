using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
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
    internal class GainEnergyEffect : AmountEffect
    {
        public override string Name => "Gain Energy";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Sun }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Plant, Element.Water }; } }
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
        [AmountValue]
        public int energyAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.35;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.1 },
            {3, 1.1 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            return $"{Context.GetTargetString(TargetType)} gains {energyAmount} Energy";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
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
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
