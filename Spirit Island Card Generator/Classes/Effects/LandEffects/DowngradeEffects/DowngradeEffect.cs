using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DowngradeEffects
{
    [LandEffect]
    [CustomEffect(1)]
    internal class DowngradeEffect : AmountEffect
    {
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Moon, Element.Water }; } }
        public override double BaseProbability => 0.07;
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        protected DowngradeOption chosenOption { get; set; }

        [AmountValue]
        public int downgradeAmount = 1;

        protected struct DowngradeOption
        {
            public int weight;
            public string pieces;
            public double strength;

            public DowngradeOption(int w, string pieces, double strength)
            {
                weight = w;
                this.pieces = pieces; 
                this.strength = strength;
            }
        }

        protected List<DowngradeOption> downGradeOptions = new List<DowngradeOption>() { 
            new DowngradeOption(10, "Invader", 1),
            new DowngradeOption(30, "{explorer}/{town}", 0.8),
            new DowngradeOption(30, "{town}/{city}", 0.7),
            new DowngradeOption(10, "{town}", 0.5),
            new DowngradeOption(3, "{city}", 0.6),
            new DowngradeOption(3, "{explorer}/{city}", 0.85),
        };

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 30, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Change amounts", 70, ChooseBetterPieces, ChooseWorsePieces),
        };

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Downgrade", RegexOptions.IgnoreCase);
            }
        }

        public override double effectStrength => chosenOption.strength;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.2},
            { 3, 1.2},
        };



        public override int Complexity => 1;

        public override string Print()
        {
            return $"Downgrade {downgradeAmount} " + $"{chosenOption.pieces}.";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoBuildings))
                return false;
            return true;
        }

        protected override void InitializeEffect()
        {
            Dictionary<DowngradeOption, int> weights = new Dictionary<DowngradeOption, int>();
            foreach (DowngradeOption option in downGradeOptions)
            {
                if (option.strength > chosenOption.strength)
                {
                    weights.Add(option, (int)(option.strength * 1000));
                }
            }
            chosenOption = Utils.ChooseWeightedOption<DowngradeOption>(weights, Context.rng);
        }

        public override IPowerLevel Duplicate()
        {
            DowngradeEffect effect = new DowngradeEffect();
            effect.Context = Context;
            effect.downgradeAmount = downgradeAmount;
            effect.chosenOption = new DowngradeOption(chosenOption.weight, chosenOption.pieces, chosenOption.strength);
            return effect;
        }

        public Effect? ChooseBetterPieces()
        {
            Dictionary<DowngradeOption, int> weights = new Dictionary<DowngradeOption, int>();
            foreach (DowngradeOption option in downGradeOptions)
            {
                if (option.strength > chosenOption.strength)
                {
                    weights.Add(option, (int)(option.strength * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            DowngradeEffect strongerEffect = (DowngradeEffect)Duplicate();
            strongerEffect.chosenOption = Utils.ChooseWeightedOption<DowngradeOption>(weights, Context.rng);
            return strongerEffect;
        }

        public Effect? ChooseWorsePieces()
        {
            Dictionary<DowngradeOption, int> weights = new Dictionary<DowngradeOption, int>();
            foreach (DowngradeOption option in downGradeOptions)
            {
                if (option.strength < chosenOption.strength)
                {
                    weights.Add(option, (int)(option.strength * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            DowngradeEffect weakerEffect = (DowngradeEffect)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption<DowngradeOption>(weights, Context.rng);
            return weakerEffect;
        }
    }
}
