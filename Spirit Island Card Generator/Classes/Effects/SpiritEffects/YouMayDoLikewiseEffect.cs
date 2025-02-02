using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class YouMayDoLikewiseEffect : Effect
    {
        public override string Name => "You May Do Likewise";
        public override double BaseProbability { get { return .25; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override List<Type> IncompatibleEffects => new List<Type>() { };

        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"You may do likewise", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone => false;
        public override bool SelfReferencingPowerLevel => true;

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            
        };

        public override int PrintOrder()
        {
            return 9;
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"You may do likewise";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            //This can only be valid if the target type is another spirit, or if this is under the "If you target another spirit" effect.
            //We do want to use the card's target, rather than the Context's as having this on a land targeting card makes no sense
            if (!(context.card.Target.targetType == Target.TargetType.AnotherSpirit || context.HasEffectAbove(typeof(IfYouTargetAnotherSpiritEffect))) || context.HasEffectAbove(typeof(OrEffect)))
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            
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
            YouMayDoLikewiseEffect effect = new YouMayDoLikewiseEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override double CalculatePowerLevel()
        {
            return Context.settings.TargetPowerLevel / 2;
        }
    }
}
