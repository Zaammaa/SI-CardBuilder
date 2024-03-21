using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class SpeedBoostEffect : Effect
    {
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return .05; } set { } }
        public override int Complexity { get { return 3; } }

        public bool viceVersa = false;
        public int amount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Allow Vice versa", 80, AllowViceVersa, DontAllowViceVersa),
        };

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"This turn, Target Spirit may use (\d) Slow Power as if it were Fast", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            if (viceVersa)
            {
                return "This turn, Target Spirit may use " + amount + " {slow} power as if it were {fast}, or vice versa.";
            } else
            {
                return "This turn, Target Spirit may use " + amount + " {slow} power as if it were {fast}";
            }
            
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.card.Fast)
                return false;
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            viceVersa = Context.rng.NextDouble() >= 0.75 ? true : false;
            amount = Context.rng.Next(1, 3);
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double power = 0;
            if (viceVersa)
            {
                power += 0.1;
            }

            if (amount == 1)
            {
                power += 0.5;
            } else if (amount == 2)
            {
                power += 0.75;
            } else
            {
                power += 1;
            }

            return power;
        }

        protected Effect? IncreaseAmount()
        {
            SpeedBoostEffect newEffect = (SpeedBoostEffect)Duplicate();
            if (amount <= 1)
            {
                newEffect.amount += 1;
            } else if (amount == 2 && Context.rng.NextDouble() >= 0.8)
            {
                //getting to 3 fast can be possible, but it should be rare
                newEffect.amount += 1;
            } else
            {
                return null;
            }
            return newEffect;
        }

        protected Effect? DecreaseAmount()
        {
            SpeedBoostEffect newEffect = (SpeedBoostEffect)Duplicate();
            if (amount > 1)
            {
                newEffect.amount -= 1;
                return newEffect;
            }
            else
            {
                return null;
            }
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
            effect.amount = amount;
            effect.viceVersa = viceVersa;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
