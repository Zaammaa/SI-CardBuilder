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

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    [LandEffect]
    internal class PresencePushEffect : PushEffect
    {
        public override double BaseProbability { get { return .01; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Presence;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, .9}
        };

        public override double effectStrength => 0.7;

        public override IPowerLevel Duplicate()
        {
            PresencePushEffect effect = new PresencePushEffect();
            effect.pushAmount = pushAmount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            pushAmount = 1;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }
    }
}
