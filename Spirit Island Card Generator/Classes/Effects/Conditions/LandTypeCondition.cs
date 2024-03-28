using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions
{
    internal class LandTypeCondition : Condition
    {
        public override double BaseProbability => 0.2;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 3;

        public struct WeightAndMultiplier
        {
            public double multiplier;
            public int weight;

            public WeightAndMultiplier(double m, int w)
            {
                multiplier = m;
                weight = w;
            }
        }

        public static Dictionary<LandConditions, WeightAndMultiplier> conditions = new Dictionary<LandConditions, WeightAndMultiplier>()
        {
            { LandConditions.Inland, new WeightAndMultiplier(0.9, 2)},
            { LandConditions.Coastal, new WeightAndMultiplier(0.7, 2)},

            { LandConditions.Mountain, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Jungle, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Sands, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Wetlands, new WeightAndMultiplier(0.6, 2)},

            { LandConditions.NoMountain, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoJungle, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoSands, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoWetlands, new WeightAndMultiplier(0.9, 1)},

            { LandConditions.MountainOrJungle, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.MountainOrSands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.MountainOrWetlands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.JungleOrSands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.JungleOrWetlands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.SandsOrWetlands, new WeightAndMultiplier(0.8, 4)},

            { LandConditions.Blighted, new WeightAndMultiplier(0.55, 2)},
            { LandConditions.Noblight, new WeightAndMultiplier(0.85, 3)},

            { LandConditions.Dahan, new WeightAndMultiplier(0.7, 3)},
            { LandConditions.NoDahan, new WeightAndMultiplier(0.85, 1)},

            { LandConditions.Invaders, new WeightAndMultiplier(0.8, 2)},
            { LandConditions.NoInvaders, new WeightAndMultiplier(0.65, 2)},
            { LandConditions.Buildings, new WeightAndMultiplier(0.7, 2)},
            { LandConditions.NoBuildings, new WeightAndMultiplier(0.7, 2)},
            { LandConditions.City, new WeightAndMultiplier(0.5, 2)},
            { LandConditions.NoCity, new WeightAndMultiplier(0.9, 2)},
        };

        public override double DifficultyMultiplier {
            get {
                return conditions[landCondition].multiplier;
            }
        }

        public override string ConditionText { 
            get {
                switch (landCondition)
                {
                    case LandConditon.LandConditions.Noblight:
                    case LandConditon.LandConditions.NoBuildings:
                    case LandConditon.LandConditions.NoDahan:
                    case LandConditon.LandConditions.Dahan:
                    case LandConditon.LandConditions.Blighted:
                    case LandConditon.LandConditions.City:
                    case LandConditon.LandConditions.Buildings:
                    case LandConditon.LandConditions.Invaders:
                        return "If target land has " + LandConditon.Print(landCondition) + ",";
                    default:
                        return "If target land is " + LandConditon.Print(landCondition) + ",";
                }
            } 
        }

        public LandConditon.LandConditions landCondition;

        public override IPowerLevel Duplicate()
        {
            LandTypeCondition condition = new LandTypeCondition();
            condition.landCondition = landCondition;
            return condition;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }

        public override void Initialize(Context context)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(context.target.landConditions);
            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition) && IsValidCondition(context, condition))
                    weights.Add(condition, conditions[condition].weight);
            }
            if (compatibleConditions.Count == 0)
            {
                throw new Exception("No valid conditions! Not really sure what to do here...");
            }

            landCondition = Utils.ChooseWeightedOption(weights, context.rng);
        }

        private bool IsValidCondition(Context context, LandConditions landCondition)
        {
            LandTypeCondition? conditionToReplace = null;
            if (context.conditions.LastOrDefault() == this)
                conditionToReplace = this;

            LandTypeCondition newCondition = (LandTypeCondition)Duplicate();
            newCondition.landCondition = landCondition;
            return IsValidForChildren(context, newCondition, conditionToReplace);
        }

        public override bool ChooseHarderCondition(Context context)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(context.target.landConditions);

            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition) && conditions[condition].multiplier < DifficultyMultiplier && IsValidCondition(context, condition))
                    weights.Add(condition, conditions[condition].weight);
            }

            if (weights.Count == 0)
                return false;

            LandConditions? newCondition =  Utils.ChooseWeightedOption(weights, context.rng);
            if (newCondition.HasValue)
            {
                landCondition = newCondition.Value;
                return true;
            } else
            {
                return false;
            }
        }
        public override bool ChooseEasierCondition(Context context)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(context.target.landConditions);

            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition) && conditions[condition].multiplier > DifficultyMultiplier && IsValidCondition(context, condition))
                    weights.Add(condition, conditions[condition].weight);
            }

            if (weights.Count == 0)
                return false;

            LandConditions? newCondition = Utils.ChooseWeightedOption(weights, context.rng);
            if (newCondition.HasValue)
            {
                landCondition = newCondition.Value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
