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
    internal class DiseaseAddEffect : AddEffect, ITrackedStat
    {
        public static string TrackedName => "Add disease";
        public static int TargetAmount => 7;
        public bool ExactTarget => false;
        public ITrackedStat.Pool pool => ITrackedStat.Pool.None;
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Water }; } }
        public override double BaseProbability { get { return .08; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Disease;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 0.6 }
        };

        public override double effectStrength => 0.9;

        public override IPowerLevel Duplicate()
        {
            DiseaseAddEffect effect = new DiseaseAddEffect();
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
            if (context.card.Fast)
                return false;

            return true;
        }
    }
}
