using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    public class FearEffect : AmountEffect
    {
        public override double BaseProbability { get { return .33; } }
        public override double AdjustedProbability { get { return .33; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex { 
            get {
                return new Regex(@"(\d{1,2}) fear", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone { get { return false; } }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change Fear", 100, IncreaseAmount, DecreaseAmount)
        };

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1},
            {2, 1},
            {3, 1},
        };

        public override double effectStrength => 0.3;

        public override int PrintOrder()
        {
            return 1;
        }

        [AmountValue]
        public int fearAmount = 1;

        //Writes what goes on the card
        public override string Print()
        {
            return fearAmount + " {fear}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            //TODO: Care about power level
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                fearAmount = Context.rng.Next(1, 4);
            } else
            {
                throw new NotImplementedException();
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                fearAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            FearEffect effect = new FearEffect();
            effect.Context = Context.Duplicate();
            effect.fearAmount = fearAmount;
            return effect;
        }
    }
}
