using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
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
    internal class DestroyFewerDahanEffect : AmountEffect
    {
        public override string Name => "Destroy Fewer Dahan";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal }; } }
        public override double BaseProbability { get { return .01; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"next time Dahan would be Destroyed in target land, Destroy (\d) fewer Dahan", RegexOptions.IgnoreCase);
            }
        }

        [AmountValue]
        public int destroyPreventionAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength => 0.4;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2},
            {3, 1.3},
            {4, 1.4},
        };

        //Writes what goes on the card
        public override string Print()
        {
            return $"next time Dahan would be Destroyed, Destroy {destroyPreventionAmount} fewer Dahan";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoDahan))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            //TODO: Care about power level
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                destroyPreventionAmount = Context.rng.Next(1,3);
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
                destroyPreventionAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DestroyFewerDahanEffect effect = new DestroyFewerDahanEffect();
            effect.destroyPreventionAmount = destroyPreventionAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
