using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    public class LinkedEffectFixer : IValidFixer
    {
        Effect effect;
        List<Effect> MissingLinkedEffects = new List<Effect>();

        public LinkedEffectFixer(Effect effect)
        {
            this.effect = effect;
            MissingLinkedEffects = new List<Effect>(effect.LinkedEffects);
            foreach (Effect potentialEffect in effect.GetSameContext().card.GetAllEffects())
            {
                if (effect.LinkedEffects.Contains(potentialEffect))
                {
                    MissingLinkedEffects.Remove(potentialEffect);
                }
            }
        }

        public FixerResult Fix()
        {
            if (MissingLinkedEffects.Count == 0)
            {
                return new FixerResult(FixerResult.FixResult.FixFailed, null);
            } else
            {
                return new FixerResult(FixerResult.FixResult.RemoveEffect, effect);
            }
        }
    }
}
