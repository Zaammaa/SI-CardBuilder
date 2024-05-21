using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
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
    internal class BringAPieceEffect : AmountMultipleEffectOption, IParentEffect
    {
        public override List<ElementSet.Element> WeaklyAssociatedElements => new List<ElementSet.Element>() { ElementSet.Element.Air };
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity
        {
            get
            {
                int complexity = 1;
                complexity += GetChildren().Sum((effect) => effect.Complexity);
                return complexity;
            }
        }

        public PushEffect? basePushEffect;

        [AmountValue]
        public int bringAmount = 1;

        public GamePieces.Piece pieceToBring;

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

        public override double effectStrength => basePushEffect.effectStrength;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1, 1 },
            {2, 0.8 },
            {3, 0.6 }
        };

        protected override List<EffectOption> EffectOptions {
            get
            {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(10, [Piece.Explorer], "{explorer}", 1, 0.5, 1),
                        new EffectOption(10, [Piece.Town], "{town}", 1, 0.65, 1),
                        new EffectOption(2, [Piece.City], "{city}", 1, 1.2, 1),
                        new EffectOption(10, [Piece.Dahan], "{dahan}", 1, 0.2, 1),
                        new EffectOption(10, [Piece.Explorer, Piece.Town], "{explorer}/{town}", 1, 0.7, 1),
                        new EffectOption(3, [Piece.Town, Piece.City], "{town}/{city}", 1, 1.5, 1),
                        new EffectOption(2, [Piece.Explorer, Piece.Town, Piece.City, Piece.Invader], "Invader", 1, 1.55, 1),
                        new EffectOption(1, [Piece.Invader], "Damaged Inavder", 1, 1, 3),
                        new EffectOption(1, [Piece.Invader], "Inavder with {strife}", 1, 0.1, 4),
                        new EffectOption(8, [Piece.Presence], "{presence}", 1, 0.3, 1),
                        new EffectOption(3, [Piece.Blight], "{blight}", 1, 0.3, 1),
                        new EffectOption(5, [Piece.Beast], "{beast}", 1, 0.1, 1),
                        new EffectOption(3, [Piece.Badland], "{badland}", 1, 0.2, 4),
                        new EffectOption(3, [Piece.Wilds], "{wilds}", 1, 0.15, 4),
                        new EffectOption(3, [Piece.Disease], "{disease}", 1, 0.2, 4),
                        new EffectOption(1, [Piece.SacredSite], "{sacred-site}", 1, 0.5, 4),
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

        public override int PrintOrder()
        {
            return 7;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            base.InitializeEffect();

            basePushEffect = (PushEffect) Context.effectGenerator.ChooseGeneratorOption<PushEffect>(UpdateContext());
            basePushEffect.InitializeEffect(UpdateContext());
        }

        public override string Print()
        {
            return $"{basePushEffect.Print()}, bringing {bringAmount} {chosenOption.text}";
        }

        public override bool Scan(string description)
        {
            return false;
        }

        public override Effect Duplicate()
        {
            BringAPieceEffect dupEffect = new BringAPieceEffect();
            dupEffect.Context = Context.Duplicate();
            dupEffect.basePushEffect = (PushEffect)basePushEffect.Duplicate();
            dupEffect.bringAmount = bringAmount;
            dupEffect.chosenOption = chosenOption;
            dupEffect.EffectOptions = new List<EffectOption>(EffectOptions);
            return dupEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return new List<Effect>() { basePushEffect };
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            basePushEffect = (PushEffect)newEffect;
        }
    }
}
