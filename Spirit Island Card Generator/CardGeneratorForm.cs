using Spirit_Island_Card_Generator.Classes.Analysis;
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
            DeckGenerator generator = new DeckGenerator(new Classes.Settings());
            generator.GenerateDeck();
        }
    }
}
