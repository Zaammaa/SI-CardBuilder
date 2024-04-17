using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public interface IGeneratorOption
    {
        double BaseProbability { get; }
        double AdjustedProbability { get; set; }
        int Complexity { get; }

        //Whether there can be more than one effect of this type anywhere on the card. Most effects can be at least at the top level and then again under a condition
        //Examples of effects where that should not happen are complex effects like ORs, or conditional effects themselves like Elemental thresholds
        //public virtual bool TopLevelEffect { get { return false; } }

        bool TopLevelEffect();

        bool IsValidGeneratorOption(Context context);
    }
}
