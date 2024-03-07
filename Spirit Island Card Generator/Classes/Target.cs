using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes
{
    public class Target
    {
        public bool SpiritTarget = false;
        public List<LandConditions> landConditions = new List<LandConditions>();


        private Dictionary<string, LandConditions> katalogStringToCondition = new Dictionary<string, LandConditions>()
        {
            {"mountain", LandConditions.Mountain},
            {"jungle", LandConditions.Jungle},
            {"sands", LandConditions.Sands},
            {"wetland", LandConditions.Wetlands},
            {"no mountain", LandConditions.NoMountain},
            {"no jungle", LandConditions.NoJungle},
            {"no sands", LandConditions.NoSands},
            {"no wetland", LandConditions.NoWetlands},
            {"mountain, sands", LandConditions.MountainOrSands},
            {"mountain, jungle", LandConditions.MountainOrJungle},
            {"mountain, wetland", LandConditions.MountainOrWetlands},
            {"jungle, sands", LandConditions.JungleOrSands},
            {"jungle, wetland", LandConditions.JungleOrWetlands},
            {"sands, wetland", LandConditions.SandsOrWetlands},
            {"inland", LandConditions.Inland},
            {"coastal", LandConditions.Coastal},
            {"blight", LandConditions.Blighted},
            {"no blight", LandConditions.Noblight},
            {"dahan", LandConditions.Dahan},
            {"no dahan", LandConditions.NoDahan},
            {"invaders", LandConditions.Invaders},
            {"no invaders", LandConditions.NoInvaders},
        };

        public void AddCondition(string conditionString)
        {
            if (katalogStringToCondition.ContainsKey(conditionString))
            {
                landConditions.Add(katalogStringToCondition[conditionString]);
            }
        }

        /// <summary>
        /// Reading from the card katalog.
        /// </summary>
        /// <param name="str">the target property from the katalog</param>
        /// <returns></returns>
        public static Target FromString(string str)
        {
            Target target = new Target();
            if (str.Contains("Target Spirit", StringComparison.InvariantCultureIgnoreCase) || str.Contains("Another Spirit",StringComparison.InvariantCultureIgnoreCase) || str.Contains("Yourself", StringComparison.InvariantCultureIgnoreCase))
            {
                target.SpiritTarget = true;
                //TODO: add the right condition
            } else
            {
                target.SpiritTarget = false;

                target.AddCondition(str);
            }
            return target;
        }

        public string Print()
        {
            if (SpiritTarget)
            {
                return "Another {spirit}";
            } else if (landConditions.Count == 0)
            {
                return "Any";
            } else
            {
                return String.Join("/", landConditions);
            }
        }
    }
}
