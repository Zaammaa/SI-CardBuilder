using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    [LandEffect]
    internal class MergedPushEffect : AmountEffect, IParentEffect
    {
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override int Complexity
        {
            get
            {
                int complexity = 1;
                foreach (Effect effect in pushEffects)
                {
                    complexity += effect.Complexity;
                }
                return complexity;
            }
        }

        public override Regex descriptionRegex
        {
            get
            {
                List<string> pieceNames = new List<string>();
                foreach (Piece piece in Enum.GetValues(typeof(Piece)))
                {
                    pieceNames.Add(piece.ToString());
                }
                string allPiecesRegex = "(?:" + String.Join("|", pieceNames) + ")";
                return new Regex(@"Push (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change amounts", 40, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Make Optional/Mandatory", 20, MakeOptional, MakeMandatory),
            new DifficultyOption("Add remove option", 40, MakeOptional, MakeMandatory)
        ];

        public int MAXAmount {
            get
            {
                int maxAmount = 99;
                foreach(PushEffect effect in pushEffects)
                {
                    if (effect.max < maxAmount)
                        maxAmount = effect.max;
                }
                return maxAmount;
            }
        }
        public static int MAXEffects = 3;
        private bool _mandatory;
        public bool mandatory {
            get { return _mandatory; }
            set {
                _mandatory = value;
                foreach (Effect effect in pushEffects)
                {
                    PushEffect pushEffect = effect as PushEffect;
                    pushEffect.mandatory = value;
                }
            }
        }
        private int _amount = 1;
        [AmountValue]
        public int amount {
            get { return _amount; }
            set {
                _amount = value;
                foreach (Effect effect in pushEffects)
                {
                    PushEffect pushEffect = effect as PushEffect;
                    pushEffect.pushAmount = value;
                }
            }
        }

        public override double effectStrength => pushEffects.Max(push => push.CalculatePowerLevel());

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            {1,1},
            {2,1},
            {3,1},
            {4,1},
        };

        public List<PushEffect> pushEffects = new List<PushEffect>();

        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            //TODO: There should be some math similar to how the 'or' effect works. High differences should be ~= to the highest value. 
            double highestPower = 0;
            foreach (Effect effect in pushEffects)
            {
                double power = effect.CalculatePowerLevel();
                if (power > highestPower)
                    highestPower = power;
            }
            powerLevel += highestPower;
            powerLevel += 0.1 * (pushEffects.Count - 1);
            //powerLevel /= pushEffects.Count;
            //powerLevel *= 1.2;
            if (mandatory)
            {
                powerLevel -= 0.05;
            }
            return powerLevel;
        }

        public override IPowerLevel Duplicate()
        {
            MergedPushEffect newEffect = (MergedPushEffect)new MergedPushEffect();
            newEffect.Context = Context.Duplicate();
            newEffect.amount = amount;
            newEffect.mandatory = mandatory;
            foreach (Effect effect in pushEffects)
            {
                newEffect.pushEffects.Add((PushEffect)effect.Duplicate());
            }
            return newEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return pushEffects;
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (pushEffects.Contains(effect))
            {
                pushEffects.Remove((PushEffect)effect);
                pushEffects.Add((PushEffect)newEffect);
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }
        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders) || context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Invaders))
                return false;

            return true;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";
            string text = $"Push {mayText}{amount} ";
            List<string> pieces = new List<string>();
            foreach (Effect effect in pushEffects)
            {
                PushEffect pushEffect = (PushEffect)effect;
                pieces.Add("{" + pushEffect.Piece.ToString().ToLower() + "}");
            }
            string piecesText = String.Join("/", pieces);
            return text + piecesText;
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
            amount = Context.rng.Next(1, 3);
            mandatory = Context.rng.NextDouble() <= 0.5 ? true : false;

            int amountOfOptions = Context.rng.Next(2, 4);
            while (pushEffects.Count < amountOfOptions)
            {
                PushEffect? effect = (PushEffect?)Context.effectGenerator.ChooseGeneratorOption<PushEffect>(UpdateContext());
                if (effect != null && !pushEffects.Any((e) => { return e.GetType() == effect.GetType(); }))
                {
                    effect.InitializeEffect(UpdateContext());
                    effect.pushAmount = amount;
                    effect.mandatory = mandatory;
                    pushEffects.Add(effect);
                }
            }

        }

        #region DifficultyOptions

        public Effect? IncreaseAmount()
        {
            if (amount < MAXAmount)
            {
                MergedPushEffect newEffect = (MergedPushEffect)Duplicate();
                newEffect.amount += 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }

        public Effect? DecreaseAmount()
        {
            if (amount > 1)
            {
                MergedPushEffect newEffect = (MergedPushEffect)Duplicate();
                newEffect.amount -= 1;

                return newEffect;
            }
            else
            {
                return null;
            }
        }

        protected Effect? MakeMandatory()
        {
            if (!mandatory)
            {
                MergedPushEffect newEffect = (MergedPushEffect)Duplicate();
                newEffect.mandatory = true;

                return newEffect;
            }
            return null;
        }

        protected Effect? MakeOptional()
        {
            if (mandatory)
            {
                MergedPushEffect newEffect = (MergedPushEffect)Duplicate();
                newEffect.mandatory = false;
                return newEffect;
            }
            return null;
        }

        protected Effect? AddPushOption()
        {
            MergedPushEffect strongerThis = (MergedPushEffect)Duplicate();
            PushEffect? newPushEffect = (PushEffect?)Context?.effectGenerator.ChooseGeneratorOption<PushEffect>(UpdateContext());
            newPushEffect?.InitializeEffect(UpdateContext());
            if (newPushEffect != null)
            {
                foreach (Effect effects in strongerThis.pushEffects)
                {
                    if (effects.GetType() == newPushEffect.GetType())
                        return null;
                }
                newPushEffect.pushAmount = amount;
                newPushEffect.mandatory = mandatory;
                strongerThis.pushEffects.Add(newPushEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? RemovePushOption()
        {
            if (pushEffects.Count > 2)
            {
                MergedPushEffect weakerThis = (MergedPushEffect)Duplicate();
                PushEffect? effectToRemove = (PushEffect?)Utils.ChooseRandomListElement(weakerThis.pushEffects, Context.rng);
                if (effectToRemove != null)
                {
                    weakerThis.pushEffects.Remove(effectToRemove);
                    return weakerThis;
                }
            }
            else
            {
                //Return one of the two push effects
                PushEffect? singleEffect = (PushEffect?)Utils.ChooseRandomListElement(pushEffects, Context.rng);
                if (singleEffect != null)
                {
                    singleEffect.pushAmount = amount;
                    singleEffect.mandatory = mandatory;
                    return singleEffect;
                }
            }
            return null;
        }
        #endregion
    }
}
