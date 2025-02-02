using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class BadlandsAddEffect : AddEffect, ITrackedStat
    {
        public override string Name => "Add Badlands";
        public static string TrackedName => "Add Badlands";
        public static int TargetAmount => 6;
        public bool ExactTarget => false;
        public ITrackedStat.Pool pool => ITrackedStat.Pool.None;
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun, Element.Earth }; } }
        public override double BaseProbability { get { return .06; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Badland;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.3},
            { 3, 1.5},
        };

        public override double effectStrength => 0.85;

        public override IPowerLevel Duplicate()
        {
            BadlandsAddEffect effect = new BadlandsAddEffect();
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
