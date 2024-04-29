using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DowngradeEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.PowerCardInstructions;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class RepeatAPowerCardEffect : Effect
    {
        //This is the power of repeating a 0 cost card with no modifiers
        protected static double BASE_POWER = 0.7;
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Sun }; } }
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 3; } }

        protected ModifierOptions chosenOption { get; set; }

        protected struct ModifierOptions
        {
            public int weight;
            public PowerCardInstruction modifier;
            public double strength;
            public int customLevel;

            public ModifierOptions(int w, PowerCardInstruction m, double strength, int customLevel = 2)
            {
                weight = w;
                this.modifier = m;
                this.strength = strength;
            }
        }

        protected List<ModifierOptions> instructionModifiers = new List<ModifierOptions>() {
            new ModifierOptions(200, PowerCardInstruction.ANY, 1.0, 0),
            new ModifierOptions(10, PowerCardInstruction.PushOrGather, 0.7),
            new ModifierOptions(3, PowerCardInstruction.Push, 0.55),
            new ModifierOptions(3, PowerCardInstruction.Gather, 0.6),
            new ModifierOptions(3, PowerCardInstruction.Fear, 0.7),
            new ModifierOptions(5, PowerCardInstruction.Defend, 0.7),
            new ModifierOptions(10, PowerCardInstruction.Slow, 0.85),
            new ModifierOptions(10, PowerCardInstruction.Fast, 0.85),
            new ModifierOptions(10, PowerCardInstruction.Damage, 0.9),
            new ModifierOptions(10, PowerCardInstruction.SacredSite, 0.8),
        };

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Repeat a Power Card", RegexOptions.IgnoreCase);
            }
        }

        //public PowerCardInstructions.PowerCardInstruction cardModifiers;
        public bool byPayingItsCost = true;
        public int maxCost = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change card modifiers", 40, ChooseBetterModifier, ChooseWorseModifier),
            new DifficultyOption("Change max cost", 50, RaiseMaxCost, LowerMaxCost),
            new DifficultyOption("require/remove paying its cost", 10, DontRequirePaying, RequirePaying),
        };

        //Writes what goes on the card
        public override string Print()
        {
            string energyCostText;
            if (maxCost == 0)
            {
                energyCostText = $" with Energy cost of 0";
            } else if (maxCost > 0)
            {
                energyCostText = $" with Energy cost of {maxCost} or less";
            } else
            {
                energyCostText = "";
            }
            if (maxCost > 0)
            {
                string payingItsCostText = byPayingItsCost ? " by paying its cost" : "";
                return $"{Context.GetTargetString(TargetType)} may Repeat a Power Card{energyCostText} {PowerCardInstructions.InstructionConversions[chosenOption.modifier]}{payingItsCostText}";
            } else
            {
                return $"{Context.GetTargetString(TargetType)} may Repeat a 0 cost Power Card {PowerCardInstructions.InstructionConversions[chosenOption.modifier]}";
            }
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;

            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            instructionModifiers.RemoveAll((modOption) => modOption.customLevel > Context.settings.CustomEffectLevel);
            Dictionary<ModifierOptions, int> weights = new Dictionary<ModifierOptions, int>();
            foreach(ModifierOptions option in instructionModifiers)
            {
                weights.Add(option, option.weight);
            }
            chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
            byPayingItsCost = Context.rng.Next(0, 100) < 75;
            maxCost = Context.rng.Next(0, 3);
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            if (byPayingItsCost)
            {
                return BASE_POWER * chosenOption.strength + (maxCost * 0.1);
            } else
            {
                return BASE_POWER * chosenOption.strength + ((maxCost) * 0.5);
            }
        }

        protected Effect? ChooseBetterModifier()
        {
            Dictionary<ModifierOptions, int> weights = new Dictionary<ModifierOptions, int>();
            foreach (ModifierOptions option in instructionModifiers)
            {
                if (option.strength > chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            RepeatAPowerCardEffect strongerEffect = (RepeatAPowerCardEffect)Duplicate();
            strongerEffect.chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
            return strongerEffect;
        }

        protected Effect? ChooseWorseModifier()
        {
            Dictionary<ModifierOptions, int> weights = new Dictionary<ModifierOptions, int>();
            foreach (ModifierOptions option in instructionModifiers)
            {
                if (option.strength < chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            RepeatAPowerCardEffect weakerEffect = (RepeatAPowerCardEffect)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
            return weakerEffect;
        }

        protected Effect? RaiseMaxCost()
        {
            if (maxCost <= 2)
            {
                RepeatAPowerCardEffect strongerEffect = (RepeatAPowerCardEffect)Duplicate();
                strongerEffect.maxCost += 1;
                return strongerEffect;
            }
            return null;
        }

        protected Effect? LowerMaxCost()
        {
            if (maxCost > 0)
            {
                RepeatAPowerCardEffect weakerEffect = (RepeatAPowerCardEffect)Duplicate();
                weakerEffect.maxCost -= 1;
                return weakerEffect;
            }
            return null;
        }

        protected Effect? DontRequirePaying()
        {
            if (byPayingItsCost)
            {
                RepeatAPowerCardEffect strongerEffect = (RepeatAPowerCardEffect)Duplicate();
                strongerEffect.byPayingItsCost = false;
                return strongerEffect;
            }
            return null;
        }

        protected Effect? RequirePaying()
        {
            if (!byPayingItsCost)
            {
                RepeatAPowerCardEffect weakerEffect = (RepeatAPowerCardEffect)Duplicate();
                weakerEffect.byPayingItsCost = true;
                return weakerEffect;
            }
            return null;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            RepeatAPowerCardEffect effect = new RepeatAPowerCardEffect();
            effect.chosenOption = new ModifierOptions(chosenOption.weight, chosenOption.modifier, chosenOption.strength, chosenOption.customLevel);
            effect.byPayingItsCost = byPayingItsCost;
            effect.maxCost = maxCost;
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
