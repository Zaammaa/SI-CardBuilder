using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class InEachOfSpiritsLandsEffect : MultipleOptionEffect, IParentEffect
    {
        public override double BaseProbability => 0.05;

        public override int Complexity => 3 + GetChildren().Sum((effect) => effect.Complexity);

        public override bool MentionsTarget => true;

        public override Regex descriptionRegex => new Regex("");

        public List<Effect> effects = new List<Effect>();

        protected override List<EffectOption> EffectOptions {
            get
            {
                if (_effectOptions.Count == 0)
                {
                    _effectOptions = [
                        new EffectOption(30, [Piece.Presence], $": ", 6, 0, 0),
                        new EffectOption(30, [Piece.SacredSite], "with their {sacred-site}: ", 3.5, 0, 1),
                        new EffectOption(15, [Piece.Presence, Piece.Dahan], "with {dahan}: ", 4, 0, 1),
                        new EffectOption(15, [Piece.Presence, Piece.Dahan], "with 2 or more {dahan}: ", 2.5, 0, 2),
                        new EffectOption(2, [Piece.Presence, Piece.Dahan], "with 3 or more {dahan}: ", 1.8, 0, 3),
                        new EffectOption(5, [Piece.Dahan, Piece.SacredSite], "with their {sacred-site} and {dahan}: ", 2, 0, 1),
                        new EffectOption(5, [Piece.Presence, Piece.Beast], "with {beast}: ", 3, 0, 2),
                        new EffectOption(2, [Piece.Presence, Piece.City], "with {city}: ", 3, 0, 2),
                        new EffectOption(2, [Piece.Presence, Piece.Invader], "with 3 or more Invaders: ", 2.5, 0, 2),
                    ];
                }

                return _effectOptions;
            }
            set
            {
                _effectOptions = value;
            }
        }

        public EffectGeneratorSettings effectSettings
        {
            get
            {
                EffectGeneratorSettings effectSettings = EffectGeneratorSettings.GetStandardEffectSettings(UpdateContext());
                effectSettings.bannedAttributes.Add(new UnspecificLandAttribute());
                return effectSettings;
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
            InEachOfSpiritsLandsEffect dupEffect = new InEachOfSpiritsLandsEffect();
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
            if (!context.card.Target.SpiritTarget)
                return false;

            return true;
        }

        public override string Print()
        {
            string target = Context.GetTargetString(TargetType);
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

            if (target.Equals("target Spirit", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"In each of {target}'s lands {chosenOption.text} {effectText}";
            } else if (target.Equals("they", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"In each of their lands {chosenOption.text} {effectText}";
            } else if (target.Equals("you", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"In each of your lands {chosenOption.text} {effectText}";
            } else
            {
                return $"In each of [target's] lands {chosenOption.text} {effectText}";
            }
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

        protected override void InitializeEffect()
        {
            base.InitializeEffect();

            try
            {
                effects.Add(ChooseChildEffect());
            } catch (Exception ex)
            {
                Effect defaultEffect = new DefendEffect();
                defaultEffect.InitializeEffect(UpdateContext());
                effects.Add(defaultEffect);
            }
            
        }

        protected override Context UpdateContext()
        {
            Context childContext = base.UpdateContext();
            Target newTarget = new Target();
            newTarget.targetType = Target.TargetType.Land;
            childContext.target = newTarget;

            HashSet<TargetConditions.LandConditon.LandConditions> conditions = new HashSet<TargetConditions.LandConditon.LandConditions>();
            foreach (Piece piece in chosenOption.pieces)
            {
                foreach (TargetConditions.LandConditon.LandConditions condition in TargetConditions.LandConditon.PieceImplications[piece])
                {
                    conditions.Add(condition);
                }
            }
            childContext.target.landConditions.AddRange(conditions);
            return childContext;
        }

        private Effect ChooseChildEffect()
        {
            Effect effect = (Effect)Context.effectGenerator.ChooseGeneratorOption<Effect>(effectSettings);
            effect.InitializeEffect(UpdateContext());
            return effect;
        }

        protected Effect? ChooseStrongerEffect()
        {
            InEachOfSpiritsLandsEffect strongerThis = (InEachOfSpiritsLandsEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.effects, Context.rng);

            Effect? strongerEffect = Context.effectGenerator.ChooseStrongerEffect(effectSettings, effectToStrengthen.CalculatePowerLevel());
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
            InEachOfSpiritsLandsEffect weakerThis = (InEachOfSpiritsLandsEffect)Duplicate();
            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.effects, Context.rng);

            Effect? weakerEffect = Context.effectGenerator.ChooseWeakerEffect(effectSettings, effectToWeaken.CalculatePowerLevel());
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
            InEachOfSpiritsLandsEffect strongerThis = (InEachOfSpiritsLandsEffect)Duplicate();
            Effect newEffect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (newEffect != null)
            {
                strongerThis.effects.Add(newEffect);
                return strongerThis;
            } else
            {
                return null;
            }
            
        }

        protected Effect? RemoveEffect()
        {
            InEachOfSpiritsLandsEffect weakerThis = (InEachOfSpiritsLandsEffect)Duplicate();
            if (effects.Count > 1) {
                Effect effectToRemove = Utils.ChooseRandomListElement(effects, Context.rng);
                weakerThis.effects.Remove(effectToRemove);
                return weakerThis;
            } else
            {
                return null;
            }
        }
    }
}
