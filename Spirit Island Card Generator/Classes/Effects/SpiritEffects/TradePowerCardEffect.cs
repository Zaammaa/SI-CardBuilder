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
    [CustomEffect(2)]
    [LimitedOption(1)]
    internal class TradePowerCardEffect : Effect
    {
        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 4;

        public override Regex descriptionRegex => new Regex(@"gift the other 1 Power from hand", RegexOptions.IgnoreCase);

        protected override DifficultyOption[] difficultyOptions => [
        ];

        public override double CalculatePowerLevel()
        {
            return 0.7;
        }

        public override IPowerLevel Duplicate()
        {
            TradePowerCardEffect effect = new TradePowerCardEffect();
            effect.Context = Context?.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            return $"{Context.GetTargetString(TargetType)} and you may swap 1 power card from hand";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        protected override void InitializeEffect()
        {

        }
    }
}
