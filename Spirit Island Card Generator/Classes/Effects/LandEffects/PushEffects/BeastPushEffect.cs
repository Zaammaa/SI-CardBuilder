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
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    [LandEffect]
    internal class BeastPushEffect : PushEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Air }; } }
        public override double BaseProbability { get { return .01; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Beast;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, .7},
            { 3, .4},
        };

        public override double effectStrength => 0.25;

        public override IPowerLevel Duplicate()
        {
            BeastPushEffect effect = new BeastPushEffect();
            effect.pushAmount = pushAmount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            pushAmount = 1;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}
