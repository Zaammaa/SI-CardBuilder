using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    [SpiritCondition]
    internal class TerrorLevelConditionEffect : MultipleOptionEffect, IParentEffect
    {
        public override double BaseProbability => 0.02;

        public override int Complexity => 2 + GetChildren().Sum((effect) => effect.Complexity);

        public override Regex descriptionRegex => new Regex("");

        public List<Effect> effects = new List<Effect>();

        protected override List<EffectOption> EffectOptions
        {
            get
            {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(30, [], $"if Terror Level is 1", 0.7, 0),
                        new EffectOption(30, [], "if Terror Level is 2 or lower", 0.75, 0),
                        new EffectOption(20, [], "if Terror Level is 2 or higher", 0.5, 0),
                        new EffectOption(10, [], "if Terror Level is 2", 0.4, 0),
                        new EffectOption(5, [], "if Terror Level is 3", 0.25, 0),
                        new EffectOption(3, [], "if Terror Level is 1 or 3", 0.75, 0, 5),
                    ];
                }

                return _effectOptions;
            }
            set
            {
                _effectOptions = value;
            }
        }

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Choose better/worse option", 10, ChooseBetterOption, ChooseWorseOption),
            new DifficultyOption("Strengthen/Weaken child effect", 80, ChooseStrongerEffect, ChooseWeakerEffect),
            new DifficultyOption("Add/Remove effect", 10, AddNewEffect, RemoveEffect),
        ];

        public override double CalculatePowerLevel()
        {
            double effectPower = effects.Sum(x => x.CalculatePowerLevel());
            return effectPower * chosenOption.powerMult;
        }

        public override IPowerLevel Duplicate()
        {
            TerrorLevelConditionEffect dupEffect = new TerrorLevelConditionEffect();
            dupEffect.Context = Context?.Duplicate();
            dupEffect.effects = new List<Effect>(effects);
            dupEffect.chosenOption = chosenOption;
            dupEffect.EffectOptions = new List<EffectOption>(EffectOptions);
            return dupEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return effects;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        protected override void InitializeEffect()
        {
            base.InitializeEffect();

            effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
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

            return $"{chosenOption.text}, {effectText}";
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

        protected Effect? ChooseStrongerEffect()
        {
            TerrorLevelConditionEffect strongerThis = (TerrorLevelConditionEffect)Duplicate();
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
            TerrorLevelConditionEffect weakerThis = (TerrorLevelConditionEffect)Duplicate();
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
            TerrorLevelConditionEffect strongerThis = (TerrorLevelConditionEffect)Duplicate();
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
            TerrorLevelConditionEffect weakerThis = (TerrorLevelConditionEffect)Duplicate();
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
