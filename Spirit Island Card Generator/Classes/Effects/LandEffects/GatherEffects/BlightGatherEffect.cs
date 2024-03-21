using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    [LandEffect]
    internal class BlightGatherEffect : GatherEffect
    {

        public override double BaseProbability { get { return .03; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Blight;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.2},
            { 3, 1.3},
        };

        protected override double PieceStrength => 0.5;

        public override IPowerLevel Duplicate()
        {
            BlightGatherEffect effect = new BlightGatherEffect();
            effect.amount = amount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 1;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }
    }
}
