using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public interface GeneratorOption
    {
        double BaseProbability { get; }
        double AdjustedProbability { get; set; }
        int Complexity { get; }
        
        bool IsValid(Card card, Settings settings);
    }
}
