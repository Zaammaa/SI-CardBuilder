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

namespace Spirit_Island_Card_Generator.Classes.Effects.MyEffects
{
    [LandEffect]
    [CustomEffect(4)]
    internal class ForeachUnusedRangeEffect : Effect, IParentEffect
    {
        public override string Name => "For Each Unused Range";
        public override double BaseProbability => 0.01;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 4;

        public override Regex descriptionRegex => new Regex("");

        public List<Effect> effects = new List<Effect>();

        protected override DifficultyOption[] difficultyOptions => [
            
        ];

        public override double CalculatePowerLevel()
        {
            return Context.card.Range.range * 1.2 * effects.Sum(x => x.CalculatePowerLevel());
        }

        public override IPowerLevel Duplicate()
        {
            ForeachUnusedRangeEffect dupEffect = new ForeachUnusedRangeEffect();
            dupEffect.Context = Context?.Duplicate();
            dupEffect.effects = new List<Effect>(effects);
            return dupEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return effects;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Range.range < 1)
            {
                return false;
            }
            return true;
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

            return $"for each unused range, {effectText}";
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
            
        }

        protected Effect? ChooseStrongerEffect()
        {
            ForeachUnusedRangeEffect strongerThis = (ForeachUnusedRangeEffect)Duplicate();
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
            ForeachUnusedRangeEffect weakerThis = (ForeachUnusedRangeEffect)Duplicate();
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
            ForeachUnusedRangeEffect strongerThis = (ForeachUnusedRangeEffect)Duplicate();
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
            ForeachUnusedRangeEffect weakerThis = (ForeachUnusedRangeEffect)Duplicate();
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
