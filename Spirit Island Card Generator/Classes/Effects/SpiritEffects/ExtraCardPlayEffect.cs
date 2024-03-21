using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class ExtraCardPlayEffect : Effect
    {
        public override double BaseProbability { get { return .07; } }
        public override double AdjustedProbability { get { return .07; } set { } }
        public override int Complexity { get { return 1; } }

        protected override DifficultyOption[] difficultyOptions => [];

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit may (?:immediately)? play another Power Card by paying its cost", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Target Spirit may immediately play another Power Card by paying its cost.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.target.SpiritTarget || !context.card.Fast)
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
            return 0.75;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override Effect Duplicate()
        {
            ExtraCardPlayEffect effect = new ExtraCardPlayEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
