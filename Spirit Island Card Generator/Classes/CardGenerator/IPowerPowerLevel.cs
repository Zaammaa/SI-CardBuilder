using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public interface IPowerPowerLevel
    {
        double CalculatePowerLevel();

        //Generates a new shallow copy of the current effect. Useful for hypotheticals like "Would this effect work with the current card"
        IPowerPowerLevel Duplicate();

        public static bool IsWithinAcceptablePowerLevel(Card card, Settings settings, IPowerPowerLevel option, IPowerPowerLevel? replacedOption)
        {
            double minPowerLevel = settings.TargetPowerLevel - settings.PowerLevelVariance;
            double maxPowerLevel = settings.TargetPowerLevel + settings.PowerLevelVariance;

            double currentPowerLevel = card.CalculatePowerLevel();
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
