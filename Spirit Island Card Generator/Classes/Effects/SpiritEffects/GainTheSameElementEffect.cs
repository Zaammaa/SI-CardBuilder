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
    internal class GainTheSameElementEffect : Effect
    {
        public override string Name => "Gain Same Element";
        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override Regex descriptionRegex => new Regex(@"2 of a single element", RegexOptions.IgnoreCase);

        protected override DifficultyOption[] difficultyOptions => [

        ];

        public override double CalculatePowerLevel()
        {
            return 0.5;
        }

        public override IPowerLevel Duplicate()
        {
            GainTheSameElementEffect effect = new GainTheSameElementEffect();
            effect.Context = Context?.Duplicate();
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;
            else
                return true;
        }

        public override string Print()
        {
            return $"{Context.GetTargetString(TargetType)} gains 2 of a single Element";
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
