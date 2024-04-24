using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LinkedEffects
{
    [ConditionalEffect]
    [UnspecificLand]
    internal class YouMayDoBothEffect : Effect
    {
        public override double BaseProbability => 0.75;

        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override int Complexity => 2;

        protected override DifficultyOption[] difficultyOptions => [];

        public override int PrintOrder()
        {
            return 10;
        }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"you may do both", RegexOptions.IgnoreCase);
            }
        }

        public override double CalculatePowerLevel()
        {
            //Strength is approximately the weaker of the two choices since you probably pick the other one most of the time
            OrEffect orEffect = (OrEffect)LinkedEffects.First();
            return Math.Min(orEffect.choice1.CalculatePowerLevel(), orEffect.choice2.CalculatePowerLevel());
        }

        public override IPowerLevel Duplicate()
        {
            YouMayDoBothEffect effect = new YouMayDoBothEffect();
            effect.Context = Context.Duplicate();
            effect.LinkedEffects = new List<Effect>(LinkedEffects);
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.GetAllEffects().Any((Effect) => { return Effect.GetType().Equals(typeof(OrEffect)); }) && !context.HasEffectAbove(typeof(OrEffect))) {
                return true;
            } else
            {
                return false;
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            if (Context.target.SpiritTarget)
            {
                return "They may do both";
            }
            else
            {
                return "You may do both";
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        protected override void InitializeEffect()
        {
            Effect? OrEffect = Context.card.GetAllEffects().Find((effect) => { return effect.GetType().Equals(typeof(OrEffect)); });
            if (OrEffect != null)
            {
                LinkedEffects.Add(OrEffect);
            } else
            {
                throw new Exception("No Or Effect on card! This should not happen");
            }
        }
    }
}
