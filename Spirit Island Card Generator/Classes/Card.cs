using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    public class Card : IPowerPowerLevel
    {
        public enum CardTypes
        {
            Minor,
            Major,
            Unique
        }
        public CardTypes CardType { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public bool Fast { get; set; }
        public Range Range { get; set; }
        public Target Target { get; set; }

        public ElementSet elements { get; set; }
        public List<Effect> effects = new List<Effect>();
        public string descrition = "Not Set";

        public string artworkDataString = "";

        public bool ContainsEffect(Type effectType)
        {
            foreach (Effect effect in effects)
            {
                if (effect.GetType() == effectType)
                {
                    return true;
                }
            }
            return false;
        }

        public double CalculatePowerLevel()
        {
            double power = 0;
            //Cost
            power -= Cost * 0.5;
            //Elements
            if (elements.GetElements().Count == 2)
            {
                power -= 0.1;
            } else if (elements.GetElements().Count == 4)
            {
                power += 0.1;
            }

            //Speed
            if (Fast)
            {
                power += 0.15;
            }
            //Target
            //TODO: Different conditions should have different power reductions
            power -= 0.15 * Target.landConditions.Count;
            //Range
            if (Range.range == 0)
            {
                power -= 0.2;
            } else if (Range.range == 2) {
                power += 0.1;
            } else if (Range.range >= 3)
            {
                power += 0.15;
            }

            if (Range.sacredSite)
            {
                power -= 0.15;
            }

            if (Range.SourceLand.HasValue) {
                power -= 0.2;
            }
            //Effects
            foreach (Effect effect in effects)
            {
                power += effect.CalculatePowerLevel();
            }
            return power;
        }

        public int Complexity()
        {
            int complexity = 0;

            if (Target.SpiritTarget)
            {
                complexity += 2;
            } else
            {
                complexity += Target.landConditions.Count * 3;

                if (Range.sacredSite)
                {
                    complexity += 1;
                }
                if (Range.SourceLand.HasValue) {
                    complexity += 3;
                }
            }


            complexity += effects.Count * 2;
            foreach(Effect effect in effects)
            {
                complexity += effect.Complexity;
            }

            return complexity;
        }

        public bool IsValid(Settings settings)
        {
            if (CalculatePowerLevel() >= settings.TargetPowerLevel + settings.PowerLevelVariance) {
                return false;
            } else if (CalculatePowerLevel() <= settings.TargetPowerLevel - settings.PowerLevelVariance) {
                return false;
            } else if (Complexity() > settings.MaxComplexity || Complexity() < settings.MinComplexity)
            {
                return false;
            }

            bool hasStandaloneEffect = false;
            foreach(Effect effect in effects)
            {
                if (effect.Standalone)
                {
                    hasStandaloneEffect = true;
                    break;
                }
            }
            if (!hasStandaloneEffect)
            {
                return false;
            }

            foreach (Effect effect in effects)
            {
                if (!effect.IsValid(this, settings)) { 
                    return false;
                }
            }

            return true;
        }

        public IPowerPowerLevel Duplicate()
        {
            Card newCard = new Card();
            newCard.CardType = CardType;
            newCard.descrition = descrition;
            //Name
            newCard.Name = Name;
            //Image
            newCard.artworkDataString = artworkDataString;
            //Cost
            newCard.Cost = Cost;
            //Elements
            List<ElementSet.Element> el = new List<ElementSet.Element>(elements.GetElements());
            newCard.elements = new ElementSet(el);
            //Speed
            newCard.Fast = Fast;
            //Target
            newCard.Target = new Target();
            newCard.Target.SpiritTarget = Target.SpiritTarget;
            newCard.Target.landConditions = new List<TargetConditions.LandConditon.LandConditions>(Target.landConditions);
            //Range
            newCard.Range = new Range(Range.sacredSite, Range.range, Range.SourceLand);
            //Effects
            foreach(Effect effect in effects)
            {
                newCard.effects.Add((Effect)effect.Duplicate());
            }
            return newCard;
        }
    }
}
