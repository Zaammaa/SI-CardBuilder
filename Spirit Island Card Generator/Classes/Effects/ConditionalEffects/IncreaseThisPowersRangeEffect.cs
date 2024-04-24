using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffect]
    [UnspecificLand]
    internal class IncreaseThisPowersRangeEffect : Effect
    {
        public override double BaseProbability { get { return .1; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 1; } }

        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air}; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Increase this Power's Range by (\d)", RegexOptions.IgnoreCase);
            }
        }
        protected int rangeIncrease = 1;

        public override bool Standalone { get { return false; } }

        protected override DifficultyOption[] difficultyOptions =>
        [
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
        ];

        //Writes what goes on the card
        public override string Print()
        {
            return $"Increase this Power's Range by {rangeIncrease}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.SpiritTarget || context.card.Range.range >= 2)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            if (Context.card.Range.range == 0)
            {
                return 0.2 * rangeIncrease;
            } else if (Context.card.Range.range == 1) { 
                return 0.1 * rangeIncrease;
            } else
            {
                return 0.05 * rangeIncrease;
            }
        }

        protected Effect? IncreaseAmount()
        {
            if (rangeIncrease <= 1)
            {
                IncreaseThisPowersRangeEffect effect = (IncreaseThisPowersRangeEffect)Duplicate();
                effect.rangeIncrease += 1;
                return effect;
            } else
            {
                return null;
            }
        }

        protected Effect? DecreaseAmount()
        {
            if (rangeIncrease > 1)
            {
                IncreaseThisPowersRangeEffect effect = (IncreaseThisPowersRangeEffect)Duplicate();
                effect.rangeIncrease -= 1;
                return effect;
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
                rangeIncrease = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            IncreaseThisPowersRangeEffect effect = new IncreaseThisPowersRangeEffect();
            effect.rangeIncrease = rangeIncrease;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
