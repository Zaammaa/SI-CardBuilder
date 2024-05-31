using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using Serilog;
using System.Reflection;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using System.Linq;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class EffectGenerator
    {
        private static double STRONG_ELEMENT_MODIFIER = 1.65;
        private static double WEAK_ELEMENT_MODIFIER = 1.3;

        private static Dictionary<Type, double> _adjustedProbabilities = new Dictionary<Type, double>();
        public static Dictionary<Type, double> AdjustedProbabilities {
            get { return _adjustedProbabilities; }
            protected set { _adjustedProbabilities = value; }
        }

        //Random rng;
        //private List<IGeneratorOption> options = new List<IGeneratorOption>();
        private Dictionary<Type, int> timesUsed = new Dictionary<Type, int>();
        List<Type> trackedStats = new List<Type>();
        //public Context context;
        int deckIndex = 0;
        int deckMax = 100;

        public EffectGenerator()
        {
            //this.rng = context.rng;
            //this.context = context;
            FirstTimeSetup();
        }

        private void FirstTimeSetup()
        {
            //List<Type> types = ReflectionManager.GetSubClasses<Effect>();
            List<Type> types = ReflectionManager.GetSubClasses<IGeneratorOption>();
            foreach (Type type in types)
            {
                if (type.GetInterfaces().Contains(typeof(IGeneratorOption)))
                {
                    timesUsed.Add(type, 0);
                }

                if (type.GetInterfaces().Contains(typeof(ITrackedStat)))
                    trackedStats.Add(type);
            }
            AdjustWeights(new ElementSet(new List<ElementSet.Element>()));
        }

        private void AdjustWeights(ElementSet elements)
        {
            double deckPercentage = ((double)deckIndex)/ deckMax;
            AdjustedProbabilities.Clear();
            //IEnumerable<IGeneratorOption> options = ReflectionManager.GetInstanciatedSubClasses<Effect>();
            IEnumerable<IGeneratorOption> options = ReflectionManager.GetGeneratorOptions();
            foreach (IGeneratorOption option in options)
            {
                //TODO: pools
                //tracked stats

                int used = timesUsed[option.GetType()];
                if (option.GetType().GetCustomAttribute(typeof(LimitedOptionAttribute)) != null)
                {
                    LimitedOptionAttribute limitedOptionAttribute = (LimitedOptionAttribute)option.GetType().GetCustomAttribute(typeof(LimitedOptionAttribute));
                    if (limitedOptionAttribute.maxUses >= used)
                    {
                        AdjustedProbabilities[option.GetType()] = 0;
                        continue;
                    }
                }


                if (option.GetType().GetInterfaces().Contains(typeof(ITrackedStat)))
                {
                    ITrackedStat trackedStat = (ITrackedStat)option;

                    int targetAmount = (int)trackedStat.GetType().GetProperty("TargetAmount", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                    int left = targetAmount - used;
                    double chance = (double)left / (deckMax - deckIndex);
                    chance *= 5;
                    //This gets weird since I set the probability based on how likely it is to be on a card, but the EffectGenerator uses it as a weight PER EFFECT.
                    //So as an example, if about 12 minor cards have defense, I set the weight to .12. But more than one effect can be on a card. Since all the effects have a weight, that should balance the relative amount still under normal conditions
                    //However for tracked stats, we try to set the probability as a percentage of cards remaining. This will result in the effect comming up more often than it should.
                    //Still, the tracked stat will drop off after having a higher probability, so it may be fine?
                    //If analysis gets good enough later, we might be able to use a percentage of total effects, though this still may not work as I may add custom effects.
                    AdjustedProbabilities.Add(option.GetType(), chance);
                } else
                {
                    //Do a soft percentage adjustment based on how much more it's come up than expected
                    double expectedAmount = option.BaseProbability * deckIndex;
                    if (used > 0)
                    {
                        double modifier = expectedAmount/used;
                        AdjustedProbabilities.Add(option.GetType(), modifier * option.BaseProbability);
                    } else
                    {
                        AdjustedProbabilities.Add(option.GetType(), option.BaseProbability);
                    }


                }
                double elementalModifier = 1;
                foreach (ElementSet.Element element in elements.GetElements())
                {
                    if (option.WeaklyAssociatedElements.Contains(element))
                        elementalModifier *= WEAK_ELEMENT_MODIFIER;
                    if (option.StronglyAssociatedElements.Contains(element))
                        elementalModifier *= STRONG_ELEMENT_MODIFIER;
                }

                AdjustedProbabilities[option.GetType()] *= elementalModifier;
            }
        }

        public IGeneratorOption? ChooseEffect(Context context)
        {
            EffectGeneratorSettings effectSettings = EffectGeneratorSettings.GetStandardEffectSettings(context);
            return ChooseEffect(effectSettings);
        }

        public IGeneratorOption ChooseEffect(EffectGeneratorSettings effectSettings)
        {
            Effect? effect = (Effect?)ChooseGeneratorOption<Effect>(effectSettings);
            effect?.InitializeEffect(effectSettings.context);
            return effect;
        }

        public IGeneratorOption? ChooseCondition(Context context)
        {
            Condition? condition = (Condition?)ChooseGeneratorOption<Condition>(context);
            return condition;
        }

        public IGeneratorOption? ChooseGeneratorOption<T>(Context context) where T : IGeneratorOption
        {
            IEnumerable<T> options = ReflectionManager.GetInstanciatedSubClasses<T>();

            Dictionary<T, int> weightedEffectChances = new Dictionary<T, int>();
            foreach (T effect in options)
            {
                //effect.InitializeEffect(context.card, context.settings);
                if (effect.IsValidGeneratorOption(context) && (context.chain.Count == 0 || !effect.TopLevelEffect()))
                {
                    //TODO: change adjusted probability to use int instead.
                    weightedEffectChances.Add(effect, (int)(AdjustedProbabilities[effect.GetType()] * 1000));
                }
            }
            T? choosenEffect = Utils.ChooseWeightedOption<T>(weightedEffectChances, context.rng);
            return choosenEffect;
        }

        public IGeneratorOption? ChooseGeneratorOption<T>(Attribute attribute, Context context) where T : IGeneratorOption
        {
            EffectGeneratorSettings effectSettings = new EffectGeneratorSettings(context);
            effectSettings.attributes.Add(attribute);
            return ChooseGeneratorOption<T>(effectSettings);
        }

        public IGeneratorOption ChooseGeneratorOption<T>(EffectGeneratorSettings effectSettings) where T : IGeneratorOption
        {
            List<Type> types = new List<Type>();
            foreach (Attribute attribute in effectSettings.attributes)
            {
                List<Type> allTypes = ReflectionManager.GetAttributeClasses(attribute);
                foreach(Type type in allTypes)
                {
                    if (!types.Contains(type) && effectSettings.Matches(type) && type.IsSubclassOf(typeof(T)))
                    { 
                        types.Add(type);
                    }
                }
            }
                
            List<T> options = ReflectionManager.InstanciateTypes<T>(types);

            Dictionary<T, int> weightedEffectChances = new Dictionary<T, int>();
            foreach (T option in options)
            {
                if (option.IsValidGeneratorOption(effectSettings.context) && (effectSettings.context.chain.Count == 0 || !option.TopLevelEffect()) && (!option.Singleton || !effectSettings.context.card.GetAllEffects().Any(opt => opt.GetType() == option.GetType())))
                {
                    //TODO: change adjusted probability to use int instead.
                    weightedEffectChances.Add(option, (int)(AdjustedProbabilities[option.GetType()] * 10000));
                }

                //TODO: Remove this
                if (option.Singleton && effectSettings.context.card.GetAllEffects().Any(opt => opt.GetType() == option.GetType()))
                {
                    Log.Information("Option rejected for being a singleton");
                }
            }
            T? choosenEffect = Utils.ChooseWeightedOption<T>(weightedEffectChances, effectSettings.context.rng);
            return choosenEffect;
        }

        //Choose a random effect and upgrade it until it is stronger than a given amount, or until it cannot be made stronger. If the later happens, return null
        public Effect? ChooseStrongerEffect(Context context, double minPower)
        {
            return ChooseStrongerEffect(EffectGeneratorSettings.GetStandardEffectSettings(context), minPower);
        }
        //Choose an effect and downgrade it until it is weaker than a given amount, or until it cannot be made weaker. If the later happens, return null
        public Effect? ChooseWeakerEffect(Context context, double maxPower)
        {
            return ChooseWeakerEffect(EffectGeneratorSettings.GetStandardEffectSettings(context), maxPower);
        }

        //Choose a random effect and upgrade it until it is stronger than a given amount, or until it cannot be made stronger. If the later happens, return null
        public Effect? ChooseStrongerEffect(EffectGeneratorSettings effectSettings, double minPower)
        {
            Effect? option = (Effect?)ChooseEffect(effectSettings);
            while (option != null && option.CalculatePowerLevel() <= minPower)
            {
                option = option.Strengthen();
            }
            return option;
        }
        //Choose an effect and downgrade it until it is weaker than a given amount, or until it cannot be made weaker. If the later happens, return null
        public Effect? ChooseWeakerEffect(EffectGeneratorSettings effectSettings, double maxPower)
        {
            Effect? option = (Effect?)ChooseEffect(effectSettings);
            while (option != null && option.CalculatePowerLevel() >= maxPower)
            {
                option = option.Weaken();
            }
            return option;
        }

        public void IncreaseCardComplexity(Context context)
        {
            //TODO, this should potentially touch targeting and range requirements?
            Effect? effect = (Effect?)ChooseEffect(context);
            context.card.effects.Add(effect);
            Log.Information("added: " + effect);
        }

        public void DecreaseCardComplexity(Context context)
        {
            //TODO, this should potentially touch targeting and range requirements?
            Effect? effect = Utils.ChooseRandomListElement(context.card.effects, context.rng);
            context.card.effects.Remove(effect);
            Log.Information("removed: " + effect);
        }

        public void StrengthenCard(Context context)
        {
            Card card = context.card;
            Effect? effect = Utils.ChooseRandomListElement(card.effects, context.rng);
            Effect? newEffect = effect?.Strengthen();

            if (newEffect != null && effect != null && newEffect.CalculatePowerLevel() > effect.CalculatePowerLevel())
            {
                double newPowerLevel = card.CalculatePowerLevel() - effect.CalculatePowerLevel() + newEffect.CalculatePowerLevel();
                //Make sure we didn't overcorrect
                if (newPowerLevel <= context.settings.TargetPowerLevel + context.settings.PowerLevelVariance)
                {
                    //TODO: Keep order?
                    card.effects.Remove(effect);
                    card.effects.Add(newEffect);
                    if (context.card.GetAllEffects().Any((eff) => eff.LinkedEffects.Contains(effect)))
                    {
                        Effect EffectLinkingToThis = context.card.GetAllEffects().Find((eff) => eff.LinkedEffects.Contains(effect));
                        EffectLinkingToThis.LinkedEffects.Remove(effect);
                        EffectLinkingToThis.LinkedEffects.Add(newEffect);
                    }
                    Log.Information("Strengthened effect: " + newEffect);
                    
                    return;
                }
            }
            
            newEffect = (Effect?)ChooseEffect(context);
            Log.Information($"Strengthen of {effect} failed. Added " + newEffect);
            context.card.effects.Add(newEffect);
        }

        public void WeakenCard(Context context)
        {
            Card card = context.card;

            //Stronger options tend to be removed more often since they are harder to slot in with other effects
            //This is an attempt to counteract that by making weaker effects be selected more often
            Dictionary<Effect, int> weights = new Dictionary<Effect, int>();
            foreach (Effect e in card.effects)
            {
                weights.Add(e, (int)(Math.Pow(1 / e.CalculatePowerLevel(), 1.0) * 10000));
            }

            Effect? effect = Utils.ChooseWeightedOption(weights, context.rng);
            Effect? newEffect = effect?.Weaken();
            if (newEffect != null && effect != null && newEffect.CalculatePowerLevel() < effect.CalculatePowerLevel())
            {

                double newPowerLevel = card.CalculatePowerLevel() - effect.CalculatePowerLevel() + newEffect.CalculatePowerLevel();
                //Make sure we didn't overcorrect
                if (newPowerLevel >= context.settings.TargetPowerLevel - context.settings.PowerLevelVariance)
                {
                    //TODO: Keep order?
                    card.effects.Remove(effect);
                    card.effects.Add(newEffect);
                    if (context.card.GetAllEffects().Any((eff) => eff.LinkedEffects.Contains(effect)))
                    {
                        Effect EffectLinkingToThis = context.card.GetAllEffects().Find((eff) => eff.LinkedEffects.Contains(effect));
                        EffectLinkingToThis.LinkedEffects.Remove(effect);
                        EffectLinkingToThis.LinkedEffects.Add(newEffect);
                    }
                    Log.Information("Weakened effect: " + newEffect);
                    return;
                }
            }
            
            card.effects.Remove(effect);
            Log.Information("Removed effect: " + effect);
        }

        //Update the stats after a card has been finished
        public void CardChosen(Card card)
        {

            //TODO: change Effect to generator options so the tracked stats works for cost targets and the like
            foreach(Effect effect in card.GetAllEffects())
            {
                if (timesUsed.ContainsKey(effect.GetType()))
                {
                    timesUsed[effect.GetType()]++;
                } else
                {
                    timesUsed.Add(effect.GetType(), 1);
                }
            }
            deckIndex++;
            AdjustWeights(card.elements);
        }

        public void LogTrackedStats()
        {
            Log.Information("Tracked Stats:");
            foreach(Type stat in trackedStats)
            {
                string name = stat.GetProperty("TrackedName", BindingFlags.Static | BindingFlags.Public).GetValue(null).ToString();
                int target = (int)stat.GetProperty("TargetAmount", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                Log.Information($"{name}: {timesUsed[stat]}. Target: {target}");
            }
        }

    }
}
