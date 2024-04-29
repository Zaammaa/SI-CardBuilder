using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions
{
    [CostCondition]
    [LandCondition]
    [CustomEffect(3)]
    internal class DestroyAPieceCondition : Condition
    {
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override double DifficultyMultiplier
        {
            get
            {
                return pieceOptions[piece].multiplier;
            }
        }
        protected bool isFast = false;

        struct WeightAndMultiplier
        {
            public double multiplier;
            public int weight;

            public WeightAndMultiplier(double m, int w)
            {
                multiplier = m;
                weight = w;
            }
        }

        Dictionary<GamePieces.Piece, WeightAndMultiplier> pieceOptions = new Dictionary<GamePieces.Piece, WeightAndMultiplier>()
        {
            {GamePieces.Piece.Dahan, new WeightAndMultiplier(0.5, 50) },
            {GamePieces.Piece.Beast, new WeightAndMultiplier(0.55, 20) },
            {GamePieces.Piece.Blight, new WeightAndMultiplier(0.9, 10) }, //Needs special text
            {GamePieces.Piece.Presence, new WeightAndMultiplier(0.45, 20) }, //Needs special text
            //I'm going to treat this one as Disease/Wilds/Strife/Badlands
            {GamePieces.Piece.Disease, new WeightAndMultiplier(0.55, 20) }, //Needs special text

        };

        protected GamePieces.Piece piece;
        protected Context Context;

        public override string ConditionText
        {
            get
            {
                if (piece.Equals(GamePieces.Piece.Blight))
                {
                    return "You may destroy a {blight} (it goes back to the box). If you do,";
                } else if (piece.Equals(GamePieces.Piece.Presence) && !Context.target.SpiritTarget)
                {
                    return "A spirit with {presence} in target land may destroy 1 of their {presence}. If they do,";
                } 
                else if (piece.Equals(GamePieces.Piece.Disease))
                {
                    return "Destroy 1 {disease}/{wilds}/{strife}/{badland}. If you do,";
                } else
                {
                    return "Destroy 1 {" + piece.ToString().ToLower() + "}. If you do,";
                }
            }
        }

        public override bool ChooseEasierCondition(Context context)
        {
            Dictionary<GamePieces.Piece, int> weights = new Dictionary<GamePieces.Piece, int>();

            foreach (GamePieces.Piece pieceOption in pieceOptions.Keys)
            {
                if (pieceOptions[pieceOption].multiplier > DifficultyMultiplier && IsValidPiece(pieceOption, context))
                    weights.Add(pieceOption, pieceOptions[pieceOption].weight);
            }
            if (weights.Count == 0)
                return false;

            GamePieces.Piece? newCondition = Utils.ChooseWeightedOption(weights, context.rng);
            if (newCondition.HasValue)
            {
                piece = newCondition.Value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ChooseHarderCondition(Context context)
        {
            Dictionary<GamePieces.Piece, int> weights = new Dictionary<GamePieces.Piece, int>();

            foreach (GamePieces.Piece pieceOption in pieceOptions.Keys)
            {
                if (pieceOptions[pieceOption].multiplier < DifficultyMultiplier)
                    weights.Add(pieceOption, pieceOptions[pieceOption].weight);
            }
            if (weights.Count == 0)
                return false;

            GamePieces.Piece? newCondition = Utils.ChooseWeightedOption(weights, context.rng);
            if (newCondition.HasValue)
            {
                piece = newCondition.Value;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsValidPiece(GamePieces.Piece piece, Context context)
        {
            DestroyAPieceCondition? conditionToReplace = null;
            if (context.conditions.LastOrDefault() == this)
                conditionToReplace = this;

            DestroyAPieceCondition newCondition = (DestroyAPieceCondition)Duplicate();
            newCondition.piece = piece;
            return IsValidForChildren(context, newCondition, conditionToReplace);
        }

        public override IPowerLevel Duplicate()
        {
            DestroyAPieceCondition condition = new DestroyAPieceCondition();
            condition.Context = Context.Duplicate();
            condition.piece = piece;
            return condition;
        }

        public override void Initialize(Context context)
        {
            Dictionary<GamePieces.Piece, int> weights = new Dictionary<GamePieces.Piece, int>();
            //TODO: Need to make sure the piece isn't excluded by the target type. For example: The piece shouldn't be a dahan if the target says noDahan

            foreach (GamePieces.Piece pieceOption in pieceOptions.Keys)
            {
                weights.Add(pieceOption, pieceOptions[pieceOption].weight);
            }

            piece = Utils.ChooseWeightedOption(weights, context.rng);
            Context = context;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.targetType != Target.TargetType.Land)
                return false;
            return true;
        }
    }
}
