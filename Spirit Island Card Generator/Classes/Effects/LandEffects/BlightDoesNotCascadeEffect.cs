using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    internal class BlightDoesNotCascadeEffect : Effect
    {
        public override string Name => "Blight Does Not Cascade";
        public override double BaseProbability { get { return .01; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Earth }; } }

        public override List<Type> IncompatibleEffects => new List<Type>() { };
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"doesn't cascade", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => [];

        //Writes what goes on the card
        public override string Print()
        {
            return $"When " + "{blight}" +  $" is added to {Context.GetTargetString(TargetType)}, it doesn't cascade";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.ContainsSameEffectType(this) || !context.card.Fast || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Noblight))
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
            return 0.35;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override BlightDoesNotCascadeEffect Duplicate()
        {
            BlightDoesNotCascadeEffect effect = new BlightDoesNotCascadeEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
