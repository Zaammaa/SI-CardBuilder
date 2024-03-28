using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    public abstract class AddEffect : AmountEffect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }
        [AmountValue]
        public int addAmount = 1;

        public override Regex descriptionRegex
        {
            get
            {
                List<string> pieceNames = new List<string>();
                foreach (Piece piece in Enum.GetValues(typeof(Piece)))
                {
                    pieceNames.Add(piece.ToString());
                }
                string allPiecesRegex = "(?:" + String.Join("|", pieceNames) + ")";
                return new Regex(@"Add (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }
        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

        public override string Print()
        {
            string text = $"Add {addAmount} " + "{" + Piece.ToString().ToLower() + "}";
            text = text.Replace("destroyedpresence", "destroyed-presence");
            return text;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }
    }
}
