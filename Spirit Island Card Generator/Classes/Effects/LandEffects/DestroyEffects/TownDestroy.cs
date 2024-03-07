using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    internal class TownDestroy : PieceDestroyed
    {
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return .04; } set { } }
        public override int Complexity { get { return 1; } }
        public override Piece piece { get { return Piece.Town; } }

        public override bool IsValid(Card card, Settings settings)
        {
            if (card.Target.landConditions.Contains(LandConditions.NoInvaders) || card.Target.landConditions.Contains(LandConditions.NoBuildings))
                return false;
            return true;
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return 1.3;
        }
    }
}
