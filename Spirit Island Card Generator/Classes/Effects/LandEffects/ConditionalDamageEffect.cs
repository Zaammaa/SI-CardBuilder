using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DowngradeEffects;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class ConditionalDamageEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun, Element.Animal }; } }
        public override double effectStrength
        {
            get
            {
                if (Context != null && Context.card.Fast)
                    return (double)damageAmount * 0.8 * chosenOption.strength;
                else
                    return (double)(damageAmount * 0.7 * chosenOption.strength);
            }
        }

        public override double BaseProbability => 0.07;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Damage to ", RegexOptions.IgnoreCase);
            }
        }

        [AmountValue]
        public int damageAmount = 1;
        protected DamageOption chosenOption;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.5 },
            {3, 1.5 },
        };

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change amounts", 30, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Choose different pieces", 70, ChooseBetterPieces, ChooseWorsePieces),
        ];

        protected struct DamageOption
        {
            public int weight;
            public Piece[] pieces;
            public string text;
            public double strength;

            public DamageOption(int w, Piece[] piece, string text, double strength)
            {
                weight = w;
                pieces = piece;
                this.text = text;
                this.strength = strength;
            }
        }

        protected List<DamageOption> damageOptions = new List<DamageOption>() {
            new DamageOption(10, [Piece.Explorer, Piece.Town], "{explorer}/{town}", 0.9),
            new DamageOption(30, [Piece.Town, Piece.City], "{town}/{city}", 0.8),
            new DamageOption(5, [Piece.Town],"{town}", 0.6),
            new DamageOption(3, [Piece.City], "{city}", 0.5),
            new DamageOption(1, [Piece.Explorer, Piece.City], "{explorer}/{city}", 0.8),
        };

        public override IPowerLevel Duplicate()
        {
            ConditionalDamageEffect effect = new ConditionalDamageEffect();
            effect.damageAmount = damageAmount;
            effect.chosenOption = new DamageOption(chosenOption.weight, chosenOption.pieces, chosenOption.text, chosenOption.strength);
            effect.damageOptions = new List<DamageOption>(damageOptions);
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            return $"{damageAmount} Damage to {chosenOption.text}";
        }

        public override bool Scan(string description)
        {
            return true;
        }

        protected override void InitializeEffect()
        {
            List<Piece> inValidPieces = LandConditon.ConditionToInvalidPieces(Context.target.landConditions).ToList();
            foreach (Piece piece in inValidPieces)
            {
                if (inValidPieces.Contains(piece))
                    damageOptions.RemoveAll((mod) => mod.pieces.Contains(piece));
            }

            Dictionary<DamageOption, int> weights = new Dictionary<DamageOption, int>();
            foreach (DamageOption option in damageOptions)
            {
                weights.Add(option, option.weight);
            }
            chosenOption = Utils.ChooseWeightedOption(weights, Context.rng);
            damageAmount = 1;
        }

        public Effect? ChooseBetterPieces()
        {
            Dictionary<DamageOption, int> weights = new Dictionary<DamageOption, int>();
            foreach (DamageOption option in damageOptions)
            {
                if (option.strength > chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            ConditionalDamageEffect strongerEffect = (ConditionalDamageEffect)Duplicate();
            strongerEffect.chosenOption = Utils.ChooseWeightedOption<DamageOption>(weights, Context.rng);
            return strongerEffect;
        }

        public Effect? ChooseWorsePieces()
        {
            Dictionary<DamageOption, int> weights = new Dictionary<DamageOption, int>();
            foreach (DamageOption option in damageOptions)
            {
                if (option.strength < chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            ConditionalDamageEffect weakerEffect = (ConditionalDamageEffect)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption<DamageOption>(weights, Context.rng);
            return weakerEffect;
        }
    }
}
