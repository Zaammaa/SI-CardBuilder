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

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class StrifeAddEffect : AddEffect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Animal }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Sun, Element.Fire }; } }
        public override double BaseProbability { get { return .08; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Strife;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 1.2 }
        };

        public override double effectStrength => 0.75;

        public override IPowerLevel Duplicate()
        {
            StrifeAddEffect effect = new StrifeAddEffect();
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
            if (context.target.landConditions.Contains(LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(LandConditon.LandConditions.NoBuildings))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
