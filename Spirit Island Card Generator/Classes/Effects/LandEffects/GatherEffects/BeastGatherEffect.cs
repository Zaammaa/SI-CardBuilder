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
    internal class BeastGatherEffect : GatherEffect
    {
        public override double BaseProbability { get { return .01; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Beast;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 0.9},
            { 3, 0.8},
        };

        public override double effectStrength => 0.3;

        public override IPowerLevel Duplicate()
        {
            BeastGatherEffect effect = new BeastGatherEffect();
            effect.gatherAmount = gatherAmount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            gatherAmount = 1;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }
    }
}
