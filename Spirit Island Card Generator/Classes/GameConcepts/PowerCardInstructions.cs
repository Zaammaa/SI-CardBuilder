using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.GameConcepts
{
    internal class PowerCardInstructions
    {
        public enum PowerCardInstruction
        {
            ANY,
            Push,
            Gather,
            PushOrGather,
            Fear,
            Damage,
            Slow,
            Fast,
            Defend,
            Add,
            SacredSite,
        }

        //Pre-text for this will probably be somethin like:
        //Choose 1 of their slow Powers [template] and make that power fast
        //Choose 2 of their slow Powers [template] and make those powers fast
        //Repeat one of their Powers [template] (by paying its cost)
        //Reclaim 1 Power Card [template]
        public static Dictionary<PowerCardInstruction, string> InstructionConversions = new Dictionary<PowerCardInstruction, string>()
        {
            {PowerCardInstruction.ANY, ""},
            {PowerCardInstruction.Push, "with a \"Push\" instruction" },
            {PowerCardInstruction.Gather, "with a \"Gather\" instruction" },
            {PowerCardInstruction.PushOrGather, "with a \"Push\" or \"Gather\" instruction" },
            {PowerCardInstruction.Fear, "with a \"Fear\" instruction" },
            {PowerCardInstruction.Damage, "with a \"Damage\" instruction" },
            {PowerCardInstruction.Slow, "that is {slow}" },
            {PowerCardInstruction.Fast, "that is {fast}" },
            {PowerCardInstruction.Defend, "with a \"Defend\" instruction" },
            {PowerCardInstruction.Add, "with an \"Add\" instruction" },
            {PowerCardInstruction.SacredSite, "that targets from a {sacred-site}" },

        };

    }
}
