using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class BeastAddEffect : AddEffect, ITrackedStat
    {
        public override string Name => "Add Beast";
        public static string TrackedName => "Add Beast";
        public static int TargetAmount => 7;
        public bool ExactTarget => false;
        public ITrackedStat.Pool pool => ITrackedStat.Pool.None;
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal }; } }
        public override double BaseProbability { get { return .07; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Beast;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.2},
        };

        public override double effectStrength => 0.65;

        public override IPowerLevel Duplicate()
        {
            BeastAddEffect effect = new BeastAddEffect();
            effect.Context = Context.Duplicate();
            effect.addAmount = addAmount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            addAmount = 1;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}
