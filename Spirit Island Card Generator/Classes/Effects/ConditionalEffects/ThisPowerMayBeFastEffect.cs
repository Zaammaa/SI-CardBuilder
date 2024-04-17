using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffectAttribute]
    internal class ThisPowerMayBeFastEffect : Effect
    {
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Air }; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"This power may be fast", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone { get { return false; } }

        protected override DifficultyOption[] difficultyOptions =>
        [
        ];

        //Writes what goes on the card
        public override string Print()
        {
            return "This power may be {fast}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            foreach(Condition c in context.conditions) { 
                //Powers don't target until the slow phase, so land targeting conditions don't make sense
                if (c.GetType() == typeof(LandTypeCondition) || c.GetType() == typeof(PlayPowerSlowCondition)) {
                    return false;
                }
            }
            if (context.card.ContainsSameEffectType(this) || context.card.Fast)
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
            return 0.15;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override Effect Duplicate()
        {
            ThisPowerMayBeFastEffect effect = new ThisPowerMayBeFastEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
