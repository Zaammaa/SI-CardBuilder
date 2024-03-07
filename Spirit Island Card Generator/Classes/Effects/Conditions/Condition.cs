using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions
{
    public abstract class Condition : IPowerPowerLevel, GeneratorOption
    {
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }

        public double CalculatePowerLevel()
        {
            return 0;
        }
        public abstract IPowerPowerLevel Duplicate();
        public abstract bool IsValid(Card card, Settings settings);

        public abstract double DifficultyMultiplier { get; }

        public abstract string ConditionText { get; }

        public abstract void Initialize(Card card, Settings settings);

        public abstract bool ChooseHarderCondition(Card card, Settings settings);
        public abstract bool ChooseEasierCondition(Card card, Settings settings);
    }
}
