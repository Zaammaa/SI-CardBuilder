using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
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
    internal class SpeedBoostEffect : AmountEffect
    {
        public override string Name => "Speed Boost";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air}; } }
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return .05; } set { } }
        public override int Complexity { get { return 3; } }

        public bool viceVersa = false;
        [AmountValue]
        public int speedBoostAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Allow Vice versa", 20, AllowViceVersa, DontAllowViceVersa),
        };

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"This turn, Target Spirit may use (\d) Slow Power as if it were Fast", RegexOptions.IgnoreCase);
            }
        }

        public override double effectStrength => 0.7;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, .5 }
        };

        //Writes what goes on the card
        public override string Print()
        {
            if (viceVersa)
            {
                return $"This turn, {Context.GetTargetString(TargetType)} may use " + speedBoostAmount + " {slow} power as if it were {fast}, or vice versa.";
            } else
            {
                return $"This turn, {Context.GetTargetString(TargetType)} may use " + speedBoostAmount + " {slow} power as if it were {fast}";
            }
            
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            viceVersa = Context.rng.NextDouble() >= 0.75 ? true : false;
            speedBoostAmount = Context.rng.Next(1, 3);
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double power = base.CalculatePowerLevel();
            if (viceVersa)
            {
                power += 0.1;
            }

            return power;
        }

        protected Effect? AllowViceVersa()
        {
            if (!viceVersa)
            {
                SpeedBoostEffect newEffect = (SpeedBoostEffect)Duplicate();
                newEffect.viceVersa = true;
                return newEffect;
            }
            return null;
        }

        protected Effect? DontAllowViceVersa()
        {
            if (viceVersa)
            {
                SpeedBoostEffect newEffect = (SpeedBoostEffect)Duplicate();
                newEffect.viceVersa = false;
                return newEffect;
            }
            return null;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            SpeedBoostEffect effect = new SpeedBoostEffect();
            effect.speedBoostAmount = speedBoostAmount;
            effect.viceVersa = viceVersa;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
