using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.CardMetaEffects;
using Spirit_Island_Card_Generator.Classes.Fixers;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LinkedEffects
{
    [ConditionalEffect]
    internal class InsteadDoStrongerEffect : Effect, IParentEffect
    {
        public override double BaseProbability => 0.5;

        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override int Complexity
        {
            get
            {
                int complexity = 4;
                complexity += GetChildren().Sum((effect) => effect.Complexity);
                return complexity;
            }
        }

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("ChangeStrength", 100, StrengthenEffect, WeakenEffect)
            ];

        public override int PrintOrder()
        {
            return 9;
        }

        protected Effect? strongerEffect;

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"instead,", RegexOptions.IgnoreCase);
            }
        }

        public override double CalculatePowerLevel()
        {
            return strongerEffect.CalculatePowerLevel() - LinkedEffects.First().CalculatePowerLevel();
        }

        public override IPowerLevel Duplicate()
        {
            InsteadDoStrongerEffect effect = new InsteadDoStrongerEffect();
            effect.strongerEffect = (Effect?)strongerEffect?.Duplicate();
            effect.Context = Context?.Duplicate();
            effect.LinkedEffects = new List<Effect>(LinkedEffects);
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            //We need a top-level effect that is not a parent effect. Also this should not be under an 'or' effect. Usually that just makes the card look like the 'or' effect should have been written to include the other effect.
            if (context.card.effects.Any((effect) => { return !effect.GetType().GetInterfaces().Contains(typeof(IParentEffect)); }) && !context.HasEffectAbove(typeof(OrEffect)))
            {
                //We have to set the linked effect here since we can't know if the upgrade will be successful otherwise, and failing to set it before initializing causes all sorts of problems
                Effect? baseEffect = context.card.effects.Find((effect) => { return !effect.GetType().GetInterfaces().Contains(typeof(IParentEffect)); });
                strongerEffect = baseEffect?.Strengthen();
                if (baseEffect != null && strongerEffect != null)
                {
                    LinkedEffects = new List<Effect>() { baseEffect };
                } else
                {
                    return false;
                }
                return true;
            } 
            return false;
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Instead, {strongerEffect.Print()}";
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
            //Setting these in the function that checks if the effect is vaid now
            //Effect? baseEffect = Context.card.effects.Find((effect) => { return !effect.GetType().GetInterfaces().Contains(typeof(IParentEffect)); });
            //strongerEffect = baseEffect?.Strengthen();
            //if (baseEffect != null)
            //{
            //    LinkedEffects.Add(baseEffect);
            //}
            //else
            //{
            //    throw new Exception("No Or Effect on card! This should not happen");
            //}
        }

        public IEnumerable<Effect> GetChildren()
        {
            if (strongerEffect == null)
                return new List<Effect>();
            return new List<Effect>() { strongerEffect };
        }

        public void ReplaceEffect(Effect oldEffect, Effect newEffect)
        {
            strongerEffect = newEffect;
        }

        protected override IValidFixer? CustomIsValid()
        {
            Effect linkedEffect = LinkedEffects.First();
            if (strongerEffect == null)
            {
                return new InvalidEffectFixer(this);
            }
            else if (strongerEffect.CalculatePowerLevel() <= linkedEffect.CalculatePowerLevel())
                return new GenericFixer(() =>
                {
                    Effect? evenStrongerEffect = strongerEffect.Strengthen();
                    if (evenStrongerEffect != null)
                    {
                        strongerEffect = evenStrongerEffect;
                        return new FixerResult(FixerResult.FixResult.FixSucceeded, null);
                    } else
                    {
                        return new FixerResult(FixerResult.FixResult.RemoveEffect, this);
                    }
                });
            else
                return null;
        }

        protected Effect? StrengthenEffect()
        {
            InsteadDoStrongerEffect strongerThis = (InsteadDoStrongerEffect)Duplicate();
            strongerThis.strongerEffect = strongerEffect?.Strengthen();
            if (strongerThis.strongerEffect != null )
                return strongerThis;
            else 
                return null;
        }

        protected Effect? WeakenEffect()
        {
            InsteadDoStrongerEffect weakerThis = (InsteadDoStrongerEffect)Duplicate();
            weakerThis.strongerEffect = strongerEffect?.Weaken();
            if (weakerThis.strongerEffect != null)
                return weakerThis;
            else
                return null;
        }
    }
}
