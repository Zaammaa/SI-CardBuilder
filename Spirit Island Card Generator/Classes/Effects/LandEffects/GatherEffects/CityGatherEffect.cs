using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    [LandEffect]
    internal class CityGatherEffect : GatherEffect
    {
        public override double BaseProbability { get { return .01; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.City;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 1.3},
            { 3, 1.5},
        };

        public override double effectStrength => 2.0;

        public override IPowerLevel Duplicate()
        {
            CityGatherEffect effect = new CityGatherEffect();
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
