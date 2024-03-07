using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    internal class LandElementalThresholdEffect : LandEffect
    {
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return .2; } set { } }
        public override int Complexity { get { return 6; } }

        private bool offElement = false;
        private Dictionary<ElementSet.Element, int> elements = new Dictionary<ElementSet.Element, int>();
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return true; } }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(@"-If you have- ((?:\d (?:Sun|Moon|Fire|Air|Water|Earth|Plant|Animal),?\s?)+): (.+)", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            //If there's more than 5 elements it's probably some specially made elemental card
            if (card.ContainsEffect(typeof(LandElementalThresholdEffect)) && card.elements.GetElements().Count <= 5 && card.elements.GetElements().Count > 0)
                return false;

            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            ChooseElements(card, settings);

            //Choose Effect
            ChooseRandomEffect(card, settings);
        }

        private void ChooseElements(Card card, Settings settings)
        {
            ElementSet set = card.elements;
            List<ElementSet.Element> elementOptions = new List<ElementSet.Element>();
            int roll = (int)(settings.rng.NextDouble() * 100) + 1;
            if (roll <= 20 && card.CardType == Card.CardTypes.Minor)
            {
                //Choose 1 off element type
                offElement = true;
                foreach (ElementSet.Element el in ElementSet.GetAllElements())
                {
                    if (!set.GetElements().Contains(el))
                    {
                        elementOptions.Add(el);
                    }
                }

            }
            else
            {
                //Choose 1 on element type
                foreach (ElementSet.Element el in ElementSet.GetAllElements())
                {
                    if (set.GetElements().Contains(el))
                    {
                        elementOptions.Add(el);
                    }
                }
            }

            if (card.CardType == Card.CardTypes.Minor)
            {
                //Only 1 element type for minors?
                int amount;
                if (offElement)
                {
                    amount = settings.rng.Next(1, 3);
                }
                else
                {
                    amount = settings.rng.Next(2, 4);
                }
                roll = settings.rng.Next(0, elementOptions.Count);
                elements.Add(elementOptions.ToArray()[roll], amount);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected void ChooseRandomEffect(Card card, Settings settings)
        {
            List<LandEffect> effects = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
            List<LandEffect> validEffects = new List<LandEffect>();
            foreach (LandEffect landEffect in effects)
            {
                if (landEffect.GetType() == typeof(TargetLandConditionEffect) || landEffect.GetType() == typeof(LandElementalThresholdEffect))
                    continue;

                landEffect.InitializeEffect(card, settings);
                if (landEffect.IsValid(card, settings))
                {
                    validEffects.Add(landEffect);
                }
            }
            Effects.Add((LandEffect)ChooseEffect(card, settings, validEffects));
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel() * CalculateElementalThresholdDifficulty();
            }
            return powerLevel;
        }

        private double CalculateElementalThresholdDifficulty()
        {
            Dictionary<int, double> difficultyMultipliers = new Dictionary<int, double>()
            {
                {2, 0.8 },
                {3, 0.6 },
                {4, 0.3 },
            };
            double multiplier = 1;
            if (offElement)
            {
                foreach (ElementSet.Element el in elements.Keys)
                {
                    int count = elements[el];
                    if (difficultyMultipliers[count + 1] < multiplier)
                    {
                        multiplier = difficultyMultipliers[count + 1];
                    }
                }
            } else
            {
                foreach(ElementSet.Element el in elements.Keys)
                {
                    int count = elements[el];
                    if (difficultyMultipliers[count] < multiplier)
                    {
                        multiplier = difficultyMultipliers[count];
                    }
                }
            }
            return multiplier;
        }

        public override string Print()
        {
            string conditionText = "";
            foreach(ElementSet.Element el in elements.Keys)
            {
                conditionText += $"{elements[el]}-{el},";
            }
            string effectText = "";
            bool first = true;
            foreach (Effect effect in Effects)
            {
                if (!first)
                {
                    effectText += " and ";
                }
                else
                {
                    first = false;
                }
                effectText += effect.Print();
            }

            return conditionText + ": " + effectText;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                //TODO: make the scan work
            }
            return match.Success;
        }

        private int CountTotalElements()
        {
            int totalElements = 0;
            foreach(ElementSet.Element element in elements.Keys)
            {
                totalElements += elements[element];
            }
            return totalElements;
        }

        public override Effect Duplicate()
        {
            LandElementalThresholdEffect dupEffect = new LandElementalThresholdEffect();
            dupEffect.elements = elements.ToDictionary(entry => entry.Key, entry => entry.Value);
            dupEffect.offElement = offElement;
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            LandElementalThresholdEffect strongerThis = (LandElementalThresholdEffect)Duplicate();
            int roll = settings.rng.Next(0, 100);
            //Reduce elements
            if (roll <= 15 && (strongerThis.offElement && CountTotalElements() >= 2 || !strongerThis.offElement && CountTotalElements() >= 3))
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, settings.rng);
                strongerThis.elements[element] -= 1;
                if (strongerThis.elements[element] <= 0)
                {
                    strongerThis.elements.Remove(element);
                }
            }
            //Strengthen effect
            else if (roll <= 90)
            {
                Effect effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, settings.rng);

                Effect strongerEffect = effectToStrengthen.Strengthen(card, settings);
                if (strongerEffect != null)
                {
                    strongerThis.Effects.Remove(effectToStrengthen);
                    strongerThis.Effects.Add(strongerEffect);
                    return strongerThis;
                }
                else
                {
                    //Try to add a different, stronger effect
                    List<LandEffect> options = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
                    options.ForEach((LandEffect effect) =>
                    {
                        effect.InitializeEffect(card, settings);
                    });
                    Effect? effect = ChooseStrongerEffect(card, settings, strongerThis.CalculatePowerLevel(), options);
                    if (effect != null)
                    {
                        strongerThis.Effects.Remove(effectToStrengthen);
                        strongerThis.Effects.Add(effect);
                        return strongerThis;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            //Add another effect
            else
            {
                List<LandEffect> options = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
                options.ForEach((LandEffect effect) =>
                {
                    effect.InitializeEffect(card, settings);
                });
                Effect? effect = (Effect?)ChooseEffect(card, settings, options);
                if (effect != null)
                {
                    strongerThis.Effects.Add(effect);
                    return strongerThis;
                }
                else
                {
                    return null;
                }
            }
            return strongerThis;

        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            LandElementalThresholdEffect weakerThis = (LandElementalThresholdEffect)Duplicate();
            int roll = settings.rng.Next(0, 100);
            //Increase elements
            if (roll <= 15 && (weakerThis.offElement && CountTotalElements() <= 3 || !weakerThis.offElement && CountTotalElements() <= 4))
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, settings.rng);
                weakerThis.elements[element] += 1;
            }
            //Weaken effect
            else if (roll <= 90)
            {
                Effect effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, settings.rng);

                Effect weakerEffect = effectToWeaken.Weaken(card, settings);
                if (weakerEffect != null)
                {
                    weakerThis.Effects.Remove(effectToWeaken);
                    weakerThis.Effects.Add(weakerEffect);
                    return weakerThis;
                }
                else
                {
                    return null;
                }
            }
            //Choose a different, weaker effect
            else
            {
                List<LandEffect> options = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
                options.ForEach((LandEffect effect) =>
                {
                    effect.InitializeEffect(card, settings);
                });
                Effect? effect = ChooseWeakerEffect(card, settings, CalculatePowerLevel(), options);
                if (effect != null)
                {
                    weakerThis.Effects.Remove(Utils.ChooseRandomListElement(weakerThis.Effects, settings.rng));
                    weakerThis.Effects.Add(effect);
                    return weakerThis;
                } else
                {
                    return null;
                }
            }
            return weakerThis;
        }
    }
}
