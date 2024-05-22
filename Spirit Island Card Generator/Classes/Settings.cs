using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    public class Settings
    {
        //If true, the deck of generated cards will have the exact same elements that are in the base.
        //Effectively, this takes all the cards from the old deck, replaces everything but the elements, and then generates new effects/names/art/etc...
        public bool UseExactElements = false;
        //If true, the deck of generated cards will have the same number of slow and fast cards as the base deck
        public bool UseExactSlowToFastRatio = false;
        //If true, the deck of generated cards will have the same number of 0 cost and 1 cost cards as the base
        public bool UseExactCardCosts = false;

        public bool AllowRandomEffects = true;

        //How strong the cards should be on average.
        public double TargetPowerLevel = 1.0;
        //How much variance the card generator will allow in power level.
        public double PowerLevelVariance = 0.1;
        //How complex the generated deck should be. Some cards will still be more or less complicated.
        //Higher complexity means more outlandish effects can be generated, as well as allowing more effects on each card more often
        public int MaxComplexity = 20;
        public int MinComplexity = 8;

        //Weirdness of custom effects goes from 1-5. Each level includes all the ones below it
        public int CustomEffectLevel = 3;

        //Spirit targeting powers can be intentionally made a bit stronger to encourage cooperation
        public double SpiritEffectBuff = 0.1;

        //Generate Artworks
        public bool GenerateArt = false;

        public string workspace = AppSettings.Default.CardOutputDir;
        public string deckName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        public int seed = -1;
        private Random _rng;
        public Random rng { 
            get { 
                if (_rng != null)
                {
                    return _rng;
                }
                else if (seed == -1)
                {
                    _rng = new Random();
                    return _rng;
                } else
                {
                    _rng = new Random(seed);
                    return _rng;
                }
            }
        }
    }
}
