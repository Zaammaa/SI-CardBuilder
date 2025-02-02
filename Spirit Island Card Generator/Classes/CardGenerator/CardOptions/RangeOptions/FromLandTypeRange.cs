using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    internal class FromLandTypeRange : FromLandOption
    {
        public override string Name => "From Land: " + land;
        public override double BaseProbability => 0.01;

        public override int Complexity => 2;

        public Lands.LandTypes land;

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Target.SpiritTarget)
                return false;
            //Range obviously needs to be higher than 0, and 
            if (context.settings.CustomEffectLevel > 1 && context.card.Range.range > 0 && (!context.card.Range.sacredSite || context.settings.CustomEffectLevel >= 4))
            {
                return true;
            }
            return false;
        }

        public override Range OnSelected(Context context)
        {
            Range range = context.card.Range;
            List<Lands.LandTypes> lands = new List<Lands.LandTypes>() { Lands.LandTypes.Sands, Lands.LandTypes.Wetlands, Lands.LandTypes.Mountains, Lands.LandTypes.Jungles};
            land = Utils.ChooseRandomListElement(lands, context.rng);
            range.SourceLand = land;
            return range;
        }
    }
}
