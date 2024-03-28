using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.Lands;

namespace Spirit_Island_Card_Generator.Classes.TargetConditions
{

    public class LandConditon
    {
        //public List<LandTypes> matchesLandTypes = new List<LandTypes>();
        //public List<LandTypes> forbiddenLandTypes = new List<LandTypes>();
        public enum LandConditions
        {
            Mountain,
            Jungle,
            Sands,
            Wetlands,
            //Combinations
            MountainOrJungle,
            MountainOrSands,
            MountainOrWetlands,
            JungleOrSands,
            JungleOrWetlands,
            SandsOrWetlands,
            //Not terrain types
            NoMountain,
            NoJungle,
            NoSands,
            NoWetlands,
            Coastal,
            Inland,
            Blighted,
            Noblight,
            Invaders,
            NoInvaders,
            Dahan,
            NoDahan,
            Buildings,
            NoBuildings,
            City,
            NoCity
            //TODO: add more
        }

        //If a card has the targeting condition in the key, it implies the conditions in the list are always met.
        public static Dictionary<LandConditions, List<LandConditions>> implications = new Dictionary<LandConditions, List<LandConditions>>() {
            {LandConditions.Mountain, new List<LandConditions>() { LandConditions.MountainOrJungle, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoJungle, LandConditions.NoSands, LandConditions.NoWetlands} },
            {LandConditions.Jungle, new List<LandConditions>() { LandConditions.MountainOrJungle, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.NoMountain, LandConditions.NoSands, LandConditions.NoWetlands } },
            {LandConditions.Sands, new List<LandConditions>() { LandConditions.MountainOrSands, LandConditions.JungleOrSands, LandConditions.SandsOrWetlands,  LandConditions.NoMountain, LandConditions.NoJungle, LandConditions.NoWetlands } },
            {LandConditions.Wetlands, new List<LandConditions>() { LandConditions.MountainOrWetlands, LandConditions.JungleOrWetlands, LandConditions.SandsOrWetlands, LandConditions.NoMountain, LandConditions.NoSands, LandConditions.NoJungle } },

            {LandConditions.MountainOrJungle, new List<LandConditions>() { LandConditions.NoSands, LandConditions.NoWetlands } },
            {LandConditions.MountainOrSands, new List<LandConditions>() { LandConditions.NoJungle, LandConditions.NoWetlands } },
            {LandConditions.MountainOrWetlands, new List<LandConditions>() { LandConditions.NoJungle, LandConditions.NoSands } },
            {LandConditions.JungleOrSands, new List<LandConditions>() { LandConditions.NoMountain, LandConditions.NoWetlands } },
            {LandConditions.JungleOrWetlands, new List<LandConditions>() { LandConditions.NoMountain, LandConditions.NoSands } },
            {LandConditions.SandsOrWetlands, new List<LandConditions>() { LandConditions.NoMountain, LandConditions.NoJungle } },

            {LandConditions.Buildings, new List<LandConditions>() { LandConditions.Invaders} },
            {LandConditions.City, new List<LandConditions>() { LandConditions.Invaders, LandConditions.Buildings} },
            {LandConditions.NoInvaders, new List<LandConditions>() { LandConditions.NoBuildings, LandConditions.NoCity} },
            {LandConditions.NoBuildings, new List<LandConditions>() { LandConditions.NoCity} },


        };

        //If a card has the targeting condition in the key, it means the conditions in the list are never met.
        public static Dictionary<LandConditions, List<LandConditions>> incompatible = new Dictionary<LandConditions, List<LandConditions>>() {
            {LandConditions.Mountain, new List<LandConditions>() { LandConditions.NoMountain, LandConditions.Jungle, LandConditions.Sands, LandConditions.Wetlands } },
            {LandConditions.NoMountain, new List<LandConditions>() { LandConditions.Mountain, LandConditions.MountainOrJungle, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands} },
            {LandConditions.Jungle, new List<LandConditions>() { LandConditions.NoJungle, LandConditions.Mountain, LandConditions.Sands, LandConditions.Wetlands } },
            {LandConditions.NoJungle, new List<LandConditions>() { LandConditions.Jungle, LandConditions.MountainOrJungle, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands } },
            {LandConditions.Sands, new List<LandConditions>() { LandConditions.NoSands, LandConditions.Mountain, LandConditions.Jungle, LandConditions.Wetlands } },
            {LandConditions.NoSands, new List<LandConditions>() { LandConditions.Sands, LandConditions.MountainOrSands, LandConditions.JungleOrSands, LandConditions.SandsOrWetlands } },
            {LandConditions.Wetlands, new List<LandConditions>() { LandConditions.NoWetlands, LandConditions.Mountain, LandConditions.Sands, LandConditions.Jungle } },
            {LandConditions.NoWetlands, new List<LandConditions>() { LandConditions.Wetlands, LandConditions.MountainOrWetlands, LandConditions.JungleOrWetlands, LandConditions.SandsOrWetlands } },

            {LandConditions.Blighted, new List<LandConditions>() { LandConditions.Noblight} },
            {LandConditions.Noblight, new List<LandConditions>() { LandConditions.Blighted} },

            {LandConditions.Invaders, new List<LandConditions>() { LandConditions.NoInvaders} },
            {LandConditions.NoInvaders, new List<LandConditions>() { LandConditions.Invaders, LandConditions.Buildings, LandConditions.City }  },
            {LandConditions.Buildings, new List<LandConditions>() { LandConditions.NoInvaders, LandConditions.NoBuildings} },
            {LandConditions.City, new List<LandConditions>() { LandConditions.NoInvaders, LandConditions.NoBuildings, LandConditions.NoCity} },
            {LandConditions.NoCity, new List<LandConditions>() { LandConditions.City} },

            {LandConditions.Dahan, new List<LandConditions>() { LandConditions.NoDahan} },
            {LandConditions.NoDahan, new List<LandConditions>() { LandConditions.Dahan} },

            {LandConditions.Coastal, new List<LandConditions>() { LandConditions.Inland} },
            {LandConditions.Inland, new List<LandConditions>() { LandConditions.Coastal} },
            
            //Technically, (Mountains or Jungle) and (Mountain or Sands) aren't logically incompatitble, but it doesn't make any sense to design a card that way.
            {LandConditions.MountainOrJungle, new List<LandConditions>() { LandConditions.SandsOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoMountain, LandConditions.NoJungle, LandConditions.Sands, LandConditions.Wetlands} },
            {LandConditions.MountainOrSands, new List<LandConditions>() { LandConditions.SandsOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoMountain, LandConditions.NoSands, LandConditions.Jungle, LandConditions.Wetlands} },
            {LandConditions.MountainOrWetlands, new List<LandConditions>() { LandConditions.SandsOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoMountain, LandConditions.NoWetlands, LandConditions.Jungle, LandConditions.Sands} },
            {LandConditions.JungleOrSands, new List<LandConditions>() { LandConditions.SandsOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoJungle, LandConditions.NoSands, LandConditions.Mountain, LandConditions.Wetlands} },
            {LandConditions.JungleOrWetlands, new List<LandConditions>() { LandConditions.SandsOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoJungle, LandConditions.NoWetlands, LandConditions.Mountain, LandConditions.Sands} },
            {LandConditions.SandsOrWetlands, new List<LandConditions>() { LandConditions.MountainOrJungle, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.NoSands, LandConditions.NoWetlands, LandConditions.Mountain, LandConditions.Jungle} },
            
        };

        private LandConditions condition;

        public LandConditon(LandConditions cond) { 
            condition = cond;
        }

        //Certain conditions have special text that should be saved so it can be turned into icons on Spirit Island Builder
        public static Dictionary<LandConditions, string> BuilderConversions = new Dictionary<LandConditions, string>() {
            {LandConditions.Mountain, "{mountain}" },
            {LandConditions.Jungle, "{jungle}" },
            {LandConditions.Sands, "{sand}" },
            {LandConditions.Wetlands, "{wetland}" },
            //Combinations
            {LandConditions.MountainOrJungle, "{mountain-jungle}" },
            {LandConditions.MountainOrSands, "{mountain-sand}" },
            {LandConditions.MountainOrWetlands, "{mountain-wetland}" },
            {LandConditions.JungleOrSands, "{jungle-sand}" },
            {LandConditions.JungleOrWetlands, "{jungle-wetland}" },
            {LandConditions.SandsOrWetlands, "{sand-wetland}" },
            //Not terrain types
            {LandConditions.NoMountain, "{no-mountain}" },
            {LandConditions.NoJungle, "{no-jungle}" },
            {LandConditions.NoSands, "{no-sand}" },
            {LandConditions.NoWetlands, "{no-wetland}" },
            //Other
            {LandConditions.Blighted, "{blight}" },
            {LandConditions.Noblight, "{no-blight}" },
            {LandConditions.Dahan, "{dahan}" },
            {LandConditions.NoDahan, "{no-dahan}" },
            {LandConditions.Buildings, "{town}/{city}" },
            {LandConditions.NoBuildings, "{no-town}/{no-city}" },
            {LandConditions.City, "{city}" },
            {LandConditions.NoCity, "{no-city}" },
            {LandConditions.Invaders, "Invaders" },
            {LandConditions.NoInvaders, "{no}Invaders" },
            {LandConditions.Coastal, "Coastal" },
            {LandConditions.Inland, "Inland" },
        };

        public static string Print(LandConditions landConditions)
        {
            if (!BuilderConversions.ContainsKey(landConditions))
            {
                return landConditions.ToString();
            }
            else
            {
                return BuilderConversions[landConditions].ToString();
            }
        }

        public static bool Incompatible(LandConditions landConditions, LandConditions newLandConditions)
        {
            if (incompatible.ContainsKey(landConditions) && incompatible[landConditions].Contains(newLandConditions))
            {
                return false;
            }
            return true;
        }

        public static bool Implicated(LandConditions landConditions, LandConditions newLandCondition)
        {
            if (implications.ContainsKey(landConditions) && implications[landConditions].Contains(newLandCondition))
            {
                return false;
            }
            return true;
        }

        public static List<LandConditions> GetCompatibleLandConditions(List<LandConditions> currentLandConditions)
        {
            List<LandConditions> allConditions = Enum.GetValues(typeof(LandConditions)).Cast<LandConditions>().ToList();
            List<LandConditions> invalidConditions = new List<LandConditions>();
            foreach (LandConditions condition in currentLandConditions)
            {
                if (incompatible.ContainsKey(condition))
                {
                    invalidConditions.AddRange(incompatible[condition]);
                }
                if (implications.ContainsKey(condition))
                {
                    invalidConditions.AddRange(implications[condition]);
                }
                if (currentLandConditions.Contains(condition))
                {
                    invalidConditions.Add(condition);
                }
            }

            allConditions.RemoveAll(invalidConditions.Contains);

            return allConditions;
        }

        public override string ToString()
        {
            if (!BuilderConversions.ContainsKey(condition))
            {
                return condition.ToString();
            } else
            {
                return BuilderConversions[condition].ToString();
            }
        }
    }
}
