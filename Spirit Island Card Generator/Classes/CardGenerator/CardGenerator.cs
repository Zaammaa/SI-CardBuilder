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
using static Spirit_Island_Card_Generator.Classes.Card;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.Fixers;
using Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions;

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
            card.settings = settings;
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
            card.Target = ChooseTarget(context);
            context.target = card.Target;
            if (context.target.SpiritTarget)
                context.targetContext = Context.CardTargets.TargetSpirit;
            else
                context.targetContext = Context.CardTargets.Land;
            //context.conditions = card.Target.landConditions;
            card.Range = ChooseRange(context);
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
            while (!card.IsValidGeneratorOption(context))
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

                //Check effect validity
                EnforceValidity(card);

            }
            Log.Information("}");
            //Step 6: Name
            //Step 7: Art

            generator.CardChosen(card);
            return card;
        }

        public void EnforceValidity(Card card)
        {
            //foreach (Effect effect in card.GetAllEffects())
            while(card.GetAllEffects().Any((effect) => effect.IsValid() != null))
            {
                IValidFixer? fixer;
                Effect? effect = card.GetAllEffects().Find((effect) => effect.IsValid() != null);
                fixer = effect?.IsValid();
                //if (fixer == null || !card.GetAllEffects().Contains(effect))
                //    continue;

                FixerResult result = fixer.Fix();
                if (result.result == FixerResult.FixResult.UpdateEffect)
                {
                    Log.Information("Updating Effect: " + effect.ToString());
                    Effect updatedEffect = (Effect)result.resultObj;
                    if (card.effects.Contains(effect))
                    {
                        card.effects.Remove(effect);
                        card.effects.Add(updatedEffect);
                    }
                    else
                    {
                        IParentEffect? ParentEffect = effect.GetSameContext()?.Parent;
                        if (ParentEffect != null)
                        {
                            ParentEffect.ReplaceEffect(effect, updatedEffect);
                        }
                    }
                }
                else if (result.result == FixerResult.FixResult.RemoveEffect)
                {
                    Log.Information("Removing Effect: " + effect.ToString());
                    Effect? effectToRemove = (Effect)result.resultObj;

                    effectToRemove = card.GetAllEffects().Find((eff) => eff.GetType() == effectToRemove.GetType());
                    //effectToRemove = card.GetAllEffects().Find((eff) => effectToRemove.LinkedEffects.Any((e) => eff.LinkedEffects.Contains(e)));
                    Effect? topLevelEffect = card.effects.Find((topEff) => topEff.GetAllEffectsInChain().Contains(effectToRemove));
                    if (topLevelEffect != null && card.effects.Contains(topLevelEffect))
                    {
                        card.effects.Remove(topLevelEffect);
                    }
                    else if (card.effects.Contains(effectToRemove))
                    {
                        card.effects.Remove(effectToRemove);
                    } else
                    {
                        throw new Exception("Card does not contain the effect it tried to remove");
                    }
                }
                else if (result.result == FixerResult.FixResult.FixFailed)
                {
                   
                    Effect? topLevelEffect = (Effect?)effect.GetSameContext()?.chain.FirstOrDefault();
                    if (topLevelEffect != null)
                    {
                        Log.Information("Fix failed. Removing effect Effect: " + topLevelEffect.ToString());
                        card.effects.Remove(topLevelEffect);
                    }
                    else
                    {
                        Log.Information("Fix failed. Removing effect Effect: " + effect.ToString());
                        card.effects.Remove(effect);
                    }
                }
                else if (result.result == FixerResult.FixResult.FixError)
                {
                    Log.Warning("Fix result had an error");
                }
            }
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

        private Target ChooseTarget(Context context)
        {
            TargetOption option = (TargetOption)context.effectGenerator.ChooseGeneratorOption<TargetOption>(context);
            return option.OnChosen(context);

            //Card card = context.card;

            //if (card.CardType == Card.CardTypes.Minor)
            //{
            //    Target target = new Target();
            //    int roll = (int)(context.rng.NextDouble() * 100) + 1;
            //    if (roll <= 3)
            //    {
            //        target.targetType = Target.TargetType.AnotherSpirit;
            //        return target;
            //    }
            //    else if (roll <= 15)
            //    {
            //        target.targetType = Target.TargetType.AnySpirit;
            //        return target;
            //    }
            //    else
            //    {
            //        target.targetType = Target.TargetType.Land;
            //        roll = (int)(context.rng.NextDouble() * 100) + 1;
            //        if (roll <= 46)
            //        {
            //            //Any
            //            return target;
            //        } else
            //        {
            //            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            //            foreach(LandConditions land in Card.conditions.Keys)
            //            {
            //                WeightAndPowerDifference weightAndPowerDifference = Card.conditions[land];
            //                weights.Add(land, weightAndPowerDifference.weight);
            //            }

            //            target.landConditions.Add(Utils.ChooseWeightedOption(weights, context.rng));
            //            return target;
            //        }
            //    }
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
        }

        private Range ChooseRange(Context context)
        {
            RangeOption option = (RangeOption)context.effectGenerator.ChooseGeneratorOption<RangeOption>(context);
            if (option == null)
                return new Range(false, -1, null);

            Range range = option.OnSelected(context);
            context.card.Range = range;

            FromLandOption fromOption = (FromLandOption)context.effectGenerator.ChooseGeneratorOption<FromLandOption>(context);
            range = fromOption.OnSelected(context);

            return range;
        }

        #endregion
    }
}
