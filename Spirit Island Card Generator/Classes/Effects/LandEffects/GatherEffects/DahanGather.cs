using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    internal class DahanGather : PieceGather
    {
        public override double BaseProbability { get { return .11; } }
        public override double AdjustedProbability { get { return .11; } set { } }
        public override int Complexity { get { return 3; } }
        public override Piece piece { get { return Piece.Dahan; } }

        public override bool IsValid(Card card, Settings settings)
        {
            return true;
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return 0.4;
        }
    }
}
