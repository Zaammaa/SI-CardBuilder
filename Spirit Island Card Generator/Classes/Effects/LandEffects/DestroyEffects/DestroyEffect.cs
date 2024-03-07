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
    internal class DestroyEffect : LandEffect
    {
        public override double BaseProbability { get { return .08; } }
        public override double AdjustedProbability { get { return .08; } set { } }
        public override int Complexity { get { return 2; } }

        public int amount = 1;

        public PieceDestroyed DestroyedPiece;

        protected virtual bool Valid(Card card, Settings settings)
        {
            throw new NotImplementedException();
        }

        public override Regex descriptionRegex
        {
            get
            {
                List<string> pieceNames = new List<string>();
                foreach (Piece piece in Enum.GetValues(typeof(Piece)))
                {
                    pieceNames.Add(piece.ToString());
                }
                string allPiecesRegex = "(" + String.Join("|", pieceNames) + ")";
                return new Regex(@"Destroy (\d) " + allPiecesRegex, RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            if (!DestroyedPiece.IsValid(card, settings))
            {
                return false;
            }
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            //TODO: Care about power level
            if (card.CardType == Card.CardTypes.Minor)
            {
                DestroyedPiece = ChooseRandomDestroyedEffect(card, settings);
                amount = 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool HasDestroyedType(Piece piece)
        {
            if (DestroyedPiece != null && DestroyedPiece.piece == piece)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private PieceDestroyed ChooseRandomDestroyedEffect(Card card, Settings settings)
        {
            double totalWeight = 0;
            List<PieceDestroyed> DestroyedOptions = ReflectionManager.GetInstanciatedSubClasses<PieceDestroyed>();
            List<PieceDestroyed> validOptions = new List<PieceDestroyed>();
            foreach (PieceDestroyed effect in DestroyedOptions)
            {
                if (!HasDestroyedType(effect.piece) && effect.IsValid(card, settings))
                {
                    totalWeight += effect.AdjustedProbability;
                    validOptions.Add(effect);
                }
            }

            double choice = settings.rng.NextDouble() * totalWeight;
            double index = 0;
            foreach (PieceDestroyed effect in validOptions)
            {
                if (choice < index + effect.AdjustedProbability)
                {
                    return effect;
                }
                index += effect.AdjustedProbability;
            }
            throw new Exception("No choice made");
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            //This will also need diminishing returns for having lots of pieces since a land may not have all?
            //Or maybe it would be higher since it's more efficient? Either way, I doubt it's as simple as a simple Destroyedition
            double powerLevel = 0;
            powerLevel += DestroyedPiece.CalculatePowerLevel() * amount;
            return powerLevel;
        }

        public override string Print()
        {
            string piecesText = GamePieces.ToSIBuilderString(DestroyedPiece.piece);
            return $"Destroy {amount} {piecesText}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                amount = Int32.Parse(match.Groups[1].Value);
                string pieceString = match.Groups[2].Value;

                List<PieceDestroyed> DestroyedEffects = ReflectionManager.GetInstanciatedSubClasses<PieceDestroyed>();
                foreach (PieceDestroyed DestroyedType in DestroyedEffects)
                {
                    if (DestroyedType.piece.ToString().Equals(pieceString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DestroyedPiece = DestroyedType;
                        break;
                    }
                }
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            DestroyEffect effect = new DestroyEffect();
            effect.amount = amount;
            effect.DestroyedPiece = (PieceDestroyed)DestroyedPiece.Duplicate();

            return effect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();
                newEffect.amount += 1;
                if (newEffect.CalculatePowerLevel() > settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    newEffect = (DestroyEffect)Duplicate();
                    newEffect.DestroyedPiece = ChooseRandomDestroyedEffect(card, settings);
                    //This is not guaranteed to be better, but the card selector will confirm that
                    return newEffect;
                } else
                {
                    return newEffect;
                }
                
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                DestroyEffect newEffect = (DestroyEffect)Duplicate();
                newEffect.amount -= 1;
                if (newEffect.amount <= 0 || newEffect.CalculatePowerLevel() < settings.TargetPowerLevel - settings.PowerLevelVariance)
                {
                    newEffect = (DestroyEffect)Duplicate();
                    newEffect.DestroyedPiece = ChooseRandomDestroyedEffect(card, settings);
                    //This is not guaranteed to be better, but the card selector will confirm that
                    return newEffect;
                }
                else
                {
                    return newEffect;
                }

            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
