using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    abstract class AmountMultipleEffectOption : AmountEffect
    {
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        protected EffectOption chosenOption { get; set; }

        protected List<EffectOption> _effectOptions = new List<EffectOption>();
        protected abstract List<EffectOption> EffectOptions { get; set; }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {

            }
            return match.Success;
        }

        [AmountValue]
        public int effectAmount = 1;

        public override double effectStrength => chosenOption.baseStrength;

        protected override void InitializeEffect()
        {
            EffectOptions.RemoveAll((mod) => mod.customLvl > Context.settings.CustomEffectLevel);
            List<Piece> inValidPieces = LandConditon.ConditionToInvalidPieces(Context.target.landConditions).ToList();
            foreach (Piece piece in inValidPieces)
            {
                if (inValidPieces.Contains(piece))
                    EffectOptions.RemoveAll((mod) => mod.pieces.Contains(piece));
            }

            Dictionary<EffectOption, int> weights = new Dictionary<EffectOption, int>();
            foreach (EffectOption option in EffectOptions)
            {
                weights.Add(option, (int)(option.weight * 1000));
            }
            chosenOption = Utils.ChooseWeightedOption<EffectOption>(weights, Context.rng);
        }

        public override double CalculatePowerLevel()
        {
            double baseLvl = base.CalculatePowerLevel();
            return baseLvl * chosenOption.powerMult;
        }

        public Effect? ChooseBetterOption()
        {
            Dictionary<EffectOption, int> weights = new Dictionary<EffectOption, int>();
            foreach (EffectOption option in EffectOptions)
            {
                if (option.powerMult * option.baseStrength > chosenOption.powerMult * chosenOption.baseStrength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            AmountMultipleEffectOption strongerEffect = (AmountMultipleEffectOption)Duplicate();
            strongerEffect.chosenOption = Utils.ChooseWeightedOption<EffectOption>(weights, Context.rng);
            return strongerEffect;
        }

        public Effect? ChooseWorseOption()
        {
            Dictionary<EffectOption, int> weights = new Dictionary<EffectOption, int>();
            foreach (EffectOption option in EffectOptions)
            {
                if (option.powerMult * option.baseStrength < chosenOption.powerMult * chosenOption.baseStrength)
                {
                    weights.Add(option, (int)(option.weight * 1000));
                }
            }
            if (weights.Count == 0)
                return null;

            AmountMultipleEffectOption weakerEffect = (AmountMultipleEffectOption)Duplicate();
            weakerEffect.chosenOption = Utils.ChooseWeightedOption<EffectOption>(weights, Context.rng);
            return weakerEffect;
        }
    }
}
