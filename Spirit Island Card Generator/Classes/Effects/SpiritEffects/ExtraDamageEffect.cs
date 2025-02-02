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
    [CustomEffect(2)]
    internal class ExtraDamageEffect : AmountEffect
    {
        public override string Name => "Extra Damage";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Target Spirit may deal \+(\d{1,2}) Damage when using a Damage-dealing Power", RegexOptions.IgnoreCase);
            }
        }
        [AmountValue]
        public int extraDamageAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.5;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1.2 },
            {2, 1 },
            {3, 1 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            return $"Once this turn, {Context.GetTargetString(TargetType)} may deal +{extraDamageAmount} Damage when using a Damage-dealing Power";

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
                extraDamageAmount = Context.rng.Next(1, 3);
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
                extraDamageAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            ExtraDamageEffect effect = new ExtraDamageEffect();
            effect.extraDamageAmount = extraDamageAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
