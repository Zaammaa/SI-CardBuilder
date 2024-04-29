using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DowngradeEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.LandStateConditions
{
    [LandCondition]
    internal class NumberOfPiecesCondition : Condition
    {
        public override double BaseProbability => 0.15;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        protected struct PieceOptions
        {
            public Piece piece;
            public int Weight;
            public double multiplier;
            public string text;
            public int customLevel;

            public PieceOptions(Piece piece, int weight, double multiplier, string text, int customLevel = 2)
            {
                this.piece = piece;
                this.Weight = weight;
                this.multiplier = multiplier;
                this.text   = text;
                this.customLevel = customLevel;
            }
        }
        protected PieceOptions chosenOption;

        protected List<PieceOptions> options = new List<PieceOptions>()
        {
            new PieceOptions(Piece.Dahan, 30, 0.5, "If there are at least 2 {dahan}"),
            new PieceOptions(Piece.Dahan, 30, 0.4, "If there are at least 3 {dahan}", 3),
            new PieceOptions(Piece.Dahan, 15, 0.3, "If there are at least 4 {dahan}", 4),

            new PieceOptions(Piece.Beast, 10, 0.35, "If there are at least 2 {beast}"),
            new PieceOptions(Piece.Beast, 5, 0.2, "If there are at least 3 {beast}", 4),

            new PieceOptions(Piece.City, 10, 0.3, "If there are at least 2 {city}", 3),
            new PieceOptions(Piece.City, 5, 0.1, "If there are at least 3 {city}", 4),
            new PieceOptions(Piece.Town, 5, 0.4, "If there are at least 2 {town}", 3),
            new PieceOptions(Piece.Town, 5, 0.2, "If there are at least 3 {town}", 4),
            new PieceOptions(Piece.Explorer, 20, 0.6, "If there are at least 2 {explorer}"),
            new PieceOptions(Piece.Explorer, 10, 0.3, "If there are at least 3 {explorer}", 4),

            new PieceOptions(Piece.Presence, 10, 0.4, "If there are {presence} from at least 2 different Spirits", 3),

            new PieceOptions(Piece.Blight, 20, 0.4, "If there is at least 2 {blight}", 3),
            new PieceOptions(Piece.Blight, 10, 0.25, "If there is at least 3 {blight}", 4),

            new PieceOptions(Piece.Invader, 20, 0.5, "If there is exactly 1 Invader"),

            new PieceOptions(Piece.Invader, 20, 0.8, "If there are 2 or more Invaders"),
            new PieceOptions(Piece.Invader, 10, 0.6, "If there are 3 or more Invaders"),
            new PieceOptions(Piece.Invader, 5, 0.5, "If there are 4 or more Invaders", 4),

            new PieceOptions(Piece.NoPiece, 20, 0.4, "If the land has no pieces", 3),

            //new PieceOptions(Piece.Vitality, 10, 0.4, "If the land has no pieces", 4),


        };
        
        public List<LandConditions> Implications {
            get
            {
                return LandConditon.PieceImplications[chosenOption.piece];
            }
        }

        public override double DifficultyMultiplier
        {
            get
            {
                return chosenOption.multiplier;
            }
        }

        public override string ConditionText => chosenOption.text;

        public override bool ChooseEasierCondition(Context context)
        {
            Dictionary<PieceOptions, int> weights = new Dictionary<PieceOptions, int>();
            foreach (PieceOptions option in options)
            {
                if (option.multiplier > chosenOption.multiplier)
                {
                    weights.Add(option, (int)(option.Weight * 1000));
                }
            }
            if (weights.Count == 0)
                return false;

            chosenOption = Utils.ChooseWeightedOption<PieceOptions>(weights, context.rng);
            return true;
        }

        public override bool ChooseHarderCondition(Context context)
        {
            Dictionary<PieceOptions, int> weights = new Dictionary<PieceOptions, int>();
            foreach (PieceOptions option in options)
            {
                if (option.multiplier < chosenOption.multiplier)
                {
                    weights.Add(option, (int)(option.Weight * 1000));
                }
            }
            if (weights.Count == 0)
                return false;

            chosenOption = Utils.ChooseWeightedOption<PieceOptions>(weights, context.rng);
            return true;
        }

        public override IPowerLevel Duplicate()
        {
            NumberOfPiecesCondition condition = new NumberOfPiecesCondition();
            condition.chosenOption = new PieceOptions(chosenOption.piece, chosenOption.Weight, chosenOption.multiplier, chosenOption.text, chosenOption.customLevel);
            condition.options = new List<PieceOptions>(options);
            return condition;
        }

        public override void Initialize(Context context)
        {
            options.RemoveAll((option) => option.customLevel > context.settings.CustomEffectLevel);
            foreach (LandConditions condition in context.target.landConditions)
            {
                if (condition == LandConditions.Noblight)
                {
                    options.RemoveAll((option) => option.piece == Piece.Blight);
                }
                if (condition == LandConditions.NoBuildings)
                {
                    options.RemoveAll((option) => option.piece == Piece.Town);
                    options.RemoveAll((option) => option.piece == Piece.City);
                }
                if (condition == LandConditions.NoCity)
                {
                    options.RemoveAll((option) => option.piece == Piece.City);
                }
                if (condition == LandConditions.NoDahan)
                {
                    options.RemoveAll((option) => option.piece == Piece.Dahan);
                }
                if (condition == LandConditions.NoInvaders)
                {
                    options.RemoveAll((option) => option.piece == Piece.Town);
                    options.RemoveAll((option) => option.piece == Piece.City);
                    options.RemoveAll((option) => option.piece == Piece.Explorer);
                }
            }

            Dictionary<PieceOptions, int> weights = new Dictionary<PieceOptions, int>();
            foreach (PieceOptions option in options)
            {
                weights.Add(option, (int)(option.Weight * 1000));
            }
            chosenOption = Utils.ChooseWeightedOption<PieceOptions>(weights, context.rng);
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}
