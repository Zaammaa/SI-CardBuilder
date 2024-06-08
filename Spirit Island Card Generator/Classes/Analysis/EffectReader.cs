using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace Spirit_Island_Card_Generator.Classes.Analysis
{
    /// <summary>
    /// Attempts to create a list of effects from the card descriptions
    /// </summary>
    public class EffectReader
    {
        public Dictionary<string, List<Effect>> hardCodedCards = new Dictionary<string, List<Effect>>()
        {
            //Add cards that can't be scanned well here
        };

        public EffectReader()
        {

        }

        public bool ScanDescription(Card card)
        {
            bool parsingFailure = false;
            if (hardCodedCards.ContainsKey(card.Name))
            {
                card.effects = hardCodedCards[card.Name];
                return true;
            } else
            {
                string desc = card.description;
                desc = Regex.Replace(desc, @"\s{2,}", " ");
                List<string> effectBlocks = ParseEffectBlocks(desc);

                foreach (string block in effectBlocks)
                {
                    bool added = false;
                    foreach (Effect effect in ReflectionManager.GetInstanciatedSubClasses<Effect>())
                    {
                        if (effect.Scan(block.Trim()))
                        {
                            card.effects.Add(effect);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        Log.Warning($"No effect found that could parse description block for card {card.Name}: {block}");
                        parsingFailure = true;
                    }
                }
            }
            return !parsingFailure;
        }
        /// <summary>
        /// Splits a description into sections that can be parsed individually.
        /// Sadly, the descriptions we get in do not have line breaks. But they do have periods.
        /// "or" cards are going to be a pain with this system. Some of those may need to be done manually
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        private List<string> ParseEffectBlocks(string desc)
        {
            string[] periodBlocks = desc.Split('.');
            List<string> allBlocks = new List<string>();
            foreach(string block in periodBlocks)
            {
                if (block.Equals("") || block.Equals(")"))
                    continue;

                Match match = Regex.Match(block, @"\b(push|gather|destroy|add)\b \d \w+ \band\b", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string action = match.Groups[1].Value;
                    string[] andBlocks = Regex.Split(block, @"\band\b");
                    allBlocks.Add(andBlocks[0].Trim());
                    for (int i = 1; i < andBlocks.Length; i++)
                    {
                        allBlocks.Add($"{action.Trim()} {andBlocks[i].Trim()}");
                    }
                } else
                {
                    //string[] andBlocks = Regex.Split(block, @"\band\b");
                    //foreach(string andBlock in andBlocks)
                    //{
                    //    allBlocks.Add(andBlock.Trim());
                    //}
                    allBlocks.Add(block.Trim());

                }
            }
            return allBlocks;
        }

        //private List<Effect> GetEffects()
        //{

        //    List<Type> effectTypes = ReflectionManager.GetSubClasses<Effect>();
            
        //    List<Effect> effects = new List<Effect>();
        //    foreach (Type type in effectTypes)
        //    {
        //        if (type.IsClass && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
        //        {
        //            // Instantiate the class using Activator.CreateInstance
        //            object instance = Activator.CreateInstance(type);

        //            // If needed, cast the instance to the Effect class
        //            Effect effectInstance = (Effect)instance;

        //            // Now you can use the instantiated object
        //            // For example, you can call methods or set properties
        //            effects.Add(effectInstance);
        //        }
        //    }
        //    return effects;
        //}

        //private List<Type> GetEffectClasses()
        //{
        //    string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        //    List<Type> derivedTypes = new List<Type>();

        //    // Get all .dll and .exe files in the directory and its subdirectories
        //    string[] files = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories)
        //        .Concat(Directory.GetFiles(directoryPath, "*.exe", SearchOption.AllDirectories))
        //        .ToArray();

        //    // Iterate through each assembly file
        //    foreach (string file in files)
        //    {
        //        try
        //        {
        //            // Load the assembly
        //            Assembly assembly = Assembly.LoadFrom(file);

        //            // Get all types in the assembly
        //            Type[] types = assembly.GetTypes();

        //            // Filter types that inherit from MyBaseClass
        //            foreach (Type type in types)
        //            {
        //                if (type.IsSubclassOf(typeof(Effect)))
        //                {
        //                    derivedTypes.Add(type);
        //                }
        //            }
        //        }
        //        catch (ReflectionTypeLoadException ex)
        //        {
        //            Console.WriteLine($"Error loading types from assembly {file}: {ex.Message}");
        //            // Handle the error
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error processing assembly {file}: {ex.Message}");
        //            // Handle the error
        //        }
        //    }

        //    return derivedTypes;

        //}
    }
}
