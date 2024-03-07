using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    public class IsolateEffect : LandEffect
    {
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return .04; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Isolate target land", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Isolate target land.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            if (card.ContainsEffect(this.GetType()) || card.Target.SpiritTarget || !card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return 0.2;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override IsolateEffect Duplicate()
        {
            IsolateEffect effect = new IsolateEffect();
            return effect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            return null;
        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            return null;
        }
    }
}
