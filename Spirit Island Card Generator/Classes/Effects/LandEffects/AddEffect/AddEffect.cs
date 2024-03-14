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
    public abstract class AddEffect : Effect
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
                return new Regex(@"Add (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
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
            return $"Add {amount} " + "{" + Piece + "}";
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
                AddEffect newEffect = (AddEffect)Duplicate();

                newEffect.amount += 1;

                return newEffect;
            }
            else if (amount > 1 && PieceStrength < 0)
            {
                AddEffect newEffect = (AddEffect)Duplicate();

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
                AddEffect newEffect = (AddEffect)Duplicate();
                newEffect.amount -= 1;
                return newEffect;
            }
            else if (amount < ExtraPiecesMultiplier.Count && PieceStrength < 0)
            {
                AddEffect newEffect = (AddEffect)Duplicate();

                newEffect.amount += 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }
        //public override double BaseProbability { get { return .38; } }
        //public override double AdjustedProbability { get { return .38; } set { }  }
        //public override int Complexity { get { return 2; } }

        //public int amount = 1;

        //public PieceAdd addedPiece;

        //protected virtual bool Valid(Context context)
        //{
        //    throw new NotImplementedException();
        //}

        //public override Regex descriptionRegex
        //{
        //    get
        //    {
        //        List<string> pieceNames = new List<string>();
        //        foreach (Piece piece in Enum.GetValues(typeof(Piece)))
        //        {
        //            pieceNames.Add(piece.ToString());
        //        }
        //        string allPiecesRegex = "(" + String.Join("|", pieceNames) + ")";
        //        return new Regex(@"Add (\d) " + allPiecesRegex, RegexOptions.IgnoreCase);
        //    }
        //}

        ////Checks if this should be an option for the card generator
        //public override bool IsValid(Context context)
        //{
        //    if (!addedPiece.IsValid(card, settings))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        ////Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        //protected override void InitializeEffect()
        //{
        //    //TODO: Care about power level
        //    if (card.CardType == Card.CardTypes.Minor)
        //    {
        //        addedPiece = ChooseRandomAddEffect(card, settings);
        //        amount = settings.rng.Next(1, 3);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public bool HasAddType(Piece piece)
        //{
        //    if (addedPiece != null && addedPiece.piece == piece)
        //    {
        //        return true;
        //    } else
        //    {
        //        return false;
        //    }
        //}

        //private PieceAdd ChooseRandomAddEffect(Context context)
        //{
        //    double totalWeight = 0;
        //    List<PieceAdd> AddOptions = ReflectionManager.GetInstanciatedSubClasses<PieceAdd>();
        //    List<PieceAdd> validOptions = new List<PieceAdd>();
        //    foreach (PieceAdd effect in AddOptions)
        //    {
        //        if (!HasAddType(effect.piece) && effect.IsValid(card, settings))
        //        {
        //            totalWeight += effect.AdjustedProbability;
        //            validOptions.Add(effect);
        //        }
        //    }

        //    double choice = settings.rng.NextDouble() * totalWeight;
        //    double index = 0;
        //    foreach (PieceAdd effect in validOptions)
        //    {
        //        if (choice < index + effect.AdjustedProbability)
        //        {
        //            return effect;
        //        }
        //        index += effect.AdjustedProbability;
        //    }
        //    throw new Exception("No choice made");
        //}

        ////Estimates the effects own power level
        //public override double CalculatePowerLevel()
        //{
        //    //TODO: work with the calculated power levels
        //    //This will also need diminishing returns for having lots of pieces since a land may not have all?
        //    //Or maybe it would be higher since it's more efficient? Either way, I doubt it's as simple as a simple addition
        //    double powerLevel = 0;
        //    powerLevel += addedPiece.CalculatePowerLevel() * amount;
        //    return powerLevel;
        //}

        //public override string Print()
        //{
        //    string piecesText = GamePieces.ToSIBuilderString(addedPiece.piece);
        //    return $"Add {amount} {piecesText}";
        //}

        //public override bool Scan(string description)
        //{
        //    Match match = descriptionRegex.Match(description);
        //    if (match.Success)
        //    {
        //        amount = Int32.Parse(match.Groups[1].Value);
        //        string pieceString = match.Groups[2].Value;

        //        List<PieceAdd> AddEffects = ReflectionManager.GetInstanciatedSubClasses<PieceAdd>();
        //        foreach (PieceAdd AddType in AddEffects)
        //        {
        //            if (AddType.piece.ToString().Equals(pieceString, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                addedPiece = AddType;
        //                break;
        //            }
        //        }
        //    }
        //    return match.Success;
        //}

        ///// <summary>
        ///// Chooses whether to upgrade the piece being added or whether to increase the amount of the piece being added
        ///// Then generates a new effect based on that
        ///// </summary>
        ///// <param name="card"></param>
        ///// <param name="settings"></param>
        ///// <returns></returns>
        //public override Effect? Strengthen()
        //{
        //    AddEffect hypotheticalEffect = (AddEffect)Duplicate();
        //    double changePieceChance = .75;
        //    if (settings.rng.NextDouble() < changePieceChance)
        //    {
        //        hypotheticalEffect.amount = amount;
        //        List<PieceAdd> pieces = ReflectionManager.GetInstanciatedSubClasses<PieceAdd>();
        //        List<PieceAdd> validPieces = new List<PieceAdd>();
        //        foreach (PieceAdd AddType in pieces)
        //        {
        //            if (IPowerLevel.IsWithinAcceptablePowerLevel(card, settings, AddType, this.addedPiece))
        //            {
        //                validPieces.Add(AddType);
        //            }
        //        }
        //        if (validPieces.Count == 0)
        //            return null;

        //        hypotheticalEffect.addedPiece = (PieceAdd)Effect.ChooseEffect(card, settings, validPieces);
        //    } else
        //    {
        //        if (addedPiece.CalculatePowerLevel() < 0)
        //        {
        //            hypotheticalEffect.addedPiece = (PieceAdd)addedPiece.Duplicate();
        //            hypotheticalEffect.amount = amount - 1;
        //            if (hypotheticalEffect.amount <= 0)
        //            {
        //                return null;
        //            }
        //        } else
        //        {
        //            hypotheticalEffect.addedPiece = (PieceAdd)addedPiece.Duplicate();
        //            hypotheticalEffect.amount = amount + 1;
        //        }
        //    }
        //    return hypotheticalEffect;
        //}

        //public override Effect? Weaken()
        //{
        //    AddEffect hypotheticalEffect = (AddEffect)Duplicate();
        //    double changePieceChance = .75;
        //    if (settings.rng.NextDouble() < changePieceChance)
        //    {
        //        hypotheticalEffect.amount = amount;
        //        List<PieceAdd> pieces = ReflectionManager.GetInstanciatedSubClasses<PieceAdd>();
        //        List<PieceAdd> validPieces = new List<PieceAdd>();
        //        foreach (PieceAdd AddType in pieces)
        //        {
        //            if (IPowerLevel.IsWithinAcceptablePowerLevel(card, settings, AddType, this.addedPiece))
        //            {
        //                validPieces.Add(AddType);
        //            }
        //        }
        //        if (validPieces.Count == 0)
        //            return null;

        //        hypotheticalEffect.addedPiece = (PieceAdd)Effect.ChooseEffect(card, settings, validPieces);
        //    }
        //    else
        //    {
        //        if (addedPiece.CalculatePowerLevel() < 0)
        //        {
        //            return null;
        //        }
        //        else if (amount > 1)
        //        {
        //            hypotheticalEffect.addedPiece = (PieceAdd)addedPiece.Duplicate();
        //            hypotheticalEffect.amount = amount - 1;
        //        } else
        //        {
        //            return null;
        //        }
        //    }
        //    return hypotheticalEffect;
        //}

        //public override Effect Duplicate()
        //{
        //    AddEffect effect = new AddEffect();
        //    effect.amount = amount;
        //    effect.addedPiece = (PieceAdd)addedPiece.Duplicate();

        //    return effect;
        //}
    }
}
