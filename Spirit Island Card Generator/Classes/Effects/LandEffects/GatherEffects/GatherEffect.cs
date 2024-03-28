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
    internal abstract class GatherEffect : AmountEffect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }

        //Must VS. May
        public bool mandatory = false;
        [AmountValue]
        public int gatherAmount = 1;

        public override int PrintOrder()
        {
            return 4;
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 30, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Make Optional/Mandatory", 20, MakeOptional, MakeMandatory),
            new DifficultyOption("Add option", 50, AddOption, RemoveOption),
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
            double powerLevel = base.CalculatePowerLevel();
            if (mandatory)
            {
                powerLevel -= 0.05 * gatherAmount;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";

            return $"Gather {mayText}{gatherAmount} " + "{" + Piece.ToString().ToLower()+"}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
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

        //Intentionally return a MergedGather effect rather than a new GatherEffect
        protected Effect? AddOption()
        {
            MergedGatherEffect mergedGather = new MergedGatherEffect();
            mergedGather.InitializeEffect(UpdateContext());
            mergedGather.amount = gatherAmount;
            mergedGather.mandatory = mandatory;
            mergedGather.gatherEffects = [(GatherEffect)Duplicate()];
            GatherEffect? newGather = (GatherEffect?)Context?.effectGenerator.ChooseGeneratorOption<GatherEffect>(UpdateContext());
            if (newGather != null)
            {
                if (newGather.GetType() != this.GetType())
                {
                    newGather.gatherAmount = gatherAmount;
                    newGather.mandatory = mandatory;
                    newGather.Context = Context.Duplicate();
                    mergedGather.gatherEffects.Add(newGather);
                    return mergedGather;
                } else
                {
                    return null;
                }

            } else
            {
                return null;
            }
        }
        //We need a counterpart for the effectOption, but it doesn't actually make sense. The MergedGatherEffect has the downgrade if needed
        protected Effect? RemoveOption()
        {
            return null;
        }
    }
}
