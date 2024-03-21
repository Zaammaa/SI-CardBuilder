using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    internal abstract class GatherEffect : Effect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }

        //How much stronger/weaker adding additional pieces is compared to the first
        //This works like tax brackets, so only the later pieces get multiplied by their modifier
        protected abstract Dictionary<int, double> ExtraPiecesMultiplier { get; }
        protected abstract double PieceStrength { get; }
        //Must VS. May
        public bool mandatory = false;
        public int amount = 1;

        public override int PrintOrder()
        {
            return 4;
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 80, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Make Optional/Mandatory", 20, MakeOptional, MakeMandatory),
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
                return new Regex(@"Gather (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
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
            if (mandatory)
            {
                powerLevel -= 0.05 * amount;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";

            return $"Gather {mayText}{amount} " + "{" + Piece.ToString().ToLower()+"}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public Effect? IncreaseAmount()
        {
            if (amount < ExtraPiecesMultiplier.Count)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.amount += 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }

        public Effect? DecreaseAmount()
        {
            if (amount > 1)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.amount -= 1;
                return newEffect;
            }
            else
            {
                return null;
            }
        }

        protected Effect? MakeMandatory()
        {
            if (!mandatory)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.mandatory = true;
                return newEffect;
            }
            return null;
        }

        protected Effect? MakeOptional()
        {
            if (mandatory)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.mandatory = false;
                return newEffect;
            }
            return null;
        } 
    }
}
