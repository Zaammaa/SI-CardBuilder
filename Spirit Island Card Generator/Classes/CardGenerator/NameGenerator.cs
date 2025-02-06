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
            keywords = GetFileLineList("Association Keywords");
            SetupCardNameOptions();
            SetupTemplateOptions();
        }

        public string GenerateCardName(Card card)
        {
            string name = "";
            var template = SelectTemplate(card);
            do
            {
                var remainingTypes = CountFillInWords(template);
                var assoList = GetElementsAndEffects(card);
                var pluralNouns = new List<string>();
                name = "";
                foreach (var templateWord in template)
                {
                    var tWord = templateWord;
                    if (tWord == "__Nouns__")
                        tWord = ChooseRandomWord(["__NounsSingular__", "__NounsPlural__"]);

                    if (wordLists.ContainsKey(tWord))
                    {
                        remainingTypes--;
                        string word;
                        word = GenerateTemplateWord(tWord, assoList, remainingTypes);
                        // Make sure the plural of this noun isn't already in the name (rare chance)
                        if (tWord == "__NounsPlural__")
                            pluralNouns.Add(word);
                        if (tWord == "__NounsSingular__") {
                            while (pluralNouns.Contains(CreatePluralNouns(word)[1]))
                                word = GenerateTemplateWord(tWord, assoList, remainingTypes);
                        }
                        name += word;
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
            string word = "";
            string asso1 = SelectWord(assoList);
            string asso2 = null;
            if (assoList.Count > remainingTypes)
                asso2 = SelectWord(assoList);
            bool canUseAsso1 = wordLists[type].ContainsKey(asso1) && wordLists[type][asso1].Count > 0;
            bool canUseAsso2 = asso2 != null && wordLists[type].ContainsKey(asso2) && wordLists[type][asso2].Count > 0;

            if (canUseAsso1 || canUseAsso2)
            {
                do
                {
                    if (canUseAsso1 && canUseAsso2)
                    {
                        var intersection = wordLists[type][asso1].Intersect(wordLists[type][asso2]);
                        var interList = intersection.ToList();
                        if (interList.Count > 0)
                        {
                            word = ChooseRandomWord(interList);
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
                        word = SelectGenericWord(type);
                    canUseAsso1 = wordLists[type][asso1].Count > 0;
                    canUseAsso2 = canUseAsso2 && wordLists[type][asso2].Count > 0;
                } while (!masterWordList[type].Contains(word));
            }
            else
            {
                word = SelectGenericWord(type);
            }
            //TODO? worth fixing that words will still be removed if name was too long initially
            masterWordList[type].Remove(word);
            if (masterWordList[type].Count == 0)
                SetupCardNameOptions(type.Replace("_", ""));
            if (type.Contains("Singular"))
            {
                string plural = CreatePluralNouns(word)[1];
                masterWordList[type.Replace("Singular", "Plural")].Remove(plural);
            }
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
                    if (asso == "RemoveBlight" && efStr.Contains("BlightDoesNotCascade"))
                        assoList.Add(asso);
                    else if (asso == "Blight" && (efStr.Contains("RemoveBlight") || efStr.Contains("BlightDoesNotCascade")))
                    {
                        // Do nothing
                    }
                    else if (asso == "Spirit" && efStr.Contains("Presence"))
                        assoList.Add(asso);
                    else if (asso == "Invader" && (efStr.Contains("Ravage") || efStr.Contains("Build")))
                        assoList.Add(asso);
                    else if (asso == "Damage")
                    {
                        if (efStr.Contains("Damage") && !efStr.Contains("ReducedDamage"))
                            assoList.Add(asso);
                        else if (efStr.Contains("Destroy") && !efStr.Contains("DestroyFewer") && !efStr.Contains("DestroyedPresence"))
                            assoList.Add(asso);
                        else if (efStr.Contains("Downgrade"))
                            assoList.Add(asso);
                    }
                    else if (efStr.Contains(asso))
                        assoList.Add(asso);
                    //TODO update effect classes to be consistent
                    else if (asso == "Wilds" && efStr.Contains("Wild"))
                        assoList.Add(asso);
                    else if (asso == "Badlands" && efStr.Contains("Badland"))
                        assoList.Add(asso);
                }
            }
            return assoList;
        }

        private string SelectWord(List<string> givenList)
        {
            string word = ChooseRandomWord(givenList);
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
                word = ChooseRandomWord(masterWordList[wordType]);
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
                List<string> typeKey = [$"__{type}__"];
                // typeKey will have 2 things in it for singular/plural word types
                if (type == "Nouns")
                {
                    typeKey = [$"__{type}Singular__", $"__{type}Plural__"];
                }
                foreach (string key in typeKey)
                {
                    masterWordList[key] = new List<string>();
                    wordLists[key] = new Dictionary<string, List<string>>();
                    wordLists[key][GENERIC_KEY] = new List<string>();

                    foreach (string assoKey in keywords)
                    {
                        wordLists[key][assoKey] = new List<string>();
                    }
                    var lineList = GetFileLineList(type);
                    foreach (var line in lineList)
                    {
                        var wordAndType = line.Split("#");
                        var word = wordAndType[0].Trim();
                        if (type == "Nouns")
                        {
                            var wordPair = CreatePluralNouns(word);
                            if (wordPair[0] != "" && key.Contains("Singular"))
                                word = wordPair[0];
                            else if (wordPair[1] != "" && key.Contains("Plural"))
                                word = wordPair[1];
                            else // No word for that plurality
                                continue;
                        }


                        if (wordAndType.Length == 2)
                        {
                            var assoList = wordAndType[1].Trim().Split(" ");
                            foreach (string asso in assoList)
                            {
                                wordLists[key][asso.Trim()].Add(word);
                            }
                        }
                        else
                            wordLists[key][GENERIC_KEY].Add(word);
                        masterWordList[key].Add(word);
                    }
                }
            }
        }

        private List<string> CreatePluralNouns(string word)
        {
            var wordPair = new List<string>() { "", "" };
            var splitWord = word.Split("/");
            if (splitWord.Length == 2)
            {
                wordPair[0] = splitWord[0].Trim();
                wordPair[1] = splitWord[1].Trim();
            }
            else
            {
                string vowels = "aeiou";
                word = splitWord[0].Trim().ToString();
                wordPair[0] = word;
                var endIndex = word.Length - 1;
                var endLetter = word[endIndex].ToString();
                var secondToEnd = word[endIndex - 1].ToString();
                bool endsInY = endLetter == "y";
                bool endsInVowelY = endsInY && vowels.Contains(secondToEnd);
                bool endsInO = endLetter == "o";
                bool endsInS = endLetter == "s";
                bool endsInX = endLetter == "x";
                bool endsInCH = endLetter == "h" && secondToEnd == "c";
                bool endsInSH = endLetter == "h" && secondToEnd == "s";
                bool endsInFE = endLetter == "e" && secondToEnd == "f";
                if (endsInY && !endsInVowelY)
                {
                    word = word.Remove(endIndex);
                    word += "ies";
                }
                else if (endsInO || endsInS || endsInX || endsInCH || endsInSH)
                    word += "es";
                else if (endsInFE)
                {
                    word = word.Remove(endIndex - 1);
                    word += "ves";
                }
                else if (word == "Child")
                    word += "ren";
                else
                {
                    if (word != "Dahan")
                    {
                        word += "s";
                    }
                }
                wordPair[1] = word;
            }
            return wordPair;
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