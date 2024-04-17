using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    internal class GenericFixer : IValidFixer
    {
        public Func<Effect?> fixer = delegate { return null; };
        public GenericFixer(Func<Effect?> fix) { 
            fixer = fix;
        }

        public Effect? Fix()
        {
            return fixer.Invoke();
        }
    }
}
