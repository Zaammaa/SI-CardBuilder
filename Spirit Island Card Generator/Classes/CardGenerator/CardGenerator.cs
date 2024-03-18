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
        public EffectGenerator generator;

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
            generator = new EffectGenerator();
        }

        public Card GenerateMinorCard(Settings settings)
        {
            Card card = new Card();
            card.CardType = Card.CardTypes.Minor;
            Context context = new Context();
            context.rng = settings.rng;
            context.settings = settings;
            context.card = card;
            context.effectGenerator = generator;
            
            //Step 1: Elements
            card.elements = ChooseElements(context);
            //Step 2: Speed/Cost/Target/Range
            ChooseSpeed(card, settings.rng);
            ChooseCost(card, settings.rng);
            card.Target = ChooseTarget(card, settings.rng);
            context.target = card.Target;
            //context.conditions = card.Target.landConditions;
            card.Range = ChooseRange(card, settings.rng);
            //Step 3: Template
            //TODO use other templates
            EffectTemplate template = EffectTemplate.Standard;
            //Step 4: Effects
            
            do
            {
                //generator.ChooseEffect(context);
                Effect? effect = (Effect?)generator.ChooseEffect(context);
                if (effect != null)
                    card.effects.Add(effect);
            } while (card.Complexity() <= settings.MinComplexity);
            Log.Information("Initial Effects:");
            Log.Information("{");
            card.effects.ForEach(c => { Log.Information(c.Print()); });
            Log.Information("}");

            Log.Information("Refining process:");
            Log.Information("{");
            //Step 5: Refine Balance
            while (!card.IsValid(context))
            {
                if (card.effects.Count == 1 && !card.effects.First().Standalone)
                {
                    Effect? effect = (Effect?)generator.ChooseEffect(context);
                    if (effect != null)
                        card.effects.Add(effect);
                    Log.Information("card needed another effect due to the only effect being standalone. Added effect: " + effect);
                } else if (card.Complexity() > settings.MaxComplexity)
                {
                    Log.Information("card was too complex. Reducing complexity:");
                    generator.DecreaseCardComplexity(context);
                } else if (card.Complexity() < settings.MinComplexity) {
                    Log.Information("card was not complex enough. Increasing complexity:");
                    generator.IncreaseCardComplexity(context);
                } else if (card.CalculatePowerLevel() >= settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    Log.Information("card was too strong. Reducing strength:");
                    generator.WeakenCard(context);
                } else if (card.CalculatePowerLevel() <= settings.TargetPowerLevel - settings.PowerLevelVariance)
                {
                    Log.Information("card was not strong enough. Increasing strength:");
                    generator.StrengthenCard(context);
                } else
                {
                    Log.Information("card was otherwise invalid:");
                    //TODO, this should potentially touch targeting and range requirements?
                    Effect? effect = Utils.ChooseRandomListElement(card.effects, settings.rng);
                    card.effects.Remove(effect);
                }
            }
            Log.Information("}");
            //Step 6: Name
            //Step 7: Art

            generator.CardChosen(card);
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

        private ElementSet ChooseElements(Context context)
        {
            Settings settings = context.settings;
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
        //private Effect ChooseRandomEffect(Context context)
        //{
        //    IEnumerable<Effect> effects;
        //    if (card.Target.SpiritTarget)
        //    {
        //        effects = ReflectionManager.GetInstanciatedSubClasses<SpiritEffect>();
        //    } else
        //    {
        //        effects = ReflectionManager.GetInstanciatedSubClasses<LandEffect>();
        //    }
            
        //    Dictionary<Effect, int> weightedEffectChances = new Dictionary<Effect, int>();
        //    foreach (Effect effect in effects)
        //    {
        //        effect.InitializeEffect(card, settings);
        //        if (effect.IsValid(card, settings))
        //        {
        //            //TODO: change adjusted probability to use int instead.
        //            weightedEffectChances.Add(effect, (int)(effect.AdjustedProbability * 100));
        //        }
        //    }
        //    Effect choosenEffect = Utils.ChooseWeightedOption<Effect>(weightedEffectChances, settings.rng);
        //    return choosenEffect;
        //}
        #endregion
    }
}
