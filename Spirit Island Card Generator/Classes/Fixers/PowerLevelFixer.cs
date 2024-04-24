using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    internal class PowerLevelFixer : IValidFixer
    {
        Effect effect;

        public PowerLevelFixer(Effect effect)
        {
            this.effect = effect;
        }

        public FixerResult Fix()
        {
            Effect? newEffect = (Effect?)effect.Duplicate();
            while (newEffect != null && newEffect.CalculatePowerLevel() > newEffect.MaxPowerLevel)
            {
                newEffect = newEffect.Weaken();
            }
            while (newEffect != null && newEffect.CalculatePowerLevel() < newEffect.MinPowerLevel)
            {
                newEffect = newEffect.Strengthen();
            }
            if (newEffect != null)
            {
                return new FixerResult(FixerResult.FixResult.UpdateEffect, newEffect);
            } else
            {
                return new FixerResult(FixerResult.FixResult.FixFailed, null);
            }
            
        }
    }
}
