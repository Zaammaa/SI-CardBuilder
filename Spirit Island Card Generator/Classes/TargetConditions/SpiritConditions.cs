using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.TargetConditions
{
    public class SpiritConditions
    {
        public enum SpiritTargets
        {
            Yourself,
            AnySpirit,
            AnotherSpirit
        }

        public SpiritTargets spiritCondition;
        public SpiritConditions(SpiritTargets target)
        {
            spiritCondition = target;
        }

        //Certain conditions have special text that should be saved so it can be turned into icons on Spirit Island Builder
        private static Dictionary<SpiritTargets, string> BuilderConversions = new Dictionary<SpiritTargets, string>() {
            {SpiritTargets.AnySpirit, "Any {spirit}" },
            {SpiritTargets.AnotherSpirit, "Another {spirit}" },
        };

        public override string ToString()
        {
            if (!BuilderConversions.ContainsKey(spiritCondition))
            {
                return spiritCondition.ToString();
            }
            else
            {
                return BuilderConversions[spiritCondition].ToString();
            }
        }
    }
}
