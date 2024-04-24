using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    internal class IncompatableEffectsFixer : IValidFixer
    {
        Effect effect;
        List<Effect> IncompatableEffects = new List<Effect>();

        public IncompatableEffectsFixer(Effect effect)
        {
            this.effect = effect;
            IncompatableEffects.Add(effect);
            foreach(Effect potentialEffect in effect.GetSameContext().card.GetAllEffects())
            {
                if (effect.IncompatibleEffects.Contains(potentialEffect.GetType()))
                {
                    IncompatableEffects.Add(potentialEffect);
                }
            }
        }

        public FixerResult Fix()
        {
            Effect? effectToRemove = Utils.ChooseRandomListElement<Effect>(IncompatableEffects, effect.GetSameContext().rng);
            return new FixerResult(FixerResult.FixResult.RemoveEffect, effectToRemove);
        }
    }
}
