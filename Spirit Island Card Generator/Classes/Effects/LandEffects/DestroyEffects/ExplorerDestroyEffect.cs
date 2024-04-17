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

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    [LandEffect]
    internal class ExplorerDestroyEffect : DestroyEffect
    {
        public override double BaseProbability { get { return .06; } }
        public override int Complexity { get { return 1; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Explorer;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 0.8},
            { 3, 0.5},
        };

        public override double effectStrength => 0.85;

        public override IPowerLevel Duplicate()
        {
            ExplorerDestroyEffect effect = new ExplorerDestroyEffect();
            effect.destroyAmount = destroyAmount;
            effect.Context = Context.Duplicate();
            return effect;
        }

        protected override void InitializeEffect()
        {
            destroyAmount = 1;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(LandConditon.LandConditions.NoInvaders))
                return false;
            return true;
        }
    }
}
