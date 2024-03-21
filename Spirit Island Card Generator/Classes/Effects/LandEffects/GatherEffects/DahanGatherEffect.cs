using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    [LandEffect]
    internal class DahanGatherEffect : GatherEffect
    {
        public override double BaseProbability { get { return .11; } }
        public override int Complexity { get { return 1; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Dahan;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.1},
            { 3, 1.2},
            { 4, 1.2},
        };

        protected override double PieceStrength => 0.3;

        public override IPowerLevel Duplicate()
        {
            DahanGatherEffect effect = new DahanGatherEffect();
            effect.amount = amount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 2;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }
    }
}
