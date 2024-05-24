using Spirit_Island_Card_Generator.Classes;
using Spirit_Island_Card_Generator.Classes.Analysis;
using Spirit_Island_Card_Generator.Classes.ArtGeneration;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.DeckManagement;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;

namespace Spirit_Island_Card_Generator
{
    public partial class CardGeneratorForm : Form
    {
        public CardGeneratorForm()
        {
            InitializeComponent();

            string filePath = Path.Combine(AppSettings.Default.CardOutputDir, "Minor Power Decks", "2024-05-03-11-27-40", "Cards", "A Flame of Dread and Exaltation.html");
            string fileText = File.ReadAllText(filePath);
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentText = fileText;
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

            settings.AllowRandomEffects = allowRandomEffectsBox.Checked;

            settings.TargetPowerLevel = (double)targetPowerLevelBox.Value;
            settings.PowerLevelVariance = (double)varianceBox.Value;

            settings.MinComplexity = (int)minComplexityBox.Value;
            settings.MaxComplexity = (int)maxComplexityBox.Value;

            settings.CustomEffectLevel = (int)customEffectLevelBox.Value;

            settings.SpiritEffectBuff = (double) spiritTargetBuffBox.Value;

            if (!deckNameBox.Text.Equals(""))
                settings.deckName = deckNameBox.Text;

            DeckGenerator generator = new DeckGenerator(settings);
            generator.CardUpdate += OnCardProgress;
            generator.ImageUpdate += OnArtProgress;
            generator.Finished += OnDeckFinished;
            Thread makeDeckThread = new Thread(new ThreadStart(() => generator.GenerateDeck()));
            generateDeckBtn.Visible = false;
            progressBar.Maximum = settings.DeckSize;
            makeDeckThread.Start();
        }

        private void loadDeckbtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.InitialDirectory = AppSettings.Default.CardOutputDir;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Deck deck = Deck.LoadDeck(folderBrowserDialog1.SelectedPath);
                int blightRemoveCount = 0;
                foreach(Card card in deck.deck)
                {
                    if (card.GetAllEffects().Any((eff) => eff.GetType().Equals(typeof(RemoveBlightEffect))))
                    {
                        blightRemoveCount++;
                    }
                }
                MessageBox.Show($"BlightRemoval: {blightRemoveCount}");
            }
        }

        private void OnCardProgress(object sender, SimpleEventArgs e)
        {
            if (progressBar.BackColor != Color.Blue)
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    progressBar.BackColor = Color.Blue;
                }));
            }

            progressBar.BeginInvoke(new Action(() =>
            {
                progressBar.Value = (int)e.data;
            }));
        }

        private void OnArtProgress(object sender, SimpleEventArgs e)
        {
            if (progressBar.BackColor != Color.Green)
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    progressBar.BackColor = Color.Green;
                }));
            }

            progressBar.BeginInvoke(new Action(() =>
            {
                progressBar.Value = (int)e.data;
            }));
        }

        private void OnDeckFinished(object sender, SimpleEventArgs e)
        {
            progressBar.BeginInvoke(new Action(() =>
            {
                progressBar.Visible = false;
            }));

            generateDeckBtn.BeginInvoke(new Action(() =>
            {
                generateDeckBtn.Visible = true;
            }));
        }
    }
}
