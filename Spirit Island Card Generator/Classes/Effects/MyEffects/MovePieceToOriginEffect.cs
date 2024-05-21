using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.MyEffects
{
    [LandEffect]
    [CustomEffect(3)]
    internal class MovePieceToOriginEffect : AmountMultipleEffectOption
    {
        public override double BaseProbability => 0.02;
        public override int Complexity => 2;
        public override Regex descriptionRegex => new Regex("");

        protected override List<EffectOption> EffectOptions
        {
            get
            {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(10, [Piece.Explorer], "{explorer}", 1, 0.6),
                        new EffectOption(10, [Piece.Town], "{town}", 1, 0.7),
                        new EffectOption(2, [Piece.City], "{city}", 1, 1.5, 4),
                        new EffectOption(10, [Piece.Dahan], "{dahan}", 1, 0.35),
                        new EffectOption(10, [Piece.Explorer, Piece.Town], "{explorer}/{town}", 1, 0.8),
                        new EffectOption(4, [Piece.Beast], "{beast}", 1, 0.1, 4),
                        new EffectOption(1, [Piece.Wilds], "{wilds}", 1, 0.1, 4),
                        new EffectOption(1, [Piece.Badland], "{badlands}", 1, 0.1, 4),
                        new EffectOption(1, [Piece.Disease], "{disease}", 1, 0.1, 4),
                        new EffectOption(3, [Piece.Blight], "{blight}", 1, 0.2),
                    ];
                }

                return _effectOptions;
            }
            set
            {
                _effectOptions = value;
            }
        }



        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change amounts", 80, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Change pieces", 10, ChooseBetterOption, ChooseWorseOption),
        ];

        public override double CalculatePowerLevel()
        {
            double basePower = base.CalculatePowerLevel();
            if (Context.card.Range.range == 1)
                return basePower;
            else if (Context.card.Range.range == 2)
            {
                return basePower + 0.05;
            } else
            {
                return basePower + 0.1;
            }
        }

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2 },
            {3, 1.2 },
            {4, 2 }
        };

        public override IPowerLevel Duplicate()
        {
            MovePieceToOriginEffect effect = new MovePieceToOriginEffect();
            effect.Context = Context.Duplicate();
            effect.effectAmount = effectAmount;
            effect.chosenOption = new EffectOption(chosenOption.weight, chosenOption.pieces, chosenOption.text, chosenOption.powerMult, chosenOption.baseStrength, chosenOption.customLvl);
            effect.EffectOptions = new List<EffectOption>(EffectOptions);
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Range.range == 0)
                return false;

            return true;
        }

        public override string Print()
        {
            string amountText = effectAmount == 4 ? "all" : effectAmount.ToString();
            return $"Move {amountText} {chosenOption.text} to the origin land";
        }
    }
}
