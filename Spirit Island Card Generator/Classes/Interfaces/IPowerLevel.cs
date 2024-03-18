using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public interface IPowerLevel
    {
        double CalculatePowerLevel();

        //Generates a new shallow copy of the current effect. Useful for hypotheticals like "Would this effect work with the current card"
        IPowerLevel Duplicate();

        public static bool IsWithinAcceptablePowerLevel(Context context, IPowerLevel option, IPowerLevel? replacedOption)
        {
            double minPowerLevel = context.settings.TargetPowerLevel - context.settings.PowerLevelVariance;
            double maxPowerLevel = context.settings.TargetPowerLevel + context.settings.PowerLevelVariance;

            double currentPowerLevel = context.card.CalculatePowerLevel();
            double hypotheticalPowerLevelChange = option.CalculatePowerLevel();
            if (replacedOption != null)
            {
                hypotheticalPowerLevelChange -= replacedOption.CalculatePowerLevel();
            }
            double hypotheticalPowerLevel = currentPowerLevel + hypotheticalPowerLevelChange;
            if (hypotheticalPowerLevel <= maxPowerLevel && hypotheticalPowerLevel >= minPowerLevel)
                return true;
            else
                return false;
        }
    }
}
