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
    internal class WildsAddEffect : AddEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Plant }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Earth }; } }
        public override double BaseProbability { get { return .09; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Wilds;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 0.6 }
        };

        public override double effectStrength => 0.75;

        public override IPowerLevel Duplicate()
        {
            WildsAddEffect effect = new WildsAddEffect();
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
