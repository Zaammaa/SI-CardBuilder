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
        public Func<FixerResult> fixer = delegate { return null; };
        public GenericFixer(Func<FixerResult> fix) { 
            fixer = fix;
        }

        public FixerResult Fix()
        {
            return fixer.Invoke();
        }
    }
}
