using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions
{
    public abstract class Condition : IPowerLevel, IGeneratorOption
    {
        public string Name => ConditionText;
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }

        public virtual List<ElementSet.Element> StronglyAssociatedElements { get { return new List<ElementSet.Element>(); } }
        public virtual List<ElementSet.Element> WeaklyAssociatedElements { get { return new List<ElementSet.Element>(); } }

        public double CalculatePowerLevel()
        {
            return 0;
        }
        public abstract IPowerLevel Duplicate();
        public abstract bool IsValidGeneratorOption(Context context);

        public abstract double DifficultyMultiplier { get; }

        public abstract string ConditionText { get; }

        public bool Singleton => false;

        public abstract void Initialize(Context context);

        public abstract bool ChooseHarderCondition(Context context);
        public abstract bool ChooseEasierCondition(Context context);

        public bool TopLevelEffect()
        {
            return false;
        }

        protected bool IsValidForChildren(Context context, Condition condition, Condition? conditionToReplace)
        {
            Context possibleContext = context.Duplicate();
            if (conditionToReplace != null)
            {
                possibleContext.conditions.Remove(conditionToReplace);
            }
            possibleContext.conditions.Add(condition);
            List<Effect> children = context.GetSiblings();
            foreach (Effect child in children)
            {
                if (!child.IsValidGeneratorOption(context))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
