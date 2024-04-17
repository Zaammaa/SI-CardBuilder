using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions
{
    [CostCondition]
    internal class PayEnergyCondition : Condition
    {
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun }; } }

        public override double DifficultyMultiplier
        {
            get
            {
                return multipliers[energyAmount];
            }
        }

        public Dictionary<int, double> multipliers = new Dictionary<int, double>()
        {
            {1, 0.6},
            {2, 0.45},
            {3, 0.2}
        };

        public override string ConditionText => $"You may pay {energyAmount} Energy to";

        protected int energyAmount = 1;

        public override bool ChooseEasierCondition(Context context)
        {
            if (energyAmount > 1)
            {
                energyAmount -= 1;
                return true;
            } else
            {
                return false;
            }
        }

        public override bool ChooseHarderCondition(Context context)
        {
            if (energyAmount < multipliers.Count)
            {
                energyAmount += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override IPowerLevel Duplicate()
        {
            PayEnergyCondition condition = new PayEnergyCondition();
            condition.energyAmount = energyAmount;
            return condition;
        }

        public override void Initialize(Context context)
        {
            
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}
