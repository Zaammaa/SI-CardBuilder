using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.PowerCardInstructions;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class ReducedDamageEffect : Effect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Water }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun, Element.Animal }; } }
        public override double BaseProbability => 0.02;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"-1 Damage", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => 
        [
            new DifficultyOption("Change invader types", 40, ChooseBetterModifier, ChooseWorseModifier),
        ];

        protected ModifierOptions chosenOption { get; set; }

        protected struct ModifierOptions
        {
            public int weight;
            public Piece[] pieces;
            public string text;
            public double strength;
            public int customLevel;

            public ModifierOptions(int w, Piece[] pieces, string t, double strength, int customLevel = 0)
            {
                weight = w;
                this.pieces = pieces;
                this.text = t;
                this.strength = strength;
            }
        }

        protected List<ModifierOptions> instructionModifiers = new List<ModifierOptions>() {
            new ModifierOptions(100, [Piece.Invader], "Each Invader does -1 Damage", 0.85, 0),
            new ModifierOptions(2, [Piece.Explorer], "Each {explorer} does -1 Damage", 0.2, 3),
            new ModifierOptions(5, [Piece.Explorer, Piece.Town], "Each {explorer}/{town} does -1 Damage", 0.75, 2),
            new ModifierOptions(5, [Piece.Town, Piece.City], "Each {town}/{city} does -1 Damage", 0.5, 2),
            new ModifierOptions(5, [Piece.Dahan], "Each {dahan} does -1 Damage", -0.2, 4),
        };


        public override double CalculatePowerLevel()
        {
            return chosenOption.strength;
        }

        public override IPowerLevel Duplicate()
        {
            ReducedDamageEffect effect = new ReducedDamageEffect();
            effect.chosenOption = new ModifierOptions(chosenOption.weight, chosenOption.pieces, chosenOption.text, chosenOption.strength, chosenOption.customLevel);
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders))
            {
                return false;
            }
            return true;
        }

        public override string Print()
        {
            return chosenOption.text;
        }

        public override bool Scan(string description)
        {
            return true;
        }

        protected override void InitializeEffect()
        {
            List<Piece> inValidPieces = LandConditon.ConditionToInvalidPieces(Context.target.landConditions).ToList();
            foreach(Piece piece in  inValidPieces)
            {
                if (inValidPieces.Contains(piece))
                    instructionModifiers.RemoveAll((mod) => mod.pieces.Contains(piece));
            }

            instructionModifiers.RemoveAll((mod) => mod.customLevel > Context.settings.CustomEffectLevel);
            Dictionary<ModifierOptions, int> weights = new Dictionary<ModifierOptions, int>();
            foreach (ModifierOptions option in instructionModifiers)
            {
                weights.Add(option, option.weight);
            }
            chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
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

            ReducedDamageEffect strongerEffect = (ReducedDamageEffect)Duplicate();
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

            ReducedDamageEffect weakerEffect = (ReducedDamageEffect)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
            return weakerEffect;
        }
    }
}
