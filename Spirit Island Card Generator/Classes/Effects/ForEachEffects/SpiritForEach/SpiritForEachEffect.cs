using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.ForEachEffects.LandForeach;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.ForEachEffects.SpiritForEach
{
    [SpiritEffect]
    internal class SpiritForEachEffect : Effect, IParentEffect
    {
        public override string Name => "Spirit For Each: " + chosenOption.text;
        public override Context.CardTargets TargetType => Context.CardTargets.TargetSpirit;

        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 1 + effects.Sum((eff) => eff.Complexity);

        public override Regex descriptionRegex => new Regex(@"");

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Strengthen/Weaken Effect", 30, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose different piece", 70, ChooseMoreCommonPiece, ChooseLessCommonPiece),
            new DifficultyOption("Add/Remove effect", 5, AddNewEffect, RemoveEffect),
        ];

        List<AmountEffect> effects = new List<AmountEffect>();
        protected struct ForeachOption
        {
            public int weight;
            public Piece[] pieces;
            public string text;
            public double strength;

            public bool canHaveMaxAmount = true;
            public int customEffectLevel = 0;


            public ForeachOption(int w, Piece[] pieces, string text, double strength, bool maxAmount, int customEffectLevel = 0)
            {
                weight = w;
                this.pieces = pieces;
                this.text = text;
                this.strength = strength;
                this.canHaveMaxAmount = maxAmount;
                this.customEffectLevel = customEffectLevel;
            }
        }

        protected List<ForeachOption> foreachOptions = new List<ForeachOption>() {
            new ForeachOption(30, [], "card [target] has in play", 3, false, 1),
            new ForeachOption(2, [], "0 cost card [target] has in play", 2, false, 3),
            new ForeachOption(5, [], "1 cost card [target] has in play", 1.5, false, 3),
            new ForeachOption(2, [], "slow card [target] has in play", 2, false, 3),
            new ForeachOption(2, [], "fast card [target] has in play", 2, false, 3),
            new ForeachOption(2, [], "unique power card [target] has in play", 2.2, false, 3),
            new ForeachOption(5, [], "type of card [target] has in play (minor, major, unique)", 2.2, false, 3),
            new ForeachOption(7, [], "{sacred-site} [target] has", 2.5, false, 3),
            new ForeachOption(2, [], "{mountain} [target] has {presence} in", 2.5, true, 3),
            new ForeachOption(2, [], "{jungle} [target] has {presence} in", 2.5, true, 3),
            new ForeachOption(2, [], "{wetland} [target] has {presence} in", 2.5, true, 3),
            new ForeachOption(2, [], "{sand} [target] has {presence} in", 2.5, true, 3),
            new ForeachOption(5, [], "{destroyed-presence} they have", 2.5, true, 3),
        };

        protected ForeachOption chosenOption;

        public override double CalculatePowerLevel()
        {
            return chosenOption.strength * effects.Sum((eff) => eff.CalculatePowerLevel());
        }

        public override IPowerLevel Duplicate()
        {
            SpiritForEachEffect effect = new SpiritForEachEffect();
            effect.Context = Context?.Duplicate();
            effect.effects = new List<AmountEffect>(effects);
            effect.chosenOption = new ForeachOption(chosenOption.weight, chosenOption.pieces, chosenOption.text, chosenOption.strength, chosenOption.canHaveMaxAmount, chosenOption.customEffectLevel);
            effect.foreachOptions = new List<ForeachOption>(foreachOptions);
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            string maxText = "";
            if (chosenOption.canHaveMaxAmount)
            {
                maxText = " (count max 3)";
            }

            string perText = chosenOption.text.Replace("[target]", Context.GetTargetString(TargetType));

            if (effects.Count == 1)
            {
                return $"{effects.First().Print()} per {perText}{maxText}";
            }
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
            return $"for each {perText}: {effectText}{maxText}";
        }

        public override bool Scan(string description)
        {
            return false;
        }

        protected override void InitializeEffect()
        {
            List<Piece> inValidPieces = LandConditon.ConditionToInvalidPieces(Context.target.landConditions).ToList();
            foreach (Piece piece in inValidPieces)
            {
                if (inValidPieces.Contains(piece))
                    foreachOptions.RemoveAll((mod) => mod.pieces.Contains(piece));
            }

            Dictionary<ForeachOption, int> weights = new Dictionary<ForeachOption, int>();
            foreach (ForeachOption option in foreachOptions)
            {
                weights.Add(option, (int)(option.weight * 1000));
            }
            chosenOption = Utils.ChooseWeightedOption<ForeachOption>(weights, Context.rng);

            AmountEffect amountEffect = (AmountEffect)Context.effectGenerator.ChooseGeneratorOption<AmountEffect>(EffectGeneratorSettings.GetStandardEffectSettings(UpdateContext()));
            amountEffect.InitializeEffect(UpdateContext());
            effects.Add(amountEffect);
        }

        public IEnumerable<Effect> GetChildren()
        {
            return effects;
        }

        public void ReplaceEffect(Effect oldEffect, Effect newEffect)
        {

            effects.Remove((AmountEffect)oldEffect);
            effects.Add((AmountEffect)newEffect);
        }

        #region DificultyOptions
        private Effect? StrengthenEffect()
        {
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(effects, Context.rng);
            Effect strongerEffect = effectToStrengthen?.Strengthen();

            if (effectToStrengthen != null && strongerEffect != null && strongerEffect.GetType() == typeof(AmountEffect))
            {
                SpiritForEachEffect strongerThis = (SpiritForEachEffect)Duplicate();
                strongerThis.effects.Remove((AmountEffect)effectToStrengthen);
                strongerThis.effects.Add((AmountEffect)strongerEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? WeakenEffect()
        {
            Effect? effectToWeaken = Utils.ChooseRandomListElement(effects, Context.rng);
            Effect weakerEffect = effectToWeaken?.Weaken();

            if (effectToWeaken != null && weakerEffect != null && weakerEffect.GetType() == typeof(AmountEffect))
            {
                SpiritForEachEffect weakerThis = (SpiritForEachEffect)Duplicate();
                weakerThis.effects.Remove((AmountEffect)effectToWeaken);
                weakerThis.effects.Add((AmountEffect)weakerEffect);
                return weakerThis;
            }
            return null;
        }

        protected Effect? ChooseMoreCommonPiece()
        {
            Dictionary<ForeachOption, int> weights = new Dictionary<ForeachOption, int>();
            foreach (ForeachOption option in foreachOptions)
            {
                if (option.strength > chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            SpiritForEachEffect strongerEffect = (SpiritForEachEffect)Duplicate();
            strongerEffect.chosenOption = Utils.ChooseWeightedOption<ForeachOption>(weights, Context.rng);
            return strongerEffect;
        }

        protected Effect? ChooseLessCommonPiece()
        {
            Dictionary<ForeachOption, int> weights = new Dictionary<ForeachOption, int>();
            foreach (ForeachOption option in foreachOptions)
            {
                if (option.strength < chosenOption.strength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            SpiritForEachEffect weakerEffect = (SpiritForEachEffect)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption<ForeachOption>(weights, Context.rng);
            return weakerEffect;
        }

        protected Effect? AddNewEffect()
        {
            AmountEffect? newEffect = (AmountEffect)Context.effectGenerator.ChooseGeneratorOption<AmountEffect>(EffectGeneratorSettings.GetStandardEffectSettings(UpdateContext()));
            if (newEffect == null)
                return null;

            newEffect.InitializeEffect(UpdateContext());
            if (newEffect.CalculatePowerLevel() > 0)
            {
                SpiritForEachEffect strongerThis = (SpiritForEachEffect)Duplicate();
                strongerThis.effects.Add(newEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? RemoveEffect()
        {
            if (effects.Count <= 1)
                return null;

            AmountEffect effectToRemove = Utils.ChooseRandomListElement(effects, Context.rng);
            SpiritForEachEffect weakerThis = (SpiritForEachEffect)Duplicate();
            weakerThis.effects.Remove(effectToRemove);
            return weakerThis;
        }
        #endregion
    }
}
