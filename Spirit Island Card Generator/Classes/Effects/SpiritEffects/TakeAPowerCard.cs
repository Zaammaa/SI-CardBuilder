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
    [CustomEffect(3)]
    internal class TakeAPowerCard : Effect
    {
        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override Regex descriptionRegex => new Regex(@"Take a minor power", RegexOptions.IgnoreCase);

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change whether it is played", 80, PlayIt, DontPlayIt),
        ];

        protected bool andPlayIt = false;

        public override double CalculatePowerLevel()
        {
            if (andPlayIt)
            {
                return 1;
            } else
            {
                return 0.6;
            }
            
        }

        public override IPowerLevel Duplicate()
        {
            TakeAPowerCard effect = new TakeAPowerCard();
            effect.andPlayIt = andPlayIt;
            effect.Context = Context?.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            if (andPlayIt && Context.targetMentioned)
            {
               return "they Take a Minor Power and play it(for free)";
            } else if (andPlayIt)
            {
                return $"{Context.GetTargetString(TargetType)} Takes a Minor Power and plays it(for free)";
            } else if (Context.targetMentioned)
            {
                return "they take a Minor Power into their discard";
            } else
            {
                return $"{Context.GetTargetString(TargetType)} Takes a Minor Power into their discard";
            }
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        protected override void InitializeEffect()
        {
            andPlayIt = Context.rng.Next(0, 100) < 20;
        }

        public Effect? PlayIt()
        {
            if (!andPlayIt)
            {
                TakeAPowerCard strongerEffect = (TakeAPowerCard)Duplicate();
                strongerEffect.andPlayIt = true;
                return strongerEffect;
            }
            return null;
        }

        public Effect? DontPlayIt()
        {
            if (andPlayIt)
            {
                TakeAPowerCard weakerEffect = (TakeAPowerCard)Duplicate();
                weakerEffect.andPlayIt = false;
                return weakerEffect;
            }
            return null;
        }
    }
}
