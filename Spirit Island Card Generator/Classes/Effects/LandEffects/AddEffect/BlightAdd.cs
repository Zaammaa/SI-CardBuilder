using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    public class BlightAdd : PieceAdd
    {
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return .04; } set { } }
        public override int Complexity { get { return 3; } }
        public override Piece piece { get { return Piece.Blight; } }

        public override bool IsValid(Card card, Settings settings)
        {
            if (card.Target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Blighted))
                return false;

            return true;
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return -3.0;
        }
    }
}
