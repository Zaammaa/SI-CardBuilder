using System;
using System.Diagnostics;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class NameGenerator
    {
        private List<string> keywords;
        private Dictionary<string, Dictionary<string, List<string>>> wordLists = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> masterWordList = new Dictionary<string, List<string>>();
        private Dictionary<string, List<List<string>>> templates = new Dictionary<string, List<List<string>>>();
        private List<string> previousNames = new List<string>();
        private string cardNameDir;
        private Random rng;
        private string GENERIC_KEY = "Generic";

        public NameGenerator(Settings settings)
        {
            cardNameDir = settings.cardNamePath.Equals("") ? Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Card Name Words") : settings.cardNamePath;
            rng = settings.rng;
            keywords = getFileLineList("Association Keywords");
            SetupCardNameOptions();
            SetupTemplateOptions();
        }

        public string GenerateCardName(Card card)
        {
            string name = "";
            int roll = 0;
            var template = selectTemplate(card);
            var numTypes = countFillInWords(template);
            Debug.WriteLine(template + " " + numTypes);
            do
            {
                name = "";
                var assoList = getElementsAndEffects(card);
                foreach (var tWord in template)
                {
                    if (wordLists.ContainsKey(tWord))
                    {
                        
                        string word = "";
                        roll = (int)(rng.NextDouble() * assoList.Count);
                        string asso = assoList[roll];
                        string asso2 = null;
                        if (assoList.Count > numTypes)
                        {
                            roll = (int)(rng.NextDouble() * assoList.Count);
                            asso2 = assoList[roll];
                        }
                        Debug.WriteLine("*********************************");
                        Debug.WriteLine("Part of speech: " + tWord + " Chosen asso: " + asso + " and " + asso2);
                        bool canUseAsso1 = wordLists[tWord].ContainsKey(asso) && wordLists[tWord][asso].Count > 0;
                        bool canUseAsso2 = asso2 != null && wordLists[tWord].ContainsKey(asso2) && wordLists[tWord][asso2].Count > 0;

                        if (canUseAsso1 || canUseAsso2)
                        {
                            do
                            {
                                Debug.WriteLine("Using asso: " + asso + " and " + asso2);
                                if (canUseAsso1 && canUseAsso2)
                                {
                                    Debug.WriteLine("Trying intersectioin");
                                    var intersection = wordLists[tWord][asso].Intersect(wordLists[tWord][asso2]);
                                    Debug.WriteLine("Created intersection, size " + intersection.ToList().Count);
                                    var interList = intersection.ToList();
                                    if (interList.Count > 0)
                                    {
                                        roll = (int)(rng.NextDouble() * interList.Count);
                                        word = interList[roll];
                                        Debug.WriteLine("Completed and got word: " + word);
                                        wordLists[tWord][asso].Remove(word);
                                        wordLists[tWord][asso2].Remove(word);
                                        continue;
                                    }
                                }

                                if (canUseAsso1)
                                {
                                    Debug.WriteLine("Sizd before remove: " + wordLists[tWord][asso].Count);
                                    word = selectWord(wordLists[tWord][asso]);
                                    Debug.WriteLine("Sizd after remove: " + wordLists[tWord][asso].Count);

                                }
                                else if (canUseAsso2)
                                    word = selectWord(wordLists[tWord][asso2]);
                                else
                                {
                                    word = selectGenericWord(tWord);
                                    Debug.WriteLine("Have to use Generic, got " + word);

                                }
                            canUseAsso1 = wordLists[tWord][asso].Count > 0;
                            if (asso2 != null)
                                canUseAsso2 = wordLists[tWord][asso2].Count > 0;
                        } while (!masterWordList[tWord].Contains(word));
                        }
                        else
                        {
                           word = selectGenericWord(tWord);
                        }
                        Debug.WriteLine("--Chose: " + word);
                        name += word;
                        //TODO? worth fixing that words will still be removed if name was too long initially
                        masterWordList[tWord].Remove(word);
                        if (masterWordList[tWord].Count == 0)
                            SetupCardNameOptions(tWord.Replace("_", ""));
                    }
                    else
                        name += tWord;
                    name += " ";
                }
                name = name.Trim();
            } while (previousNames.Contains(name) || name.Length > 40);
            previousNames.Add(name);
            return name;
        }

        private List<string> getElementsAndEffects(Card card)
        {
            var assoList = new List<string>();
            var effectList = new List<string>();
            var selectionList = new List<string>();
            foreach (var el in card.elements.GetElements())
                assoList.Add(el.ToString());
            foreach (var ef in card.effects)
            {
                foreach (var asso in keywords)
                {
                    string efStr = ef.GetType().Name;
                    if (efStr.Contains(asso))
                        assoList.Add(asso);
                }
            }
            return assoList;
        }

        private string selectWord(List<string> givenList)
        {
            int roll = (int)(rng.NextDouble() * givenList.Count);
            string word = givenList[roll];
            Debug.WriteLine("Found: " + word);
            givenList.Remove(word);
            return word;
        }

        private string selectGenericWord(string wordType)
        {
            string word = "";
            if (wordLists[wordType][GENERIC_KEY].Count > 0)
            {
                int roll = (int)(rng.NextDouble() * wordLists[wordType][GENERIC_KEY].Count);
                word = wordLists[wordType][GENERIC_KEY][roll];
                wordLists[wordType][GENERIC_KEY].Remove(word);
            }
            else
            {
                Debug.WriteLine("############## Resorted to masterList ###################");
                int roll = (int)(rng.NextDouble() * masterWordList[wordType].Count);
                word = masterWordList[wordType][roll];
            }
            return word;
        }

        private List<string> selectTemplate(Card card)
        {
            //TODO look at card target for land types
            string type = "Standard";
            var target = card.Target.Print();
            int roll = (int)(rng.NextDouble() * 100);
            if (target.Contains("spirit") && roll < 90)
                type = "Spirit-Targeting";
            if (target.Contains("dahan") && roll < 75)
                type = "Dahan-Targeting";
            int tType = (int)(rng.NextDouble() * 100);
            roll = (int)(rng.NextDouble() * templates[type].Count);
            var template = templates[type][roll];
            templates[type].RemoveAt(roll);
            if (templates[type].Count == 0)
                SetupTemplateOptions(type);
            return template;
        }

        private int countFillInWords(List<string> template)
        {
            int count = 0;
            foreach (string word in template)
            {
                if (word.Contains("__"))
                    count++;
            }
            return count;
        }

        private void SetupCardNameOptions(string singleType = "no")
        {
            var types = new List<string>() { "Nouns", "Adjectives", "Verbs", "Possessives", "SILocations" };
            if (singleType != "no")
            {
                types.Clear();
                types.Add(singleType);
            }
            foreach (var type in types)
            {
                string typeKey = $"__{type}__";
                masterWordList[typeKey] = new List<string>();
                wordLists[typeKey] = new Dictionary<string, List<string>>();
                wordLists[typeKey][GENERIC_KEY] = new List<string>();
                foreach (string assoKey in keywords)
                {
                    wordLists[typeKey][assoKey] = new List<string>();
                }
                var lineList = getFileLineList(type);
                foreach (var line in lineList)
                {
                    var wordAndType = line.Split("#");
                    var word = wordAndType[0].Trim();
                    if (wordAndType.Length == 2)
                    {
                        var assoList = wordAndType[1].Trim().Split(" ");
                        foreach (string asso in assoList)
                        {
                            wordLists[typeKey][asso.Trim()].Add(word);
                        }
                    }
                    else
                        wordLists[typeKey][GENERIC_KEY].Add(word);
                    masterWordList[typeKey].Add(word);
                }
            }
        }

        private void SetupTemplateOptions(string singleType = "no")
        {
            var types = new List<string>() { "Standard", "Spirit-Targeting", "Dahan-Targeting" };
            if (singleType != "no")
            {
                types.Clear();
                types.Add(singleType);
            }
            foreach (var type in types)
            {
                var lines = getFileLineList($"{type} Templates");
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

        private List<string> getFileLineList(string filename)
        {
            return File.ReadAllLines(Path.Combine(cardNameDir, $"{filename}.txt")).ToList();
        }
    }
}