using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    [LandEffect]
    internal class TownDestroyEffect : DestroyEffect
    {
        public override double BaseProbability { get { return .04; } }
        public override int Complexity { get { return 1; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Town;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 0.8},
            { 3, 0.7},
        };

        protected override double PieceStrength => 1.3;

        public override IPowerLevel Duplicate()
        {
            TownDestroyEffect effect = new TownDestroyEffect();
            effect.amount = amount;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 1;
        }

        public override bool IsValid(Context context)
        {
            if (context.target.landConditions.Contains(LandConditions.NoInvaders) || context.target.landConditions.Contains(LandConditions.NoBuildings))
                return false;
            return true;
        }
    }
}
