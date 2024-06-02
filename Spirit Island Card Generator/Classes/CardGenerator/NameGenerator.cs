using System;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class NameGenerator
    {
        private Dictionary<string, List<string>> wordLists = new Dictionary<string, List<string>>();
        private Dictionary<string, List<List<string>>> templates = new Dictionary<string, List<List<string>>>();
        private List<string> previousNames = new List<string>();
        private string cardNameDir;


        public NameGenerator(Settings settings)
        {
            cardNameDir = settings.cardNamePath.Equals("") ? Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Card Name Words") : settings.cardNamePath;
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
                name = "";
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
            //TODO look at card target for some templates
            int tType = (int)(rng.NextDouble()*100);
            string type = "Standard";
            int roll = (int)(rng.NextDouble() * templates[type].Count);
            var template = templates[type][roll];
            templates[type].RemoveAt(roll);
            if (templates[type].Count == 0)
                SetupTemplateOptions(type);
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
                wordLists[$"__{type}__"] = File.ReadAllLines(Path.Combine(cardNameDir, $"{type}.txt")).ToList();
            }
        }

        private void SetupTemplateOptions(string singleType = "no")
        {
            var types = new List<String>() { "Standard", "Spirit-Targeting", "Dahan-Targeting" };
            if (singleType != "no")
            {
                types.Clear();
                types.Add(singleType);
            }
            foreach (var type in types)
            {
                var lines = File.ReadAllLines(Path.Combine(cardNameDir, $"{type} Templates.txt")).ToList();
                var templateFile = new List<List<string>>();
                foreach (var line in lines)
                {
                    var tNumList = new List<string>(line.Split("#"));
                    var lineList = new List<string>(tNumList[0].Split(" "));
                    for (int i = 0; i < lineList.Count; i++)
                    {
                        lineList[i] = lineList[i].Trim();
                    }
                    int num = 1;
                    if (tNumList.Count == 2)
                        num = Int32.Parse(tNumList[1].Trim());
                    for (int i = 0; i < num; i++)
                    {
                        templateFile.Add(lineList);
                    }
                }
                templates[type] = templateFile;
            }
        }
    }
}