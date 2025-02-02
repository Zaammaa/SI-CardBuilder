using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.InvaderActionEffects
{
    [LandEffect]
    internal class SkipBuildActionEffect : Effect
    {
        public override string Name => "Skip Build";
        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
{
};

        public override double BaseProbability => 0.015;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 3;

        public override Regex descriptionRegex => new Regex("Skip 1 build action");

        //Writes what goes on the card
        public override string Print()
        {
            return "Skip 1 build action";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            //TODO: At some point This should avoid cards that have a blight add effect too
            if (!context.card.Fast || context.target.landConditions.Contains(LandConditions.NoInvaders) || context.target.landConditions.Contains(LandConditions.Disease))
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
            //TODO: work with the calculated power levels
            return 1;
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
            SkipBuildActionEffect effect = new SkipBuildActionEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
