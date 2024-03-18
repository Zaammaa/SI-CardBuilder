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

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class StrifeAddEffect : AddEffect
    {
        public override double BaseProbability { get { return .08; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Strife;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 1.2 }
        };

        protected override double PieceStrength => 0.75;

        public override IPowerLevel Duplicate()
        {
            StrifeAddEffect effect = new StrifeAddEffect();
            effect.Context = Context.Duplicate();
            effect.amount = amount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 1;
        }

        public override bool IsValid(Context context)
        {
            if (context.target.landConditions.Contains(LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(LandConditon.LandConditions.NoBuildings))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
