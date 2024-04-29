using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffect]
    [UnspecificLand]
    internal class RepeatThisPowerEffect : Effect
    {
        public override double BaseProbability { get { return .02; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 4; } }

        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Sun }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Moon, Element.Air }; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Repeat this Power", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone => false;
        public override bool SelfReferencingPowerLevel => true;

        protected override DifficultyOption[] difficultyOptions =>
        [
        ];

        //Writes what goes on the card
        public override string Print()
        {
            return "Repeat this Power.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.ContainsSameEffectType(this)|| context.Parent.GetType().Equals(typeof(OrEffect)))
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
            //double power = 0;
            //foreach(Effect effect in Context.card.effects)
            //{
            //    if (effect.SelfReferencingPowerLevel || Context.IsParent(effect, this))
            //        continue;
            //    power += effect.CalculatePowerLevel();
            //}
            //return power;
            return Context.settings.TargetPowerLevel / 2;
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
