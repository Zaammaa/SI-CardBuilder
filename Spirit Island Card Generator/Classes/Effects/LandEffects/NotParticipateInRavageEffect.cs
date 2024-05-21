using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    [CustomEffect(1)]
    internal class NotParticipateInRavageEffect : AmountMultipleEffectOption
    {
        public override double BaseProbability => 0.03;
        public override int Complexity => 1;
        public override Regex descriptionRegex => new Regex("");

        protected override List<EffectOption> EffectOptions { 
            get {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(10, [Piece.Explorer], "{explorer}", 1, 0.15, 1),
                        new EffectOption(10, [Piece.Town], "{town}", 1, 0.3, 1),
                        new EffectOption(7, [Piece.City], "{city}", 1, 0.5, 1),
                        new EffectOption(10, [Piece.Dahan], "{dahan}", 1, -0.1, 1),
                        new EffectOption(10, [Piece.Explorer, Piece.Town], "{explorer}/{town}", 1, 0.45, 1),
                        new EffectOption(7, [Piece.Town, Piece.City], "{town}/{city}", 1, 0.6, 1),
                        new EffectOption(10, [Piece.Explorer, Piece.Town, Piece.City, Piece.Invader], "Invader", 1, 0.65, 1),
                        new EffectOption(5, [Piece.Invader], "Damaged Inavder", 1, 0.2, 3),
                        new EffectOption(1, [Piece.Invader], "Inavder with {strife}", 1, 0.1, 4),
                    ];
                }

                return _effectOptions;
            } 
            set { 
                _effectOptions = value;
            }
        }

        

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change amounts", 80, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Change pieces", 10, ChooseBetterOption, ChooseWorseOption),
        ];

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2 },
            {3, 1.2 },
            {4, 2 }
        };

        public override IPowerLevel Duplicate()
        {
            NotParticipateInRavageEffect effect = new NotParticipateInRavageEffect();
            effect.Context = Context.Duplicate();
            effect.effectAmount = effectAmount;
            effect.chosenOption = new EffectOption(chosenOption.weight, chosenOption.pieces, chosenOption.text, chosenOption.powerMult, chosenOption.baseStrength, chosenOption.customLvl);
            effect.EffectOptions = new List<EffectOption>(EffectOptions);
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders) || !context.card.Fast)
                return false;

            return true;
        }

        public override string Print()
        {
            string amountText = effectAmount == 4 ? "all" : effectAmount.ToString();
            string text = chosenOption.text;
            if (effectAmount > 1 && text.Contains("Invader"))
                text = text.Replace("Invader", "Invaders");


            if (effectAmount > 1)
            {
                return $"{amountText} {chosenOption.text} don't participate in Ravage";
            }
            return $"{amountText} {chosenOption.text} doesn't participate in Ravage";
        }
    }
}
