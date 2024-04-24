using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Interfaces;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    /// <summary>
    /// Tells the card generator to do remove the effect
    /// </summary>
    internal class InvalidEffectFixer : IValidFixer
    {
        Effect effectToRemove;

        public InvalidEffectFixer(Effect effect)
        {
            effectToRemove = effect;
        }
        public FixerResult Fix()
        {
            return new FixerResult(FixerResult.FixResult.RemoveEffect, effectToRemove);
        }
    }
}
