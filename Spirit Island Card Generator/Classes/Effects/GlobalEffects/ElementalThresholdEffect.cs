using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    [SpiritEffect]
    internal class ElementalThresholdEffect : Effect, IParentEffect
    {
        public override Context.CardTargets TargetType
        {
            get
            {
                if (Context.target.SpiritTarget)
                {
                    return Context.CardTargets.TargetSpirit;
                }
                else
                {
                    return Context.CardTargets.Land;
                }
            }
        }
        public override double BaseProbability { get { return .2; } }
        public override double AdjustedProbability { get { return .2; } set { } }
        public override int Complexity
        {
            get
            {
                int complexity = 4;
                foreach (Effect effect in Effects)
                {
                    complexity += effect.Complexity;
                }
                return complexity;
            }
        }

        public override bool HasMinMaxPowerLevel => true;
        public override double MinPowerLevel => (double)Context?.settings.TargetPowerLevel/5;
        public override double MaxPowerLevel => (double)Context?.settings.TargetPowerLevel/2;

        public override int PrintOrder()
        {
            return 10;
        }

        private bool offElement = false;
        private Dictionary<ElementSet.Element, int> elements = new Dictionary<ElementSet.Element, int>();
        public List<Effect> Effects = new List<Effect>();

        public override bool Standalone { get { return false; } }
        public override bool TopLevelEffect()
        {
            return true;
        }

        public string ConditionText
        {
            get
            {
                string conditionText = "";
                foreach (ElementSet.Element el in elements.Keys)
                {
                    conditionText += $"{elements[el]}-{el},";
                }
                //Delete the last comma
                return conditionText.ToLower().Remove(conditionText.Length - 1); ;
            }
        }

        public string EffectText
        {
            get
            {
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
                return effectText;
            }
        }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(@"-If you have- ((?:\d (?:Sun|Moon|Fire|Air|Water|Earth|Plant|Animal),?\s?)+): (.+)", RegexOptions.IgnoreCase);
            }
        }

        protected override DifficultyOption[] difficultyOptions => new DifficultyOption[]
        {
            new DifficultyOption("Change elements", 20, ReduceElements, IncreaseElements),
            new DifficultyOption("Strengthen or Weaken effect", 40, StrengthenEffect, WeakenEffect),
            new DifficultyOption("Add/Remove effect", 30, NewEffect, RemoveEffect),
            new DifficultyOption("Replace with Stronger/Weaker effect", 10, ReplaceWithStrongerEffect, ReplaceWithWeakerEffect),
        };

        //Checks if this should be an option for the card generator
        public override bool IsValidGeneratorOption(Context context)
        {
            //If there's more than 5 elements it's probably some specially made elemental card
            if (context.card.ContainsSameEffectType(new ElementalThresholdEffect()) || context.card.elements.GetElements().Count >= 5 || context.card.elements.GetElements().Count <= 1)
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
            Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (effect != null)
            {
                effect.InitializeEffect(UpdateContext());
                Effects.Add(effect);
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

        public bool AcceptablePowerLevel()
        {
            double powerLevel = CalculatePowerLevel();
            return powerLevel >= MinPowerLevel && powerLevel <= MaxPowerLevel;
        }

        private double CalculateElementalThresholdDifficulty()
        {
            Dictionary<int, double> difficultyMultipliers = new Dictionary<int, double>()
            {
                {1, 0.8 },
                {2, 0.6 },
                {3, 0.4 },
                {4, 0.2 },
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
            }
            else
            {
                foreach (ElementSet.Element el in elements.Keys)
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
            return ConditionText + ": " + EffectText;
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
            foreach (ElementSet.Element element in elements.Keys)
            {
                totalElements += elements[element];
            }
            return totalElements;
        }

        public override Effect Duplicate()
        {
            ElementalThresholdEffect dupEffect = new ElementalThresholdEffect();
            dupEffect.elements = elements.ToDictionary(entry => entry.Key, entry => entry.Value);
            dupEffect.offElement = offElement;
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        protected Effect? ReduceElements()
        {
            ElementalThresholdEffect strongerThis = (ElementalThresholdEffect)Duplicate();
            if (strongerThis.offElement && CountTotalElements() >= 2 || !strongerThis.offElement && CountTotalElements() >= 3)
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, Context.rng);
                strongerThis.elements[element] -= 1;
                if (strongerThis.elements[element] <= 0)
                {
                    strongerThis.elements.Remove(element);
                    if (!strongerThis.AcceptablePowerLevel())
                        return null;
                }
            }
            return null;
        }

        protected Effect? IncreaseElements()
        {
            ElementalThresholdEffect weakerThis = (ElementalThresholdEffect)Duplicate();
            //Increase elements
            if (weakerThis.offElement && CountTotalElements() < 3 || !weakerThis.offElement && CountTotalElements() < 4)
            {
                ElementSet.Element element = Utils.ChooseRandomListElement(elements.Keys, Context.rng);
                weakerThis.elements[element] += 1;
                return weakerThis;
            }
            return null;
        }

        protected Effect? StrengthenEffect()
        {
            ElementalThresholdEffect strongerThis = (ElementalThresholdEffect)Duplicate();
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = effectToStrengthen?.Strengthen();
            if (strongerEffect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }
            return null;
        }

        protected Effect? WeakenEffect()
        {
            ElementalThresholdEffect weakerThis = (ElementalThresholdEffect)Duplicate();
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

        protected Effect? ReplaceWithStrongerEffect()
        {
            if (Effects.Count == 0)
                return null;

            ElementalThresholdEffect strongerThis = (ElementalThresholdEffect)Duplicate();
           
            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);
            Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), effectToStrengthen.CalculatePowerLevel());
            if (effect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(effect);
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        protected Effect? ReplaceWithWeakerEffect()
        {
            if (Effects.Count == 0)
                return null;
            ElementalThresholdEffect weakerThis = (ElementalThresholdEffect)Duplicate();

            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);
            Effect? effect = Context.effectGenerator.ChooseWeakerEffect(UpdateContext(), effectToWeaken.CalculatePowerLevel());
            if (effect != null && effectToWeaken != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(effect);
                if (weakerThis.AcceptablePowerLevel())
                    return weakerThis;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        protected Effect? NewEffect()
        {
            ElementalThresholdEffect strongerThis = (ElementalThresholdEffect)Duplicate();
            Effect? effect = Context.effectGenerator.ChooseStrongerEffect(UpdateContext(), 0);
            if (effect != null)
            {
                strongerThis.Effects.Add(effect);
                return strongerThis;
            }
            return null;
        }

        protected Effect? RemoveEffect()
        {
            if(Effects.Count <= 1)
            {
                return null;
            }

            ElementalThresholdEffect weakerThis = (ElementalThresholdEffect)Duplicate();
            Effect? effect = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);
            if (effect != null)
            {
                weakerThis.Effects.Remove(effect);
                return weakerThis;
            }
            return null;
        }

        public IEnumerable<Effect> GetChildren()
        {
            return Effects;
        }

        public void ReplaceEffect(Effect effect, Effect newEffect)
        {
            if (Effects.Remove(effect))
            {
                Effects.Add(newEffect);
            } else
            {
                throw new Exception("Replace called without the old effect existing");
            }
        }
    }
}
