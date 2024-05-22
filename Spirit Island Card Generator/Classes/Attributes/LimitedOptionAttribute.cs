using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Attributes
{
    internal class LimitedOptionAttribute : Attribute
    {
        public int maxUses = 1;
        public LimitedOptionAttribute(int maxUses)
        {
            this.maxUses = maxUses;
        }
    }
}
