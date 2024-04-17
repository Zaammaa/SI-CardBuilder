using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects
{
    [LandEffect]
    internal class MergedGatherEffect : Effect, IParentEffect
    {
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get { return BaseProbability; } set { } }

        public override int Complexity
        {
            get
            {
                int complexity = 1;
                foreach(Effect effect in gatherEffects)
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
                return new Regex(@"Gather (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => [
            new DifficultyOption("Change amounts", 40, IncreaseAmount, DecreaseAmount),
            new DifficultyOption("Make Optional/Mandatory", 20, MakeOptional, MakeMandatory),
            new DifficultyOption("Add remove option", 40, MakeOptional, MakeMandatory)
        ];

        public static int MAXAmount = 3;
        public static int MAXEffects = 3;
        private bool _mandatory;
        public bool mandatory
        {
            get { return _mandatory; }
            set
            {
                _mandatory = value;
                foreach (Effect effect in gatherEffects)
                {
                    GatherEffect gatherEffect = effect as GatherEffect;
                    gatherEffect.mandatory = value;
                }
            }
        }
        private int _amount = 1;
        public int amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                foreach (Effect effect in gatherEffects)
                {
                    GatherEffect gatherEffect = effect as GatherEffect;
                    gatherEffect.gatherAmount = value;
                }
            }
        }
        public List<Effect> gatherEffects = new List<Effect>();

        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            //TODO: There should be some math similar to how the 'or' effect works. High differences should be ~= to the highest value. 
            double highestPower = 0;
            
            foreach(Effect effect in gatherEffects)
            {
                double power = effect.CalculatePowerLevel();
                if (power > highestPower)
                    highestPower = power;
            }
            powerLevel += highestPower;
            powerLevel += 0.1 * (gatherEffects.Count - 1);
            //powerLevel /= gatherEffects.Count;
            //powerLevel *= 1.2;
            if (mandatory)
            {
                powerLevel -= 0.05;
            }
            return powerLevel;
        }

        public override IPowerLevel Duplicate()
        {
            MergedGatherEffect newEffect = (MergedGatherEffect)new MergedGatherEffect();
            newEffect.Context = Context.Duplicate();
            newEffect.amount = amount;
            newEffect.mandatory = mandatory;
            foreach (Effect effect in gatherEffects)
            {
                newEffect.gatherEffects.Add((Effect)effect.Duplicate());
            }
            return newEffect;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return gatherEffects;
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (gatherEffects.Remove(effect))
            {
                gatherEffects.Add(newEffect);
            }
            else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";
            string text = $"Gather {mayText}{amount} ";
            List<string> pieces = new List<string>();
            foreach (Effect effect in gatherEffects)
            {
                GatherEffect gatherEffect = (GatherEffect)effect;
                pieces.Add("{"+gatherEffect.Piece.ToString().ToLower() + "}");
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
            while (gatherEffects.Count < amountOfOptions)
            {
                GatherEffect? effect = (GatherEffect?)Context.effectGenerator.ChooseGeneratorOption<GatherEffect>(UpdateContext());
                if (effect != null && !gatherEffects.Any((e) => { return e.GetType() == effect.GetType(); }))
                {
                    effect.InitializeEffect(UpdateContext());
                    effect.gatherAmount = amount;
                    effect.mandatory = mandatory;
                    gatherEffects.Add(effect);
                }
            }

        }

        #region DifficultyOptions

        public Effect? IncreaseAmount()
        {
            if (amount < MAXAmount)
            {
                MergedGatherEffect newEffect = (MergedGatherEffect)Duplicate();
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
                MergedGatherEffect newEffect = (MergedGatherEffect)Duplicate();
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
                MergedGatherEffect newEffect = (MergedGatherEffect)Duplicate();
                newEffect.mandatory = true;
                return newEffect;
            }
            return null;
        }

        protected Effect? MakeOptional()
        {
            if (mandatory)
            {
                MergedGatherEffect newEffect = (MergedGatherEffect)Duplicate();
                newEffect.mandatory = false;
                return newEffect;
            }
            return null;
        }

        protected Effect? AddGatherOption() 
        {
            MergedGatherEffect strongerThis = (MergedGatherEffect)Duplicate();
            GatherEffect? newGatherEffect = (GatherEffect?)Context?.effectGenerator.ChooseGeneratorOption<GatherEffect>(UpdateContext());
            newGatherEffect?.InitializeEffect(UpdateContext());
            if (newGatherEffect != null)
            {
                foreach(Effect effects in strongerThis.gatherEffects)
                {
                    if (effects.GetType() == newGatherEffect.GetType())
                        return null;
                }
                strongerThis.gatherEffects.Add(newGatherEffect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? RemoveGatherOption()
        {
            if (gatherEffects.Count > 2)
            {
                MergedGatherEffect weakerThis = (MergedGatherEffect)Duplicate();
                GatherEffect? effectToRemove = (GatherEffect?)Utils.ChooseRandomListElement(weakerThis.gatherEffects, Context.rng);
                if (effectToRemove != null)
                {
                    weakerThis.gatherEffects.Remove(effectToRemove);
                    return weakerThis;
                }
            } else
            {
                //Return one of the two gather effects
                GatherEffect? singleEffect = (GatherEffect?)Utils.ChooseRandomListElement(gatherEffects, Context.rng);
                if (singleEffect != null)
                {
                    singleEffect.gatherAmount = amount;
                    singleEffect.mandatory = mandatory;
                    return singleEffect;
                }
            }
            return null;
        }
        #endregion
    }
}
