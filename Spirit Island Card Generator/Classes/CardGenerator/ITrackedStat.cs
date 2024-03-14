using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    internal interface ITrackedStat
    {
        //Name that gets put into the log
        public static string TrackedName { get; }
        //The amount of cards that should have this effect
        public static int TargetAmount { get; }
        //If true, the generator will try to make sure there are exaclty as many cards with the effect as the targetAmount.
        bool ExactTarget { get; }

        //Pools are used for effects that we try to keep a balance between
        //If a pool is set, the Effect generator will add it to a dictionary and try to keep the ratio between the elements about equal
        public enum Pool
        {
            None,
            Tokens,
            Elements,
            LandTargets
        }
        Pool pool {  get; }
    }
}
