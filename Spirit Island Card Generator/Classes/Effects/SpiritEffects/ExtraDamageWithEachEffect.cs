using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    [CustomEffect(2)]
    internal class ExtraDamageWithEachEffect : Effect
    {
        public override string Name => "Extra Damage For Powers";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire }; } }
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        protected override DifficultyOption[] difficultyOptions => [];

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"\+1 Damage with each Damage dealing Power they use this turn", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            if (Context.targetMentioned)
            {
                return $"{Context.GetTargetString(TargetType)} do +1 Damage with each Damage dealing Power they use this turn";
            }
            else
            {
                return $"{Context.GetTargetString(TargetType)} does +1 Damage with each Damage dealing Power they use this turn";
            }
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
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
            return 1;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override Effect Duplicate()
        {
            ExtraDamageWithEachEffect effect = new ExtraDamageWithEachEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
