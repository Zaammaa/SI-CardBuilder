using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    abstract class TargetOption : IGeneratorOption
    {
        public abstract double BaseProbability { get; }
        public double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract int Complexity { get; }
        public List<ElementSet.Element> StronglyAssociatedElements => new List<ElementSet.Element>();
        public List<ElementSet.Element> WeaklyAssociatedElements => new List<ElementSet.Element>();

        public bool Singleton => true;

        public abstract bool IsValidGeneratorOption(Context context);
        public bool TopLevelEffect()
        {
            return false;
        }

        public abstract Target OnChosen(Context context);
    }
}
