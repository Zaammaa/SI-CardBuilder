using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Fixers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Interfaces
{
    public interface IValidFixer
    {
        //Attempts to make the effect valid
        //If it fails this will return null
        //If it succeeds, this will return the new effect
        public FixerResult Fix();
    }
}
