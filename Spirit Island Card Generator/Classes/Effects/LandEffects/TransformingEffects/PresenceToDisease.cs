using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.TransformingEffects
{
    [LandEffect]
    [CustomEffect(3)]
    internal class PresenceToDisease : TransformationEffect
    {
        public override GamePieces.Piece FromPiece => GamePieces.Piece.Presence;

        public override GamePieces.Piece ToPiece => GamePieces.Piece.Disease;

        public override double BaseProbability => 0.003;

        public override int Complexity => 2;

        public override IPowerLevel Duplicate()
        {
            PresenceToDisease effect = new PresenceToDisease();
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        protected override void InitializeEffect()
        {

        }
    }
}
