using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    [SpiritEffect]
    [CustomEffect(2)]
    internal class IfTopCardOfMinorDeckHasElementEffect : Effect, IParentEffect
    {
        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 5 + effects.Sum(x => x.Complexity);

        public override Regex descriptionRegex => new Regex("");

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Strengthen/Weaken child effect", 80, ChooseStrongerEffect, ChooseWeakerEffect),
            new DifficultyOption("Add/Remove effect", 10, AddNewEffect, RemoveEffect),
        ];

        protected List<Effect> effects = new List<Effect>();
        protected ElementSet.Element element;

        public override double CalculatePowerLevel()
        {
            return 0.3 * effects.Sum(x => x.CalculatePowerLevel());
        }

        public override IPowerLevel Duplicate()
        {
            IfTopCardOfMinorDeckHasElementEffect dupEffect = new IfTopCardOfMinorDeckHasElementEffect();
            dupEffect.element = element;
            dupEffect.effects = new List<Effect>(effects);
            dupEffect.Context = Context.Duplicate();
            return dupEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return effects;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return context.settings.AllowRandomEffects;
        }

        public override string Print()
        {
            string effectText = "";
            bool first = true;
            foreach (Effect effect in effects)
            {
                if (!first)
                {
                    effectText += " and ";
                }
                else
                {
                    first = false;
                }
                effectText += effect.Print();
            }

            return "discard the top Minor Power. If it has {" + element.ToString().ToLower() + "} " + effectText;
        }

        public void ReplaceEffect(Effect oldEffect, Effect newEffect)
        {
            if (effects.Remove(oldEffect))
            {
                effects.Add(newEffect);
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }

        public override bool Scan(string description)
        {
            return false;
        }

        protected override void InitializeEffect()
        {
            element = Utils.ChooseRandomEnumValue<ElementSet.Element>(typeof(ElementSet.Element), Context.rng);
            effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
        }

        protected Effect? ChooseStrongerEffect()
        {
            IfTopCardOfMinorDeckHasElementEffect strongerThis = (IfTopCardOfMinorDeckHasElementEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.effects, Context.rng);

            Effect? strongerEffect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), effectToStrengthen.CalculatePowerLevel());
            if (effectToStrengthen != null && strongerEffect != null)
            {
                strongerThis.effects.Remove(effectToStrengthen);
                strongerThis.effects.Add(strongerEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? ChooseWeakerEffect()
        {
            IfTopCardOfMinorDeckHasElementEffect weakerThis = (IfTopCardOfMinorDeckHasElementEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.effects, Context.rng);

            Effect? weakerEffect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), effectToWeaken.CalculatePowerLevel());
            if (effectToWeaken != null && weakerEffect != null)
            {
                weakerThis.effects.Remove(effectToWeaken);
                weakerThis.effects.Add(weakerEffect);
                return weakerThis;
            }
            return null;
        }

        protected Effect? AddNewEffect()
        {
            IfTopCardOfMinorDeckHasElementEffect strongerThis = (IfTopCardOfMinorDeckHasElementEffect)Duplicate();
            Effect newEffect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (newEffect != null)
            {
                strongerThis.effects.Add(newEffect);
                return strongerThis;
            }
            else
            {
                return null;
            }

        }

        protected Effect? RemoveEffect()
        {
            IfTopCardOfMinorDeckHasElementEffect weakerThis = (IfTopCardOfMinorDeckHasElementEffect)Duplicate();
            if (effects.Count > 1)
            {
                Effect effectToRemove = Utils.ChooseRandomListElement(effects, Context.rng);
                weakerThis.effects.Remove(effectToRemove);
                return weakerThis;
            }
            else
            {
                return null;
            }
        }
    }
}
