using System;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class NameGenerator
    {
        // TODO move how likely a template is used to template files
        private static double STANDARD_TEMPLATE_PROBABILITY = 0.9;
        private static double MEMORABLE_TEMPLATE_PROBABILITY = 0.1;
        private Dictionary<string, List<string>> wordLists = new Dictionary<string, List<string>>();
        private Dictionary<string, List<List<string>>> templates = new Dictionary<string, List<List<string>>>();
        private List<string> previousNames = new List<string>();
 
        public NameGenerator()
        {
            SetupCardNameOptions();
            SetupTemplateOptions();
        }

        public string GenerateCardName(Card card, Random rng)
        {
            string name = "";
            int roll = 0;
            var template = selectTemplate(card, rng);
            do
            {
                foreach (var tWord in template)
                {
                    if (wordLists.ContainsKey(tWord))
                    {
                        //TODO look at elements/effects to inform word choice
                        roll = (int)(rng.NextDouble() * wordLists[tWord].Count);
                        name += wordLists[tWord][roll];
                        //TODO? worth fixing that words will still be removed if name was too long initially
                        wordLists[tWord].RemoveAt(roll);
                        if (wordLists[tWord].Count == 0)
                            SetupCardNameOptions(tWord.Replace("_", ""));
                    } else
                        name += tWord;
                    name += " ";
                }
                name = name.Trim();
            } while (previousNames.Contains(name) || name.Length > 40);
            previousNames.Add(name);
            return name;
        }

        private List<string> selectTemplate(Card card, Random rng)
        {
            //TODO look at card target for picking some templates
            int tType = (int)(rng.NextDouble()*100);
            string type = "";
            if (tType <= 100 * STANDARD_TEMPLATE_PROBABILITY)
                type = "Standard";
            else if (tType >= 100 - 100 * MEMORABLE_TEMPLATE_PROBABILITY)
                type = "Memorable";
            int roll = (int)(rng.NextDouble() * templates[type].Count);
            var template = templates[type][roll];
            return template;
        }

        private void SetupCardNameOptions(string singleType = "no")
        {
            var types = new List<String>() { "Nouns", "Adjectives", "Verbs", "Possessives", "SILocations" };
            if ( singleType != "no")
            {
                types.Clear();
                types.Add(singleType);
            }
            foreach (var type in types)
            {
                string text = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Card Name Words\\{type}.txt"));
                wordLists[$"__{type}__"] = new List<string>(text.Split(Environment.NewLine));
            }
        }

        private void SetupTemplateOptions()
        {
            //TODO add Common templates and consolidate files
            var types = new List<String>() { "Standard", "Memorable" };
            foreach (var type in types)
            {
                string text = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Card Name Words\\{type} Templates.txt"));
                var lines = new List<string>(text.Split(Environment.NewLine));
                var templateFile = new List<List<string>>();
                foreach (var line in lines)
                {
                    var lineList = new List<string>(line.Split(" "));
                    for (int i = 0; i < lineList.Count; i++)
                    {
                        lineList[i] = lineList[i].Trim();
                    }
                    templateFile.Add(lineList);
                }
                templates[type] = templateFile;
            }
        }
    }
}