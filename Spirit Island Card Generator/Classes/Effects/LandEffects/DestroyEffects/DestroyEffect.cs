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
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    internal abstract class DestroyEffect : Effect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }

        //How much stronger/weaker adding additional pieces is compared to the first
        //This works like tax brackets, so only the later pieces get multiplied by their modifier
        protected abstract Dictionary<int, double> ExtraPiecesMultiplier { get; }
        protected abstract double PieceStrength { get; }
        public int amount = 1;

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

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            for (int i = 1; i <= amount; i++)
            {
                powerLevel += PieceStrength * ExtraPiecesMultiplier[i];
            }
            return powerLevel;
        }

        public override string Print()
        {
            return $"Destroy {amount} " + "{"+Piece+"}.";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public override Effect? Strengthen()
        {
            if (amount < ExtraPiecesMultiplier.Count && PieceStrength > 0)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();

                newEffect.amount += 1;

                return newEffect;
            } else if (amount > 1 && PieceStrength < 0)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();

                newEffect.amount -= 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }

        public override Effect? Weaken()
        {
            if (amount > 1 && PieceStrength > 0)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();
                newEffect.amount -= 1;
                return newEffect;
            } else if (amount < ExtraPiecesMultiplier.Count && PieceStrength < 0)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();

                newEffect.amount += 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }
    }
}
