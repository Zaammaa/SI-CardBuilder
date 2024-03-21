using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffect]
    internal class RepeatThisPowerEffect : Effect
    {
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Repeat this Power", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone { get { return false; } }

        protected override DifficultyOption[] difficultyOptions =>
        [
        ];

        //Writes what goes on the card
        public override string Print()
        {
            return "Repeat this Power.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.card.ContainsSameEffectType(this))
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
            double power = 0;
            foreach(Effect effect in Context.card.effects)
            {
                if (effect.Equals(this) || Context.IsParent(effect))
                    continue;
                power += effect.CalculatePowerLevel();
            }
            return power;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override Effect Duplicate()
        {
            RepeatThisPowerEffect effect = new RepeatThisPowerEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
