using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    internal abstract class PieceDestroyed : GeneratorOption, IPowerPowerLevel
    {
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }
        public abstract Piece piece { get; }

        public abstract bool IsValid(Card card, Settings settings);

        //Estimates the effects own power level
        public abstract double CalculatePowerLevel();

        public IPowerPowerLevel Duplicate()
        {
            return this;
        }
    }
}
