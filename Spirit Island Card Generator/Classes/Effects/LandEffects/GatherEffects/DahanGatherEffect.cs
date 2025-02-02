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

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    [LandEffect]
    internal class DahanGatherEffect : GatherEffect
    {
        public override string Name => "Gather Dahan";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal, Element.Sun }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Air }; } }

        public override double BaseProbability { get { return .11; } }
        public override int Complexity { get { return 1; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Dahan;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.1},
            { 3, 1.2},
            { 4, 1.2},
        };

        public override double effectStrength => 0.3;

        public override IPowerLevel Duplicate()
        {
            DahanGatherEffect effect = new DahanGatherEffect();
            effect.gatherAmount = gatherAmount;
            effect.mandatory = mandatory;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            gatherAmount = 2;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}
