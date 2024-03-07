using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    public abstract class Effect : GeneratorOption, IPowerPowerLevel
    {
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }

        public abstract Regex descriptionRegex { get; }

        //Whether the effect can be skipped. If it can, it should say "may"
        public bool optional = false;
        //Writes what goes on the card
        public abstract string Print();
        //Initializes effect from a card description.
        public abstract bool Scan(string description);
        //Checks if this should be an option for the card generator
        public abstract bool IsValid(Card card, Settings settings);
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public abstract void InitializeEffect(Card card, Settings settings);
        //Estimates the effects own power level
        public abstract double CalculatePowerLevel();

        public abstract IPowerPowerLevel Duplicate();
        //If false, the card must have another effect.
        public virtual bool Standalone { get { return true; } }

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public abstract Effect? Strengthen(Card card, Settings settings);
        /// <summary>
        /// Similar to Strenthen, but makes the effect weaker instead
        /// </summary>
        /// <param name="card"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract Effect? Weaken(Card card, Settings settings);
        /// <summary>
        /// Utility function to pick a random generatorOption from a list of possible options
        /// </summary>
        /// <param name="card"></param>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GeneratorOption? ChooseEffect(Card card, Settings settings, IEnumerable<GeneratorOption> options)
        {
            double totalWeight = 0;
            List<GeneratorOption> validOptions = new List<GeneratorOption>();
            foreach (GeneratorOption option in options)
            {
                if (!card.ContainsEffect(option.GetType()) && option.IsValid(card, settings))
                {
                    totalWeight += option.AdjustedProbability;
                    validOptions.Add(option);
                }
            }

            double choice = settings.rng.NextDouble() * totalWeight;
            double index = 0;
            foreach (GeneratorOption option in validOptions)
            {
                if (choice < index + option.AdjustedProbability)
                {
                    return option;
                }
                index += option.AdjustedProbability;
            }
            return default(GeneratorOption);
        }

        //Choose an effect and upgrade it until it is stronger than a given amount, or until it cannot be made stronger. If the later happens, return null
        public static Effect? ChooseStrongerEffect(Card card, Settings settings, double minPower, IEnumerable<Effect> options) {
            Effect? option = (Effect?)ChooseEffect(card, settings, options);
            while (option != null && option.CalculatePowerLevel() < minPower) { 
                option = option.Strengthen(card, settings);
            }
            return option;
        }
        //Choose an effect and downgrade it until it is weaker than a given amount, or until it cannot be made weaker. If the later happens, return null
        public static Effect? ChooseWeakerEffect(Card card, Settings settings, double maxPower, IEnumerable<Effect> options)
        {
            Effect? option = (Effect?)ChooseEffect(card, settings, options);
            while (option != null && option.CalculatePowerLevel() > maxPower)
            {
                option = option.Weaken(card, settings);
            }
            return option;
        }


    }
}
