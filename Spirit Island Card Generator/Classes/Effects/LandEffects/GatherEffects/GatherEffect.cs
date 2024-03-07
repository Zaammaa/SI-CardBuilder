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
    internal class GatherEffect : LandEffect
    {
        public override double BaseProbability { get { return .17; } }
        public override double AdjustedProbability { get { return .17; } set { } }
        public override int Complexity { get { return 2; } }

        public List<PieceGather> GatherOptions = new List<PieceGather>();
        //Must VS. May
        public bool mandatory = false;
        public int amount = 1;

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
                string allPiecesRegex = "(?:" + String.Join("|", pieceNames) + ")";
                return new Regex(@"Gather (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            foreach (PieceGather GatherEffects in GatherOptions)
            {
                if (!GatherEffects.IsValid(card, settings))
                {
                    return false;
                }
            }
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            //TODO: Care about power level
            if (card.CardType == Card.CardTypes.Minor)
            {
                int numberOfPieces = settings.rng.Next(1, 4);
                for (int i = 0; i < numberOfPieces; i++)
                {
                    GatherOptions.Add(ChooseRandomGatherEffect(card, settings));
                }
                amount = 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool HasGatherType(Piece piece)
        {
            foreach (PieceGather GatherEffect in GatherOptions)
            {
                if (GatherEffect.piece == piece)
                {
                    return true;
                }
            }
            return false;
        }

        private PieceGather ChooseRandomGatherEffect(Card card, Settings settings)
        {
            double totalWeight = 0;
            List<PieceGather> GatherOptions = ReflectionManager.GetInstanciatedSubClasses<PieceGather>();
            List<PieceGather> validOptions = new List<PieceGather>();
            foreach (PieceGather effect in GatherOptions)
            {
                if (!HasGatherType(effect.piece) && effect.IsValid(card, settings))
                {
                    totalWeight += effect.AdjustedProbability;
                    validOptions.Add(effect);
                }
            }

            double choice = settings.rng.NextDouble() * totalWeight;
            double index = 0;
            foreach (PieceGather effect in validOptions)
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
            //If there's more than one options, the strongest option will be most of the power level since players will usually do that.
            //For now, we'll just use a flat power level for each other option.
            //TODO: If they are close in power, the increase from having more than one option becomes larger.
            double powerLevel = 0;
            double strongestOptionPower = 0;
            foreach (PieceGather gatherEffect in GatherOptions)
            {
                if (gatherEffect.CalculatePowerLevel() > strongestOptionPower)
                {
                    strongestOptionPower = gatherEffect.CalculatePowerLevel() * amount;
                }
            }
            powerLevel += strongestOptionPower + (0.1 * GatherOptions.Count);
            return powerLevel;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";
            List<string> convertedPieceTexts = new List<string>();
            foreach (PieceGather GatherEffect in GatherOptions)
            {
                convertedPieceTexts.Add(GamePieces.ToSIBuilderString(GatherEffect.piece));
            }
            string piecesText = String.Join("/", convertedPieceTexts);
            return $"Gather {mayText}{amount} {piecesText}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                if (match.Groups[1].Value.Equals(""))
                {
                    mandatory = true;
                }
                amount = Int32.Parse(match.Groups[2].Value);
                string pieceString = match.Groups[3].Value;
                foreach (string piece in pieceString.Split("/"))
                {
                    List<PieceGather> GatherEffects = ReflectionManager.GetInstanciatedSubClasses<PieceGather>();
                    foreach (PieceGather GatherType in GatherEffects)
                    {
                        if (GatherType.piece.ToString().Equals(piece, StringComparison.InvariantCultureIgnoreCase))
                        {
                            GatherOptions.Add(GatherType);
                        }
                    }
                }
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            GatherEffect effect = new GatherEffect();
            effect.amount = amount;
            foreach (PieceGather piece in GatherOptions)
            {
                effect.GatherOptions.Add(piece);
            }
            return effect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.amount += 1;
                if (newEffect.CalculatePowerLevel() > settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    //Add another piece
                    newEffect = (GatherEffect)Duplicate();
                    newEffect.GatherOptions.Add(ChooseRandomGatherEffect(card, settings));
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

        public override Effect? Weaken(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                GatherEffect newEffect = (GatherEffect)Duplicate();
                newEffect.amount -= 1;
                if (newEffect.amount <= 0 || newEffect.CalculatePowerLevel() > settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    if (GatherOptions.Count > 1) {
                        //Remove an option
                        newEffect = (GatherEffect)Duplicate();
                        PieceGather piece = Utils.ChooseRandomListElement(GatherOptions, settings.rng);
                        newEffect.GatherOptions.Remove(piece);
                        return newEffect;
                    } else
                    {
                        //Add another piece
                        newEffect = (GatherEffect)Duplicate();
                        newEffect.GatherOptions.Clear();
                        newEffect.GatherOptions.Add(ChooseRandomGatherEffect(card, settings));
                        //This is not guaranteed to be better, but the card selector will confirm that
                        return newEffect;
                    }
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
