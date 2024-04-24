using Spirit_Island_Card_Generator.Classes;
using Spirit_Island_Card_Generator.Classes.Analysis;
using Spirit_Island_Card_Generator.Classes.ArtGeneration;
using Spirit_Island_Card_Generator.Classes.CardGenerator;

namespace Spirit_Island_Card_Generator
{
    public partial class CardGeneratorForm : Form
    {
        public CardGeneratorForm()
        {
            InitializeComponent();
        }

        private void scanBtn_Click(object sender, EventArgs e)
        {
            CardScanner.GetCardsFromKatalogSite();
        }

        private void generateDeckBtn_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();

            settings.GenerateArt = generateArtChkBox.Checked;
            if (settings.GenerateArt && !Automatic1111Client.stableDiffusionClient.IsStableDiffusionRunning().Result)
            {
                MessageBox.Show("Stable Diffusion is not running. No art will be Generated");
                settings.GenerateArt = false;
            }

            settings.TargetPowerLevel = (double)targetPowerLevelBox.Value;
            settings.PowerLevelVariance = (double)varianceBox.Value;

            if (!deckNameBox.Text.Equals(""))
                settings.deckName = deckNameBox.Text;

            DeckGenerator generator = new DeckGenerator(settings);
            generator.GenerateDeck();
        }
    }
}
