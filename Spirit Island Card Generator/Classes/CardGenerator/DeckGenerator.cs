using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Spirit_Island_Card_Generator.Classes.ArtGeneration;
using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    internal class DeckGenerator
    {
        List<Card> deck = new List<Card>();

        Settings settings;
        Dictionary<ElementSet.Element, int> elementCounts = new Dictionary<ElementSet.Element, int>();
        CardGenerator cardGenerator = new CardGenerator();
        Automatic1111Client artGenerator = Automatic1111Client.stableDiffusionClient;
        private string Workspace => Path.Combine(settings.workspace, settings.deckName);
        private string ArtDir => Path.Combine(Workspace, "Art");
        private string CardLogs => Path.Combine(Workspace, "CardLogs");


        public DeckGenerator(Settings s)
        {
            settings = s;
        }

        public void GenerateDeck()
        {           
            SetupWorkSpace();
            for (int i = 1; i <= 100; i++)
            {
                Log.Information($"-----------------------------------------Card {i}-----------------------------------------");
                Card card = cardGenerator.GenerateMinorCard(settings);
                card.Name = i.ToString();

                PostProcessCard(card);
                deck.Add(card);
                QueueArtwork(card, i);
            }
            Log.Information("---------------------------Finished Deck---------------------------");
            Log.Information("Element Counts:");
            foreach (ElementSet.Element el in elementCounts.Keys)
            {
                Log.Information($"{el}: {elementCounts[el]}");
            }
            cardGenerator.generator.LogTrackedStats();
        }

        private void SetupWorkSpace()
        {
            if (!Directory.Exists(Workspace))
            {
                Directory.CreateDirectory(Workspace);
            }
            if (!Directory.Exists(ArtDir))
            {
                Directory.CreateDirectory(ArtDir);
            }
            if (!Directory.Exists(CardLogs))
            {
                Directory.CreateDirectory(CardLogs);
            }
            Logger.ChangeLogConfig(Logger.MakeDeckLogger(Path.Combine(Workspace, "Log.txt")));
        }

        private void QueueArtwork(Card card, int i)
        {
            StableDiffusionSettings stableDiffusionSettings = new StableDiffusionSettings();
            stableDiffusionSettings.prompt = "<lora:SI_ArtV1-33:1.0>,  PowerCard, __Templates__, artwork by __SIArtists__";
            stableDiffusionSettings.seed = settings.seed;
            stableDiffusionSettings.saveFilename = Path.Combine(ArtDir, card.Name + ".png");

            if (i == 100)
            {
                stableDiffusionSettings.AfterImageProcesses += this.OnLastImageProcessed;
            } else
            {
                stableDiffusionSettings.AfterImageProcesses += card.OnArtFinished;
            }
            
            artGenerator.AddPromptToQueue(stableDiffusionSettings);
        }

        private void OnLastImageProcessed(object  sender, SimpleEventArgs e)
        {
            deck.Last().OnArtFinished(sender, e);

            CreateBuilderFile();
            Cleanup();
        }

        private void PostProcessCard(Card card)
        {
            foreach (ElementSet.Element el in card.elements.GetElements())
            {
                if (elementCounts.ContainsKey(el))
                {
                    elementCounts[el]++;
                }
                else
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
                }
                else
                {
                    ElementalThresholdEffect thresholdEffect = (ElementalThresholdEffect)effect;
                    card.thresholdCondition = thresholdEffect.ConditionText;
                    card.thresholdDescription = thresholdEffect.EffectText;
                }
                Log.Information($"{effect.Print()} : {effect.CalculatePowerLevel()} ");
            }
            Log.Information($"Power level: {card.CalculatePowerLevel()}");
            Log.Information($"Complexity: {card.Complexity()}");
        }

        private void CreateBuilderFile()
        {
            string fileText = "<style></style>";
            foreach(Card card in deck)
            {
                fileText += $"<quick-card image=\"{card.ArtDataString}\" name=\"{card.Name}\" speed=\"{(card.Fast ? "fast" : "slow")}\" cost=\"{card.Cost}\" type=\"{card.CardType}\" range=\"{(card.Range.Print())}\" target=\"{card.Target.Print()}\" elements=\"{card.elements.ToString()}\" image=\"{card.artworkDataString}\" artist-name=\"undefined\"><rules>{card.descrition}</rules>";
                if (card.HasThreshold)
                    fileText += $"<threshold condition=\"{card.thresholdCondition}\">{card.thresholdDescription}</threshold>";
                        
                fileText += "</quick-card>\n";
            }
            fileText += "<spirit-name>Minor Deck</spirit-name>";
            File.WriteAllText(Path.Combine(Workspace, "minor power deck.html"), fileText);
        }

        

        private void Cleanup()
        {
            Logger.ChangeLogConfig(Logger.DefaultConfig);
        }
    }
}
