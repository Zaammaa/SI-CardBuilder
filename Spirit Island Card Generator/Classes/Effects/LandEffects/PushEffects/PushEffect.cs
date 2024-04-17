using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    [LandEffect]
    public abstract class PushEffect : AmountEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air}; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public abstract Piece Piece { get; }

        //Must VS. May
        public bool mandatory = false;
        [AmountValue]
        public int pushAmount = 1;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change amounts", 30, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Make Optional/Mandatory", 30, MakeOptional, MakeMandatory),
            new DifficultyOption("Add option", 40, AddOption, RemoveOption),
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
                string allPiecesRegex = "(?:"+String.Join("|", pieceNames)+")";
                return new Regex(@"Push (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }


        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = base.CalculatePowerLevel();
            if (mandatory)
            {
                powerLevel -= 0.05 * pushAmount;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";
            List<string> convertedPieceTexts = new List<string>();
            //foreach (PiecePush pushEffect in pushOptions)
            //{
            //    convertedPieceTexts.Add(GamePieces.ToSIBuilderString(pushEffect.piece));
            //}
            //string piecesText = String.Join("/",convertedPieceTexts);
            return $"Push {mayText}{pushAmount} " + "{" + Piece.ToString().ToLower() + "}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                //if (match.Groups[1].Value.Equals(""))
                //{
                //    mandatory = true;
                //}
                //amount = Int32.Parse(match.Groups[2].Value);
                //string pieceString = match.Groups[3].Value;
                //foreach (string piece in pieceString.Split("/"))
                //{
                //    List<PiecePush> PushEffects = ReflectionManager.GetInstanciatedSubClasses<PiecePush>();
                //    foreach(PiecePush pushType in PushEffects)
                //    {
                //        if (pushType.piece.ToString().Equals(piece,StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            pushOptions.Add(pushType);
                //        }
                //    }
                //}
            }
            return match.Success;
        }

        protected Effect? MakeMandatory()
        {
            if (!mandatory)
            {
                PushEffect newEffect = (PushEffect)Duplicate();
                newEffect.mandatory = true;
                return newEffect;
            }
            return null;
        }

        protected Effect? MakeOptional()
        {
            if (mandatory)
            {
                PushEffect newEffect = (PushEffect)Duplicate();
                newEffect.mandatory = false;
                return newEffect;
            }
            return null;
        }

        //Intentionally return a MergedPush effect rather than a new PushEffect
        protected Effect? AddOption()
        {
            MergedPushEffect mergedPush = new MergedPushEffect();
            mergedPush.InitializeEffect(UpdateContext());
            mergedPush.amount = pushAmount;
            mergedPush.mandatory = mandatory;
            mergedPush.pushEffects = [(PushEffect)Duplicate()];
            PushEffect? newPush = (PushEffect?)Context?.effectGenerator.ChooseGeneratorOption<PushEffect>(UpdateContext());
            if (newPush != null && (newPush.GetType() != this.GetType()))
            {
                newPush.pushAmount = pushAmount;
                newPush.mandatory = mandatory;
                newPush.Context = Context.Duplicate();
                mergedPush.pushEffects.Add(newPush);
                return mergedPush;
            }
            else
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
