﻿using Spirit_Island_Card_Generator.Classes.Attributes;
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
    internal class ExplorerToBeast : TransformationEffect
    {
        public override GamePieces.Piece FromPiece => GamePieces.Piece.Explorer;

        public override GamePieces.Piece ToPiece => GamePieces.Piece.Beast;

        public override double BaseProbability => 0.015;

        public override int Complexity => 1;

        public override IPowerLevel Duplicate()
        {
            ExplorerToBeast effect = new ExplorerToBeast();
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders))
                return false;
            return true;
        }

        protected override void InitializeEffect()
        {

        }
    }
}
