using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
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
    public class DamageEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun, Element.Animal }; } }
        public override double BaseProbability { get { return .20; } }
        public override double AdjustedProbability { get { return .20; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"(\d{1,2}) Damage", RegexOptions.IgnoreCase);
            }
        }

        [AmountValue]
        public int damageAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

        public override double effectStrength {
            get
            {
                if (Context != null && Context.card.Fast)
                    return (double)damageAmount * 0.8;
                else
                    return (double)(damageAmount * 0.7);
            }
        }

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2 },
            {3, 1.5 },
        };

        //Writes what goes on the card
        public override string Print()
        {
            return damageAmount + " Damage.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.SpiritTarget || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders))
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
                damageAmount = 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            if (Context.card.Fast)
                return (double)damageAmount * 0.9;
            else
                return (double)(damageAmount * 0.7);
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                damageAmount = Int32.Parse(match.Groups[1].Value);
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DamageEffect effect = new DamageEffect();
            effect.damageAmount = damageAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
