using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.TransformingEffects
{
    internal abstract class TransformationEffect : Effect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece FromPiece { get; }
        public abstract Piece ToPiece { get; }

        protected struct PieceStrengths
        {
            Piece piece;
            double strength;
            public PieceStrengths(Piece p, double s)
            {
                piece = p;
                strength = s;
            }
        }

        protected Dictionary<Piece, double> strengths = new Dictionary<Piece, double>() {
            {Piece.City, -2},
            {Piece.Town, -1},
            {Piece.Explorer, -0.7},

            {Piece.Dahan, 0.8},
            {Piece.Beast, 0.6},
            {Piece.Blight, -1.5},
            {Piece.Presence, 0.8},
            {Piece.DestroyedPresence, 0.8},
            {Piece.Disease, 0.9},
            {Piece.Vitality, 1},
            {Piece.Wilds, 0.75},
            {Piece.Badland, 0.7},
        };

        public override double CalculatePowerLevel()
        {
            //Replacing is slightly less powerful than the difference between the two pieces since the target land needs the from piece
            if (strengths[ToPiece] > strengths[FromPiece])
            {
                return Math.Max(strengths[ToPiece] - strengths[FromPiece] - 0.15, 0);
            } else
            {
                //If the effect is bad enough, players will just avoid a space with the from piece. So cap how bad it can be
                return Math.Max(strengths[ToPiece] - strengths[FromPiece], -0.2);
            }
            
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            
        };

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Replace", RegexOptions.IgnoreCase);
            }
        }

        public override string Print()
        {
            return $"Replace a " + "{" +  FromPiece.ToString().ToLower() + "} with a {" + ToPiece.ToString().ToLower() + "}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        //public Effect? ChooseStrongerTransformation
    }
}
