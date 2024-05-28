using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
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
    internal class InvadersDoNotHealEffect : Effect
    {
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Moon }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Air, Element.Water }; } }

        public override List<Type> IncompatibleEffects => new List<Type>() { };

        public override double BaseProbability { get { return .01; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Invaders do not heal", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => [];

        //Writes what goes on the card
        public override string Print()
        {
            return $"in {Context.GetTargetString(TargetType)}, Invaders do not heal Damage at end of turn";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
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

        public override InvadersDoNotHealEffect Duplicate()
        {
            InvadersDoNotHealEffect effect = new InvadersDoNotHealEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
