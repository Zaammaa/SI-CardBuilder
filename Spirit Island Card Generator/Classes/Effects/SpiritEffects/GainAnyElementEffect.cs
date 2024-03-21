using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class GainAnyElementEffect : Effect
    {
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        protected override DifficultyOption[] difficultyOptions => [];

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit gains (\d) Any", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            return "Target Spirit gains 1 {any}";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return 0.5;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            GainAnyElementEffect effect = new GainAnyElementEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
