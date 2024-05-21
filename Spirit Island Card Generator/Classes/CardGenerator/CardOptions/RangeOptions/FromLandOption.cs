using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    abstract class FromLandOption : IGeneratorOption
    {
        public abstract double BaseProbability { get; }
        public double AdjustedProbability { get => BaseProbability; set { } }

        public abstract int Complexity { get; }
        public List<ElementSet.Element> StronglyAssociatedElements => new List<ElementSet.Element>();
        public List<ElementSet.Element> WeaklyAssociatedElements => new List<ElementSet.Element>();



        public abstract bool IsValidGeneratorOption(Context context);
        public bool TopLevelEffect()
        {
            return false;
        }

        public abstract Range OnSelected(Context context);
    }
}
