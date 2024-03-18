using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    public class Card : IPowerLevel
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
        public string descrition = "";

        public string artworkDataString = "";

        public bool HasThreshold { 
            get
            {
                return !thresholdCondition.Equals("");
            } 
        }

        public string thresholdCondition = "";
        public string thresholdDescription = "";

        public bool ContainsSameEffectType(Effect otherEffect)
        {
            Type effectType = otherEffect.GetType();
            foreach (Effect effect in GetAllEffects())
            {
                if (effect.GetType() == effectType && !effect.Equals(otherEffect))
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

            //Target
            foreach(LandConditon.LandConditions landCondition in Target.landConditions)
            {
                power *= LandTypeCondition.conditions[landCondition].multiplier;
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
                    complexity += 2;
                }
            }


            complexity += effects.Count * 2;
            foreach(Effect effect in effects)
            {
                complexity += effect.Complexity;
            }

            return complexity;
        }

        public bool IsValid(Context context)
        {
            Settings settings = context.settings;
            if (CalculatePowerLevel() >= settings.TargetPowerLevel + settings.PowerLevelVariance) {
                return false;
            } else if (CalculatePowerLevel() <= settings.TargetPowerLevel - settings.PowerLevelVariance) {
                return false;
            } else if (Complexity() > settings.MaxComplexity || Complexity() < settings.MinComplexity)
            {
                return false;
            }

            if (effects.Count == 1 && !effects.First().Standalone)
            {
                //bool hasStandaloneEffect = false;
                //foreach (Effect effect in effects)
                //{
                //    if (effect.Standalone)
                //    {
                //        hasStandaloneEffect = true;
                //        break;
                //    }
                //}
                //if (!hasStandaloneEffect)
                //{
                //    return false;
                //}
                return false;
            }


            //foreach (Effect effect in effects)
            //{
            //    if (!effect.IsValid(context)) { 
            //        return false;
            //    }
            //}

            return true;
        }

        public IPowerLevel Duplicate()
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

        public List<Effect> GetAllEffects()
        {
            List<Effect> allEffects = new List<Effect>();
            foreach(Effect effect in effects)
            {
                allEffects.Add(effect);
                if (effect.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
                {
                    allEffects.AddRange(GetAllChildrenEffects((IParentEffect)effect));
                }
            }
            return allEffects;
        }

        private List<Effect> GetAllChildrenEffects(IParentEffect parent)
        {
            List<Effect> children = new List<Effect>();
            foreach(Effect child in parent.GetChildren())
            {
                children.Add(child);
                if (child.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
                {
                    children.AddRange(GetAllChildrenEffects((IParentEffect)child));
                }
            }
            return children;
        }
    }
}
