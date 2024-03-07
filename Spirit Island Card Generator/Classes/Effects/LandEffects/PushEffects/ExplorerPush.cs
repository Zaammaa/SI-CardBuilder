using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    public class ExplorerPush : PiecePush
    {
        public override double BaseProbability { get { return .16; } }
        public override double AdjustedProbability { get { return .16; } set { } }
        public override int Complexity { get { return 2; } }
        public override Piece piece { get { return Piece.Explorer; } }

        public override bool IsValid(Card card, Settings settings)
        {
            if (card.Target.landConditions.Contains(LandConditon.LandConditions.NoInvaders))
            {
                return false;
            } else
            {
                return true;
            }
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return 0.75;
        }
    }
}
