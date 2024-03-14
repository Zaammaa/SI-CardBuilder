using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    internal class Utils
    {
        public static T? ChooseRandomListElement<T>(IEnumerable<T> list, Random rng)
        {
            if (list.Count() == 0)
                return default;

            int i = rng.Next(0, list.Count());
            return list.ToArray()[i];
        }
        public static T? ChooseWeightedOption<T>(Dictionary<T, int> options, Random rng)
        {
            Dictionary<int, T> position = new Dictionary<int, T>();
            int sum = 0;
            foreach (KeyValuePair<T, int> entry in options)
            {
                if (entry.Value > 0)
                {
                    sum += entry.Value;
                    position.Add(sum, entry.Key);
                }
            }


            int roll = rng.Next(sum);
            foreach (int weight in position.Keys)
            {
                if (roll < weight)
                {
                    return position[weight];
                }
            }
            return default(T);

        }

        public static T ChooseRandomEnumValue<T>(Type e, Random rng) where T : Enum {
            T[] conditions = (T[])Enum.GetValues(typeof(T));
            int randomIndex = rng.Next(0, conditions.Length);
            return conditions[randomIndex];
        }
    }
}
