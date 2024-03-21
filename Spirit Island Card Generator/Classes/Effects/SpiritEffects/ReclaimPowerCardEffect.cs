using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class ReclaimPowerCardEffect : Effect
    {
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        //From play or from discard.
        public bool fromPlay = false;
        public int amount = 1;

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Reclaim (\d) Power Card", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
{
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
};

        //Writes what goes on the card
        public override string Print()
        {
            string powerCardText = amount > 1 ? "Power Cards" : "Power Card";
            if (fromPlay)
            {
                return "At the end of turn, Target Spirit may Reclaim " + amount + $" {powerCardText} instead of discarding {(amount > 1 ? "them" : "it")}";
            }
            else
            {
                return "Target Spirit Reclaims " + amount + $" {powerCardText}.";
            }

        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            fromPlay = Context.rng.NextDouble() >= 0.33 ? true : false;
            amount = Context.rng.Next(1, 3);
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            if (fromPlay)
            {
                return 1.3 * amount;
            } else
            {
                return 1.5 * amount;
            }
        }

        protected Effect? IncreaseAmount()
        {
            ReclaimPowerCardEffect newEffect = (ReclaimPowerCardEffect)Duplicate();
            if (amount <= 1)
            {
                newEffect.amount += 1;
            }
            else
            {
                return null;
            }
            return newEffect;
        }

        protected Effect? DecreaseAmount()
        {
            ReclaimPowerCardEffect newEffect = (ReclaimPowerCardEffect)Duplicate();
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
            ReclaimPowerCardEffect effect = new ReclaimPowerCardEffect();
            effect.amount = amount;
            effect.fromPlay = fromPlay;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
