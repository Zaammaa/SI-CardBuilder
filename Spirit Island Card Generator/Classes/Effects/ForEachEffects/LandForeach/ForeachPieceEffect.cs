using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DowngradeEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.ForEachEffects.LandForeach
{
    [LandEffect]
    internal class ForeachPieceEffect : Effect, IParentEffect
    {
        public override string Name => "For Each Piece: " + chosenOption.text;
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 1 + effects.Sum((eff) => eff.Complexity);

        public override Regex descriptionRegex => new Regex(@"");

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Strengthen/Weaken Effect", 5, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Choose different piece", 85, ChooseMoreCommonPiece, ChooseLessCommonPiece),
            new DifficultyOption("Add/Remove effect", 5, AddNewEffect, RemoveEffect),
        ];
        //Certain effects need caps or it can get absurd. For example if you had a fear per dahan effect, you could stack a ton of dahan in a land and keep playing the card there for fear.
        protected Type[] cappedEffects => [
            typeof(FearEffect),
            typeof(AddEffect),
            typeof(GatherEffect)
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
            new ForeachOption(50, [Piece.Dahan], "{dahan}", 2.8, true),
            new ForeachOption(25, [Piece.Beast], "{beast}", 2, true),
            new ForeachOption(15, [Piece.Blight], "{blight}", 1.5, true),
            new ForeachOption(10, [Piece.Explorer, Piece.Town, Piece.City, Piece.Invader], "type of Invader present", 2, false),
            new ForeachOption(5, [], "your {sacred-site} in/adjacent to target land", 1.6, false, 2),
            new ForeachOption(5, [], "your {presence} in/adjacent to target land", 2.5, true, 3),
            new ForeachOption(5, [], "terror level", 1.3, false, 3),
            new ForeachOption(3, [], "damaged Invader", 1.3, true, 3),
        };

        protected ForeachOption chosenOption;

        public override double CalculatePowerLevel()
        {
            return chosenOption.strength * effects.Sum((eff) => eff.CalculatePowerLevel());
        }

        public override IPowerLevel Duplicate()
        {
            ForeachPieceEffect effect = new ForeachPieceEffect();
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
            if (chosenOption.canHaveMaxAmount && effects.Any(eff => 
            cappedEffects.Contains(eff.GetType()) || 
            cappedEffects.Any(e => eff.GetType().IsSubclassOf(e))))
            {
                maxText = " (count max 3)";
            }

            if (effects.Count == 1)
            {
                return $"{effects.First().Print()} per {chosenOption.text}{maxText}";
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
            return $"for each {chosenOption.text}: {effectText}{maxText}";
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
                ForeachPieceEffect strongerThis = (ForeachPieceEffect)Duplicate();
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
                ForeachPieceEffect weakerThis = (ForeachPieceEffect)Duplicate();
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

            ForeachPieceEffect strongerEffect = (ForeachPieceEffect)Duplicate();
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

            ForeachPieceEffect weakerEffect = (ForeachPieceEffect)Duplicate();
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
                ForeachPieceEffect strongerThis = (ForeachPieceEffect)Duplicate();
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
            ForeachPieceEffect weakerThis = (ForeachPieceEffect)Duplicate();
            weakerThis.effects.Remove(effectToRemove);
            return weakerThis;
        }
        #endregion
    }
}
