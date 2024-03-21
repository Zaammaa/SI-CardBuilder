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

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class GainEnergyEffect : Effect
    {
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

        public int energyAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
{
            new DifficultyOption("Change amount", 80, IncreaseAmount, DecreaseAmount),
};

        //Writes what goes on the card
        public override string Print()
        {
            return $"Target Spirit gains {energyAmount} Energy.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
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
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return (double)energyAmount / 2;
        }

        protected Effect? IncreaseAmount()
        {
            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                GainEnergyEffect newEffect = (GainEnergyEffect)Duplicate();
                newEffect.energyAmount += 1;
                return newEffect;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected Effect? DecreaseAmount()
        {
            if (energyAmount > 1)
            {
                GainEnergyEffect newEffect = (GainEnergyEffect)Duplicate();
                newEffect.energyAmount -= 1;
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
