using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System.Text.RegularExpressions;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class IfYouTargetAnotherSpiritEffect : Effect, IParentEffect
    {
        public override string Name => "If You Target Another Spirit";

        public override bool Standalone => false;

        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity
        {
            get
            {
                int complexity = 2;
                complexity += GetChildren().Sum((effect) => effect.Complexity);
                return complexity;
            }
        }

        public override bool MentionsTarget => true;

        public override bool TopLevelEffect()
        {
            return true;
        }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"if you target another Spirit,", RegexOptions.IgnoreCase);
            }
        }

        public string EffectText
        {
            get
            {
                string effectText = "";
                bool first = true;
                foreach (Effect effect in Effects)
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
                return effectText;
            }
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change effect Strength", 95, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Add/Remove effect", 5, AddEffect, RemoveEffect),
        };

        protected double powerLevelModifier = 0.75;
        protected List<Effect> Effects = new List<Effect>();

        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effect in Effects)
            {
                powerLevel += effect.CalculatePowerLevel();
            }
            return powerLevel * powerLevelModifier;
        }

        public override IPowerLevel Duplicate()
        {
            IfYouTargetAnotherSpiritEffect effect = new IfYouTargetAnotherSpiritEffect();
            effect.Effects = new List<Effect>(Effects);
            effect.Context = Context?.Duplicate();
            return effect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return Effects;
        }
        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (Effects.Remove(effect))
            {
                Effects.Add(newEffect);
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.targetType != Target.TargetType.AnySpirit)
                return false;

            return true;
        }

        public override string Print()
        {
            return $"If you target another Spirit, {EffectText}";
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
            Effect effect = (Effect)Context.effectGenerator.ChooseEffect(UpdateContext());
            if (effect != null)
            {
                Effects.Add(effect);
            }
        }

        protected Effect? StrengthenEffect()
        {
            IfYouTargetAnotherSpiritEffect strongerThis = (IfYouTargetAnotherSpiritEffect)Duplicate();
            Effect? effect = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);
            Effect? newEffect = (Effect?)effect?.Strengthen();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseStrongerEffect(UpdateContext(), effect.CalculatePowerLevel());
            }
            if (newEffect != null)
            {
                strongerThis.Effects.Remove(effect);
                strongerThis.Effects.Add(newEffect);
                return strongerThis;
            }
            else
            {
                return null;
            }

        }

        protected Effect? WeakenEffect()
        {
            IfYouTargetAnotherSpiritEffect weakerThis = (IfYouTargetAnotherSpiritEffect)Duplicate();
            Effect? effect = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);
            Effect? newEffect = (Effect?)effect?.Weaken();
            if (newEffect == null)
            {
                newEffect = (Effect?)Context?.effectGenerator.ChooseWeakerEffect(UpdateContext(), effect.CalculatePowerLevel());
            }
            if (newEffect != null)
            {
                weakerThis.Effects.Remove(effect);
                weakerThis.Effects.Add(newEffect);
                return weakerThis;
            }
            else
            {
                return null;
            }
        }

        protected Effect? AddEffect()
        {
            IfYouTargetAnotherSpiritEffect strongerThis = (IfYouTargetAnotherSpiritEffect)Duplicate();
            Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (effect != null)
            {
                strongerThis.Effects.Add(effect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? RemoveEffect()
        {
            if (Effects.Count > 1)
            {
                IfYouTargetAnotherSpiritEffect weakerThis = (IfYouTargetAnotherSpiritEffect)Duplicate();
                Effect? effect = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);
                weakerThis.Effects.Remove(effect);
                return weakerThis;
            }
            return null;
        }
    }
}
