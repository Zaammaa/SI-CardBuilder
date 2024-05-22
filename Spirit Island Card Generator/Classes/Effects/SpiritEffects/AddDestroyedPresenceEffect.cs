using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class AddDestroyedPresenceEffect : AmountMultipleEffectOption
    {
        public override List<ElementSet.Element> WeaklyAssociatedElements => new List<ElementSet.Element>() { ElementSet.Element.Air };
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity => 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amount", 30, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Change piece to come along", 70, ChooseBetterOption, ChooseWorseOption),
        };

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex("", RegexOptions.IgnoreCase);
            }
        }

        //Base strength of adding a destroyed presence, + the modifier for the way it is added
        public override double effectStrength => 0.5 + chosenOption.baseStrength;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 1.2 },
            {3, 1 }
        };

        protected override List<EffectOption> EffectOptions
        {
            get
            {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(15, [Piece.DestroyedPresence], "one of your lands", 1, 0.1, 1),
                        new EffectOption(15, [Piece.DestroyedPresence], "one of their lands", 1, 0.05, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land where you both have {presence}", 1, -0.2, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-1} from your {presence}", 1, 0.15, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-2} from your {presence}", 1, 0.2, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-1} from their {presence}", 1, 0.1, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-2} from their {presence}", 1, 0.15, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-2} from their {presence}", 1, 0.15, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land with your {sacred-site}", 1, -0.5, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-1} of your {sacred-site}", 1, 0, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-2} of your {sacred-site}", 1, 0.1, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-1} of their {sacred-site}", 1, -0.05, 1),
                        new EffectOption(5, [Piece.DestroyedPresence], "a land within {range-2} of their {sacred-site}", 1, 0.05, 1),

                        new EffectOption(3, [Piece.DestroyedPresence], "anywhere on the island", 1, 0.3, 3),
                        new EffectOption(2, [Piece.DestroyedPresence], "a land with 2 or more {dahan} on a board they have {presence}", 1, 0, 2),
                        new EffectOption(2, [Piece.DestroyedPresence], "a land with a {city} on a board they have {presence}", 1, -0.1, 2),
                        new EffectOption(2, [Piece.DestroyedPresence], "a land with a {beast} on a board they have {presence}", 1, 0, 2),
                        new EffectOption(2, [Piece.DestroyedPresence], "a land with {no-blight} on a board they have {presence}", 1, 0.05, 2),
                        new EffectOption(2, [Piece.DestroyedPresence], "a land with no Invaders on a board they have {presence}", 1, 0.08, 2),
                    ];
                }

                return _effectOptions;
            }
            set
            {
                _effectOptions = value;
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            return $"{Context.GetTargetString(TargetType)} may Add {effectAmount} " + "{destroyed-presence}" + $" to {chosenOption.text}";
        }

        public override bool Scan(string description)
        {
            return false;
        }

        public override Effect Duplicate()
        {
            AddDestroyedPresenceEffect dupEffect = new AddDestroyedPresenceEffect();
            dupEffect.Context = Context.Duplicate();
            dupEffect.chosenOption = chosenOption;
            dupEffect.effectAmount = effectAmount;
            dupEffect.EffectOptions = new List<EffectOption>(EffectOptions);
            return dupEffect;
        }
    }
}
