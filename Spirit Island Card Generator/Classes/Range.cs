using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.Lands;

namespace Spirit_Island_Card_Generator.Classes
{
    public class Range
    {
        public bool sacredSite = false;
        public int range = -1;
        public LandTypes? SourceLand = null;

        public Range(bool sacredSite, int range, LandTypes? sourceLand)
        {
            this.sacredSite = sacredSite;
            this.range = range;
            SourceLand = sourceLand;
        }

        public override string ToString()
        {
            if (this.sacredSite)
            {
                return $"sacred-site,{range}";
            } else if (SourceLand.HasValue)
            {
                return $"{SourceLand?.ToString().ToLower()}-presence,{range}";
            } else if (range >= 0)
            {
                return range.ToString();
            } else
            {
                return "none";
            }
        }

        public string Print()
        {
            if (range < 0)
                return "--";

            string output = "";

            if (sacredSite)
            {
                output += $"sacred-site,{range}";
            } else
            {
                output += range.ToString();
            }
            if (this.SourceLand != null)
            {
                output += $",{SourceLand.ToString().ToLower()}-presence";
            }
            return output;
        }
    }
}
