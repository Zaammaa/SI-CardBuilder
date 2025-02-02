using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.MyEffects
{
    [LandEffect]
    [CustomEffect(3)]
    internal class AddWardEffect : Effect
    {
        public override string Name => "Add Ward";
        public override int Complexity { get { return 4; } }

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
            return "Add a Warding Pattern (Use a Scenario Marker. Warding Patterns provide defend 3 and are destroyed if {blight} is added to their land)";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Blighted))
                return false;

            return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {

        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return 1.2;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override AddWardEffect Duplicate()
        {
            AddWardEffect effect = new AddWardEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
