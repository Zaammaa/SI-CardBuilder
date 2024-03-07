using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Spirit_Island_Card_Generator.Classes.Effects;

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
            CardGenerator cardGenerator = new CardGenerator();
            
            Dictionary<ElementSet.Element, int> elementCounts = new Dictionary<ElementSet.Element, int>();
            for (int i = 0; i < 100; i++)
            {
                Log.Information($"-----------------------------------------Card {i + 1}-----------------------------------------");
                Card card = cardGenerator.GenerateMinorCard(settings);
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
                foreach (Effect effect in card.effects)
                {
                    Log.Information($"{effect.Print()}");
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
        }
    }
}
