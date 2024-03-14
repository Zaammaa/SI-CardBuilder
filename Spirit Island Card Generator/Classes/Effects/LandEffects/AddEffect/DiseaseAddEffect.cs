using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class DiseaseAddEffect : AddEffect
    {
        public override double BaseProbability { get { return .08; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Disease;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 0.6 }
        };

        protected override double PieceStrength => 0.9;

        public override IPowerLevel Duplicate()
        {
            DiseaseAddEffect effect = new DiseaseAddEffect();
            effect.Context = Context;
            effect.amount = amount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 1;
        }

        public override bool IsValid(Context context)
        {
            if (context.card.Fast)
                return false;

            return true;
        }
    }
}
