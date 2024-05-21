using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes
{
    internal class EffectOption
    {
        public int weight;
        public Piece[] pieces;
        public string text;
        public double powerMult;
        public double baseStrength;
        public int customLvl;

        public EffectOption(int w, Piece[] pieces, string text, double powerMult, double powerAdd, int customLvl = 0)
        {
            weight = w;
            this.pieces = pieces;
            this.text = text;
            this.powerMult = powerMult;
            this.baseStrength = powerAdd;
            this.customLvl = customLvl;
        }
    }
}
