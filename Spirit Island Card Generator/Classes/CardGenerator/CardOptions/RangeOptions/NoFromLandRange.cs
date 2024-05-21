using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    /// <summary>
    /// This is needed to provide an alternative to the generator for no source land terrain
    /// </summary>
    internal class NoFromLandRange : FromLandOption
    {
        public override double BaseProbability => 0.99;

        public override int Complexity => 0;

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Target.SpiritTarget)
                return false;
            return true;
        }

        public override Range OnSelected(Context context)
        {
            return context.card.Range;
        }
    }
}
