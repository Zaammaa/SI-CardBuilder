using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Attributes
{
    /// <summary>
    /// Toggles custom effects on or off depending on the settings
    /// There are 5 level of custom effects:
    ///     1: effects that are used in the game somewhat frequently, but don't appear on minor powers normally. Such downgrade or InOriginLand
    ///     2: effects that are in the game, but are narrow in scope. Such as Take a power card, or granting specific elements
    ///     3: effects that are not in the game, but seem like a reasonable step, such as a threshold that checks how many destroyed presence a Spirit has, or Discarding a card as a cost
    ///     4: effects that are not in the game and are new ideas I had, but do not disrupt the flow of the game much
    ///     5: effects that even I'm not sure are good ideas most of the time
    /// </summary>
    internal class CustomEffectAttribute : Attribute
    {
        public int level = 1;
        public CustomEffectAttribute(int level)
        {
            this.level = level;
        }
    }
}
