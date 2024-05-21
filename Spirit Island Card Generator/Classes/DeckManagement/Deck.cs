using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.DeckManagement
{
    internal class Deck
    {
        public List<Card> deck = new List<Card>();
        public int maxDeckSize = 100;

        /// <summary>
        /// Adds a card if there is room in the deck
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool AddCard(Card card)
        {
            if (deck.Count < maxDeckSize)
            {
                deck.Add(card);
                return true;
            }
            return false;
        }

        public bool RemoveCard(Card card)
        {
            return deck.Remove(card);
        }

        public void SaveDeck(string workspace)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.ContractResolver = new JSONPrivateContractResolver();

            using (StreamWriter sw = new StreamWriter(Path.Combine(workspace, "DeckSave.json")))
            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this, typeof(Deck));
            }
        }

        public static Deck LoadDeck(string workspace)
        {
            Deck? deck = JsonConvert.DeserializeObject<Deck>(File.ReadAllText(Path.Combine(workspace, "DeckSave.json")), new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
            });

            return deck;
        }
    }
}
