using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.Lands;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    internal class CardGenerator
    {
        //Used to select elements if exact elements is not set in settings
        public Dictionary<Element, int> elementPool = new Dictionary<Element, int>();

        public enum EffectTemplate
        {
            Standard,
            Threshold,
            Or,
            YouMayDoBoth,
            YouMayDoLikewise
        }

        public CardGenerator()
        {
            SetupElementPool();
        }

        public Card GenerateMinorCard(Settings settings)
        {
            Card card = new Card();
            card.CardType = Card.CardTypes.Minor;
            //Step 1: Elements
            card.elements = ChooseElements(card, settings);
            //Step 2: Speed/Cost/Target/Range
            ChooseSpeed(card, settings.rng);
            ChooseCost(card, settings.rng);
            card.Target = ChooseTarget(card, settings.rng);
            card.Range = ChooseRange(card, settings.rng);
            //Step 3: Template
            //TODO use other templates
            EffectTemplate template = EffectTemplate.Standard;
            //Step 4: Effects
            
            do
            {
                card.effects.Add(ChooseRandomEffect(card, settings));
            } while (card.CalculatePowerLevel() <= settings.TargetPowerLevel - settings.PowerLevelVariance);
            //Step 5: Refine Balance
            while (!card.IsValid(settings))
            {
                if (card.Complexity() > settings.MaxComplexity)
                {
                    //TODO, this should potentially touch targeting and range requirements?
                    Effect? effect = Utils.ChooseRandomListElement(card.effects, settings.rng);
                    card.effects.Remove(effect);
                } else if (card.Complexity() < settings.MinComplexity) {
                    //TODO, this should potentially touch targeting and range requirements?
                    card.effects.Add(ChooseRandomEffect(card, settings));
                } else if (card.CalculatePowerLevel() >= settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    Effect? effect = Utils.ChooseRandomListElement(card.effects, settings.rng);
                    Effect? newEffect = effect?.Weaken(card, settings);
                    if (newEffect != null && effect != null && newEffect.CalculatePowerLevel() < effect.CalculatePowerLevel())
                    {
                        //TODO: Keep order?
                        card.effects.Remove(effect);
                        card.effects.Add(newEffect);
                    } else
                    {
                        card.effects.Remove(effect);
                    }
                } else if (card.CalculatePowerLevel() <= settings.TargetPowerLevel - settings.PowerLevelVariance)
                {
                    Effect? effect = Utils.ChooseRandomListElement(card.effects, settings.rng);
                    Effect? newEffect = effect?.Strengthen(card, settings);
                    if (newEffect != null && effect != null && newEffect.CalculatePowerLevel() > effect.CalculatePowerLevel())
                    {
                        //TODO: Keep order?
                        card.effects.Remove(effect);
                        card.effects.Add(newEffect);
                    }
                    else
                    {
                        card.effects.Add(ChooseRandomEffect(card, settings));
                    }
                } else
                {
                    //TODO, this should potentially touch targeting and range requirements?
                    Effect? effect = Utils.ChooseRandomListElement(card.effects, settings.rng);
                    card.effects.Remove(effect);
                }

                if (card.ContainsEffect(typeof(LandElementalThresholdEffect)))
                {
                    int x = 0;
                }
            }
            //Step 6: Name
            //Step 7: Art

            return card;
        }
        #region elements
        private void SetupElementPool()
        {
            foreach(Element el in  Enum.GetValues(typeof(Element))) {
                if (el == Element.None) continue;

                if (!elementPool.ContainsKey(el))
                    elementPool.Add(el, 38);
                else
                    elementPool[el] = 38;
            }
        }

        private ElementSet ChooseElements(Card card, Settings settings)
        {
            int roll = (int)(settings.rng.NextDouble() * 100);
            List<Element> elements = new List<Element>();

            //2 elements
            elements.Add(PullRandomElement(settings.rng, elements));
            elements.Add(PullRandomElement(settings.rng, elements));
            if (roll >= 11)
            {
                //3 elements
                elements.Add(PullRandomElement(settings.rng, elements));
            } 
            if (roll >= 79)
            {
                //4 elements
                elements.Add(PullRandomElement(settings.rng, elements));
            }
            return new ElementSet(elements);
        }

        private Element PullRandomElement(Random rng, List<Element> previouslyChosenElements)
        {
            Dictionary<Element, int> unusedElements = elementPool.ToDictionary(entry => entry.Key, entry => entry.Value);
            foreach(Element oldEl in previouslyChosenElements)
            {
                unusedElements[oldEl] = 0;
            }
            Element? el = Utils.ChooseWeightedOption<Element>(unusedElements, rng);
            if (el == Element.None)
            {
                Log.Information("Ran out of elements. Refilling pool");
                //Just refill the pool
                SetupElementPool();
                return PullRandomElement(rng, previouslyChosenElements);
            } else
            {
                elementPool[(Element)el] -= 1;
                return (Element)el;
            }
        }
        #endregion
        #region speed/cost/target/range

        private void ChooseSpeed(Card card, Random rng)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                int roll = (int)(rng.NextDouble()*100) + 1;
                if (roll <= 48)
                {
                    card.Fast = false;
                } else
                {
                    card.Fast = true;
                }
            } else
            {
                throw new NotImplementedException();
            }
        }

        private void ChooseCost(Card card, Random rng)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                int roll = (int)(rng.NextDouble() * 100) + 1;
                if (roll <= 36)
                {
                    card.Cost = 0;
                }
                else
                {
                    card.Cost = 1;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Target ChooseTarget(Card card, Random rng)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                Target target = new Target();
                int roll = (int)(rng.NextDouble() * 100) + 1;
                if (roll <= 15)
                {
                    target.SpiritTarget = true;
                    //TODO: Any vs. Another
                    return target;
                }
                else
                {
                    target.SpiritTarget = false;
                    roll = (int)(rng.NextDouble() * 100) + 1;
                    if (roll <= 46)
                    {
                        //Any
                        return target;
                    } else if (roll <= 98)
                    {
                        //TODO: Setup analysis and have the choices weighted
                        target.landConditions.Add(Utils.ChooseRandomEnumValue<LandConditon.LandConditions>(typeof(LandConditon.LandConditions), rng));
                        return target;
                    } else
                    {
                        target.landConditions.Add(Utils.ChooseRandomEnumValue<LandConditon.LandConditions>(typeof(LandConditon.LandConditions), rng));
                        LandConditon.LandConditions condition = Utils.ChooseRandomEnumValue<LandConditon.LandConditions>(typeof(LandConditon.LandConditions), rng);
                        if (!target.landConditions.Contains(condition))
                        {
                            target.landConditions.Add(condition);
                        }
                        return target;
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Range ChooseRange(Card card, Random rng)
        {
            if (card.Target.SpiritTarget)
                return new Range(false, -1, null);

            int range = -1;
            bool sacred = false;
            LandTypes? landSource = null;
            int roll = (int)(rng.NextDouble() * 85) + 1;
            if (roll <= 16)
            {
                range = 0;
            } else if (roll <= 67)
            {
                range = 1;
            } else if (roll <= 84)
            {
                range = 2;
            } else if (roll == 85)
            {
                range = 3;
            }

            roll = (int)(rng.NextDouble() * 85) + 1;
            if (roll <= 27)
            {
                sacred = true;
            }

            roll = (int)(rng.NextDouble() * 85) + 1;
            if (roll <= 2)
            {
                landSource = Utils.ChooseRandomEnumValue<LandTypes>(typeof(LandTypes), rng);
            }

            return new Range(sacred, range, landSource);
        }

        #endregion

        #region Effects
        private Effect ChooseRandomEffect(Card card, Settings settings)
        {
            IEnumerable<Effect> effects;
            if (card.Target.SpiritTarget)
            {
                effects = ReflectionManager.GetInstanciatedSubClasses<SpiritEffect>();
            } else
            {
                effects = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
            }
            
            Dictionary<Effect, int> weightedEffectChances = new Dictionary<Effect, int>();
            foreach (Effect effect in effects)
            {
                effect.InitializeEffect(card, settings);
                if (effect.IsValid(card, settings))
                {
                    //TODO: change adjusted probability to use int instead.
                    weightedEffectChances.Add(effect, (int)(effect.AdjustedProbability * 100));
                }
            }
            Effect choosenEffect = Utils.ChooseWeightedOption<Effect>(weightedEffectChances, settings.rng);
            return choosenEffect;
        }
        #endregion
    }
}
