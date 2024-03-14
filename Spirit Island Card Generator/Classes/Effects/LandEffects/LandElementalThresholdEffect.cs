using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
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
    [LandEffect]
    internal class LandElementalThresholdEffect : Effect
    {
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return .2; } set { } }
        public override int Complexity { 
            get {
                int complexity = 4;
                foreach (Effect effect in Effects)
                {
                    complexity += effect.Complexity;
                }
                return complexity;
            } 
        }

        private bool offElement = false;
        private Dictionary<ElementSet.Element, int> elements = new Dictionary<ElementSet.Element, int>();
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }
        public override bool TopLevelEffect() { 
            return true; 
        }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(@"-If you have- ((?:\d (?:Sun|Moon|Fire|Air|Water|Earth|Plant|Animal),?\s?)+): (.+)", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            //If there's more than 5 elements it's probably some specially made elemental card
            if (context.card.ContainsSameEffectType(new LandElementalThresholdEffect()) || context.card.elements.GetElements().Count >= 5 || context.card.elements.GetElements().Count <= 1)
                return false;

            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            ChooseElements();

            //Choose Effect
            ChooseRandomEffect();
        }

        private void ChooseElements()
        {
            ElementSet set = Context.card.elements;
            List<ElementSet.Element> elementOptions = new List<ElementSet.Element>();
            int roll = (int)(Context.settings.rng.NextDouble() * 100) + 1;
            if (roll <= 20 && Context.card.CardType == Card.CardTypes.Minor)
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

            if (Context.card.CardType == Card.CardTypes.Minor)
            {
                //Only 1 element type for minors?
                int amount;
                if (offElement)
                {
                    amount = Context.settings.rng.Next(1, 3);
                }
                else
                {
                    amount = Context.settings.rng.Next(2, 4);
                }
                roll = Context.settings.rng.Next(0, elementOptions.Count);
                elements.Add(elementOptions.ToArray()[roll], amount);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected void ChooseRandomEffect()
        {
            //List<LandEffect> effects = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
            //List<LandEffect> validEffects = new List<LandEffect>();
            //foreach (LandEffect landEffect in effects)
            //{
            //    if (landEffect.GetType() == typeof(TargetLandConditionEffect) || landEffect.GetType() == typeof(LandElementalThresholdEffect))
            //        continue;
            //    if (landEffect.IsValid(Context))
            //    {
            //        landEffect.InitializeEffect(UpdateContext());
            //        validEffects.Add(landEffect);
            //    }
            //}
            Effect? landEffect = (Effect?)Context.effectGenerator.ChooseStrongerEffect(UpdateContext(),0);
            if (landEffect != null)
            {
                landEffect.InitializeEffect(UpdateContext());
                Effects.Add(landEffect);
            }   
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
                {1, 0.9 },
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
                    if (difficultyMultipliers[count] < multiplier)
                    {
                        multiplier = difficultyMultipliers[count];
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
            dupEffect.Context = Context;
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        public override Effect? Strengthen()
        {
            LandElementalThresholdEffect strongerThis = (LandElementalThresholdEffect)Duplicate();
            int roll = Context.rng.Next(0, 100);
            //Reduce elements
            if (roll <= 15 && (strongerThis.offElement && CountTotalElements() >= 2 || !strongerThis.offElement && CountTotalElements() >= 3))
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, Context.rng);
                strongerThis.elements[element] -= 1;
                if (strongerThis.elements[element] <= 0)
                {
                    strongerThis.elements.Remove(element);
                }
            }
            //Strengthen effect
            else if (roll <= 90)
            {
                Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

                Effect? strongerEffect = effectToStrengthen?.Strengthen();
                if (strongerEffect != null && effectToStrengthen != null)
                {
                    strongerThis.Effects.Remove(effectToStrengthen);
                    strongerThis.Effects.Add(strongerEffect);
                    return strongerThis;
                }
                else
                {
                    //Try to add a different, stronger effect
                    //List<LandEffect> options = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
                    //options.ForEach((LandEffect effect) =>
                    //{
                    //    effect.InitializeEffect(UpdateContext());
                    //});
                    Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), strongerThis.CalculatePowerLevel());
                    if (effect != null && effectToStrengthen != null)
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
                List<Effect> options = ReflectionManager.GetInstanciatedSubClasses<Effect>();

                Effect? effect = (Effect?)Context.effectGenerator.ChooseGeneratorOption<Effect>(new LandEffectAttribute(),UpdateContext());
                effect?.InitializeEffect(UpdateContext());

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

        public override Effect? Weaken()
        {
            LandElementalThresholdEffect weakerThis = (LandElementalThresholdEffect)Duplicate();
            int roll = Context.rng.Next(0, 100);
            //Increase elements
            if (roll <= 15 && (weakerThis.offElement && CountTotalElements() <= 3 || !weakerThis.offElement && CountTotalElements() < 4))
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, Context.rng);
                weakerThis.elements[element] += 1;
            }
            //Weaken effect
            else if (roll <= 90)
            {
                Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

                Effect? weakerEffect = effectToWeaken?.Weaken();
                if (weakerEffect != null && effectToWeaken != null)
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
                //List<LandEffect> options = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
                //options.ForEach((LandEffect effect) =>
                //{
                //    effect.InitializeEffect(Context);
                //});
                Effect? effect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), CalculatePowerLevel());
                if (effect != null && weakerThis.Effects.Count > 0)
                {
                    weakerThis.Effects.Remove(Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng));
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
