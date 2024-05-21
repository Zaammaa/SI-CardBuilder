using Spirit_Island_Card_Generator.Classes.DeckManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardOutput
{
    /// <summary>
    /// Structures the folders and outputs cards into files so they can be viewed.
    /// 
    /// There are are two types of files for now:
    ///     1: The file for the spirit island builder website
    ///     2: The file for the local spirit island javascript
    /// In the future there will be a json save of the card so it can be loaded back up
    /// </summary>
    internal class CardPrinter
    {
        public static void CreateSIBuilderFile(Deck deck, string outputDir)
        {
            CreateSIBuilderFile(deck.deck, Path.Combine(outputDir, "minor power deck.html"));
        }

        public static void CreateSIBuilderFile(IEnumerable<Card> cards, string filepath)
        {
            string fileText = "<style></style>";

            foreach (Card card in cards)
            {
                fileText += MakeQuickCard(card) + "\n";
            }
            fileText += "<spirit-name>Minor Deck</spirit-name>";
            File.WriteAllText(filepath, fileText);
        }

        public static void CreatePartialSIBuilderFiles(Deck deck, string outputDir)
        {
            string fileText = "<style></style>";
            Card[] cards = deck.deck.ToArray();
            List<Card> cardChunk = new List<Card>();

            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                cardChunk.Add(card);
                if (cardChunk.Count >= 9 || (i == cards.Length - 1 && cardChunk.Count > 0))
                {
                    CreateSIBuilderFile(cardChunk, Path.Combine(outputDir, $"{i - 9}-{i}.html"));
                    cardChunk.Clear();
                }
            }
        }

        public static void CreateSingleCardBuilderFile(Card card, string outputDir) {
            string fileText = "<style></style>";
            fileText += MakeQuickCard(card) + "\n";
            fileText += $"<spirit-name>{card.Name}</spirit-name>";
            File.WriteAllText(Path.Combine(outputDir, $"{card.Name}-SI-builder.html"), fileText);
        }


        public static void CreateLocalViewFile(Card card, string outputDir)
        {
            string javascriptPath = AppSettings.Default.CSSPath;
            string fileText = "<!DOCTYPE html>";
            fileText += "<head>\n  " +
                $"<link href=\"{javascriptPath}/_global/css/global.css\" rel=\"stylesheet\" />\n" +
                $"<link href=\"{javascriptPath}/_global/css/card.css\" rel=\"stylesheet\" />\n" +
                $"<script type=\"text/javascript\" src=\"{javascriptPath}/_global/js/general.js\"></script>\n" +
                $"<script src=\"{javascriptPath}/_global/js/card.js\" defer></script>\n" +
                "</head>\n";

            fileText += "<body>\n";
            fileText += MakeQuickCard(card) + "\n";
            fileText += "</body>\n";
            fileText += "</html>";
            File.WriteAllText(Path.Combine(outputDir,card.Name + ".html"), fileText);
        }

        private static string MakeQuickCard(Card card)
        {
            string quickCard = $"<quick-card image=\"{card.ArtDataString}\" name=\"{card.Name}\" speed=\"{(card.Fast ? "fast" : "slow")}\" cost=\"{card.Cost}\" type=\"{card.CardType}\" range=\"{(card.Range.Print())}\" target=\"{card.Target.Print()}\" elements=\"{card.elements}\" image=\"{card.artworkDataString}\" artist-name=\"undefined\"><rules>{card.descrition}</rules>";
            if (card.HasThreshold)
                quickCard += $"<threshold condition=\"{card.thresholdCondition}\">{card.thresholdDescription}</threshold>";

            quickCard += "</quick-card>\n";
            return quickCard;
        }
    }
}
