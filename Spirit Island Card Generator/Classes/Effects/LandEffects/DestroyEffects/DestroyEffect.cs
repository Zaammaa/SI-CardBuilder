using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    internal abstract class DestroyEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }
        [AmountValue]
        public int destroyAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]   
        {
            new DifficultyOption("Change amounts", 100, IncreaseAmount, DecreaseAmount),
        };

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
                return new Regex(@"Destroy (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }

        public override string Print()
        {
            return $"Destroy {destroyAmount} " + "{"+Piece.ToString().ToLower() + "}";
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
