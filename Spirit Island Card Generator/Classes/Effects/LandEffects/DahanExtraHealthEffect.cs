using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class DahanExtraHealthEffect : AmountEffect
    {
        public override double BaseProbability { get { return .01; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"dahan have \+(\d{1,2}) Health while in target land", RegexOptions.IgnoreCase);
            }
        }

        [AmountValue]
        public int extraDahanHealth = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.2;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1 },
            {3, 0.9 },
            {4, 0.8 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            return "{dahan}" + $" have +{extraDahanHealth} Health while in target land";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.target.SpiritTarget || context.target.landConditions.Contains(LandConditions.NoDahan) || !context.card.Fast || context.target.landConditions.Contains(LandConditions.NoInvaders))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            extraDahanHealth = 2;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                extraDahanHealth = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DahanExtraHealthEffect effect = new DahanExtraHealthEffect();
            effect.extraDahanHealth = extraDahanHealth;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
