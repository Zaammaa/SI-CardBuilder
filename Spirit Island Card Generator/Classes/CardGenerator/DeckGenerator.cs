using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    internal class DeckGenerator
    {
        List<Card> deck = new List<Card>();

        Settings settings;

        public DeckGenerator(Settings s)
        {
            settings = s;
        }

        public void GenerateDeck()
        {
            Context context = new Context();
            context.rng = settings.rng;
            context.settings = settings;
            CardGenerator cardGenerator = new CardGenerator();
            
            Dictionary<ElementSet.Element, int> elementCounts = new Dictionary<ElementSet.Element, int>();
            for (int i = 1; i <= 100; i++)
            {
                Log.Information($"-----------------------------------------Card {i}-----------------------------------------");
                Card card = cardGenerator.GenerateMinorCard(settings);
                card.Name = i.ToString();
                deck.Add(card);

                foreach(ElementSet.Element el in card.elements.GetElements())
                {
                    if (elementCounts.ContainsKey(el))
                    {
                        elementCounts[el]++;
                    } else
                    {
                        elementCounts.Add(el, 1);
                    }
                }

                Log.Information("Elements: " + card.elements.ToString());
                string speed = card.Fast ? "fast" : "slow";
                Log.Information("Speed: " + speed);
                Log.Information("Cost: " + card.Cost);
                Log.Information($"Target: {card.Target.Print()}");
                Log.Information($"Range: {card.Range.Print()}");
                card.effects.Sort((x, y) => x.PrintOrder().CompareTo(y.PrintOrder()));
                foreach (Effect effect in card.effects)
                {
                    if (effect.GetType() != typeof(ElementalThresholdEffect))
                    {
                        card.descrition += effect.Print() + "\n";
                    } else
                    {
                        ElementalThresholdEffect thresholdEffect = (ElementalThresholdEffect) effect;
                        card.thresholdCondition = thresholdEffect.ConditionText;
                        card.thresholdDescription = thresholdEffect.EffectText;
                    }
                    Log.Information($"{effect.Print()} : {effect.CalculatePowerLevel()} ");
                }
                Log.Information($"Power level: {card.CalculatePowerLevel()}");
                Log.Information($"Complexity: {card.Complexity()}");
                
            }
            Log.Information("---------------------------Finished Deck---------------------------");
            Log.Information("Element Counts:");
            foreach (ElementSet.Element el in elementCounts.Keys)
            {
                Log.Information($"{el}: {elementCounts[el]}");
            }
            cardGenerator.generator.LogTrackedStats();
            CreateBuilderFile();
        }

        private void CreateBuilderFile()
        {
            string fileText = "<style></style>";
            foreach(Card card in deck)
            {
                fileText += $"<quick-card name=\"{card.Name}\" speed=\"{(card.Fast ? "fast" : "slow")}\" cost=\"{card.Cost}\" type=\"{card.CardType}\" range=\"{(card.Range.Print())}\" target=\"{card.Target.Print()}\" elements=\"{card.elements.ToString()}\" image=\"{card.artworkDataString}\" artist-name=\"undefined\"><rules>{card.descrition}</rules>";
                if (card.HasThreshold)
                    fileText += $"<threshold condition=\"{card.thresholdCondition}\">{card.thresholdDescription}</threshold>";
                        
                fileText += "</quick-card>\n";
            }
            fileText += "<spirit-name>Minor Deck</spirit-name>";
            File.WriteAllText(@"D:\Spirit Island\Minor Power Decks\minor power deck.html", fileText);
        }
    }
}
