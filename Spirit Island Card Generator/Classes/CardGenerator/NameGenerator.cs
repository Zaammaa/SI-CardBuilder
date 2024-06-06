using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

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
            keywords = GetFileLineList("Association Keywords");
            SetupCardNameOptions();
            SetupTemplateOptions();
        }

        public string GenerateCardName(Card card)
        {
            string name = "";
            var template = SelectTemplate(card);
            Debug.WriteLine(template);
            do
            {
                var remainingTypes = CountFillInWords(template);
                var assoList = GetElementsAndEffects(card);
                name = "";
                foreach (var tWord in template)
                {
                    if (wordLists.ContainsKey(tWord))
                    {
                        remainingTypes--;
                        name += GenerateTemplateWord(tWord, assoList, remainingTypes);
                        if (assoList.Count == 0)
                            assoList = GetElementsAndEffects(card);
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

        private string GenerateTemplateWord(string type, List<string> assoList, int remainingTypes)
        {
            Debug.WriteLine("*********************************");
            string word = "";
            string asso1 = SelectWord(assoList);
            string asso2 = null;
            Debug.WriteLine("NumTypes: " + remainingTypes + " assoList.Count " + assoList.Count);
            if (assoList.Count > remainingTypes)
                asso2 = SelectWord(assoList);
            Debug.WriteLine("Part of speech: " + type + " Chosen asso: " + asso1 + " and " + asso2);
            bool canUseAsso1 = wordLists[type].ContainsKey(asso1) && wordLists[type][asso1].Count > 0;
            bool canUseAsso2 = asso2 != null && wordLists[type].ContainsKey(asso2) && wordLists[type][asso2].Count > 0;

            if (canUseAsso1 || canUseAsso2)
            {
                do
                {
                    Debug.WriteLine("Using asso: " + asso1 + " and " + asso2);
                    if (canUseAsso1 && canUseAsso2)
                    {
                        var intersection = wordLists[type][asso1].Intersect(wordLists[type][asso2]);
                        Debug.WriteLine("Created intersection, size " + intersection.ToList().Count);
                        var interList = intersection.ToList();
                        if (interList.Count > 0)
                        {
                            word = ChooseRandomWord(interList);
                            Debug.WriteLine("Completed and got word: " + word);
                            wordLists[type][asso1].Remove(word);
                            wordLists[type][asso2].Remove(word);
                            continue;
                        }
                    }

                    if (canUseAsso1)
                        word = SelectWord(wordLists[type][asso1]);
                    else if (canUseAsso2)
                        word = SelectWord(wordLists[type][asso2]);
                    else
                    {
                        word = SelectGenericWord(type);
                        Debug.WriteLine("Have to use Generic, got " + word);

                    }
                    canUseAsso1 = wordLists[type][asso1].Count > 0;
                    canUseAsso2 = canUseAsso2 && wordLists[type][asso2].Count > 0;
                } while (!masterWordList[type].Contains(word));
            }
            else
            {
                word = SelectGenericWord(type);
            }
            Debug.WriteLine("--Chose: " + word);
            //TODO? worth fixing that words will still be removed if name was too long initially
            masterWordList[type].Remove(word);
            if (masterWordList[type].Count == 0)
                SetupCardNameOptions(type.Replace("_", ""));
            return word;
        }

        private List<string> GetElementsAndEffects(Card card)
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

        private string SelectWord(List<string> givenList)
        {
            string word = ChooseRandomWord(givenList);
            Debug.WriteLine("Found: " + word);
            givenList.Remove(word);
            return word;
        }

        private string SelectGenericWord(string wordType)
        {
            string word = "";
            if (wordLists[wordType][GENERIC_KEY].Count > 0)
            {
                word = ChooseRandomWord(wordLists[wordType][GENERIC_KEY]);
                wordLists[wordType][GENERIC_KEY].Remove(word);
            }
            else
            {
                Debug.WriteLine("############## Resorted to masterList ###################");
                word = ChooseRandomWord(masterWordList[wordType]);
            }
            return word;
        }

        private List<string> SelectTemplate(Card card)
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

        private int CountFillInWords(List<string> template)
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
                var lineList = GetFileLineList(type);
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
                var lines = GetFileLineList($"{type} Templates");
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

        private List<string> GetFileLineList(string filename)
        {
            return File.ReadAllLines(Path.Combine(cardNameDir, $"{filename}.txt")).ToList();
        }

        private string ChooseRandomWord(List<string> wordList)
        {
            int roll = (int)(rng.NextDouble() * wordList.Count);
            return wordList[roll];
        }
    }
}