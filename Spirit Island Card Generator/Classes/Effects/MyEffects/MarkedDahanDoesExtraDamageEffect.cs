using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.MyEffects
{
    [LandEffect]
    [CustomEffect(4)]
    [LimitedOption(2)]
    internal class MarkedDahanDoesExtraDamageEffect : Effect
    {
        public override string Name => "Dahan Does More Damage";
        public override int Complexity { get { return 5; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => [];

        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        //Writes what goes on the card
        public override string Print()
        {
            return "Mark a {dahan}. It does +1 Damage after Ravage (It can still die, but doesn't need to be targeted first by Invaders/Events)";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoDahan))
            {
                return false;
            }
            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return 1;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override MarkedDahanDoesExtraDamageEffect Duplicate()
        {
            MarkedDahanDoesExtraDamageEffect effect = new MarkedDahanDoesExtraDamageEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
