using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Interfaces
{
    //Interface for Effects that have children effects
    public interface IParentEffect
    {
        IEnumerable<Effect> GetChildren();
    }
}
