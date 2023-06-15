namespace Pepegov.UnitOfWork.Search.FuzzySearch;

public class DistanceLevenshtein
{
    private readonly char[] Separator;
    List<LanguageSet> Samples { get; set; } = new List<LanguageSet>();


    public DistanceLevenshtein(char[] separators)
    {
        Separator = separators;
    }
    
    public void SetData(List<Tuple<string, string>> datas)
    {
        List<KeyValuePair<char, int>> codeKeys = LanguageLayouts.CodeKeysRus.Concat(LanguageLayouts.CodeKeysEng).ToList();
        foreach (var data in datas)
        {
            LanguageSet languageSet = new LanguageSet();
            languageSet.Rus.Original = data.Item1;
            if (data.Item1.Length > 0)
            {
                languageSet.Rus.Words = data.Item1.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Select(w =>
                    new Word()
                    {
                        Text = w.ToLower(),
                        Codes = GetKeyCodes(codeKeys, w)
                    }).ToList();
            }

            languageSet.Eng.Original = Transliterate(data.Item2.ToLower(), LanguageLayouts.TransliterationRusToEng);
            if (data.Item2.Length > 0)
            {
                languageSet.Eng.Words = data.Item2.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Select(w =>
                    new Word()
                    {
                        Text = w.ToLower(),
                        Codes = GetKeyCodes(codeKeys, w)
                    }).ToList();
            }

            Samples.Add(languageSet);
        }
    }

    public List<Tuple<string, string, double, int>> Search(string targetStr)
    {
        List<KeyValuePair<char, int>> codeKeys = LanguageLayouts.CodeKeysRus.Concat(LanguageLayouts.CodeKeysEng).ToList();
        AnalysisObject originalSearchObj = new AnalysisObject();
        if (targetStr.Length > 0)
        {
            originalSearchObj.Words = targetStr.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Select(w =>
                new Word()
                {
                    Text = w.ToLower(),
                    Codes = GetKeyCodes(codeKeys, w)
                }).ToList();
        }

        AnalysisObject translationSearchObj = new AnalysisObject();
        if (targetStr.Length > 0)
        {
            translationSearchObj.Words = targetStr.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Select(w =>
            {
                string translateStr = Transliterate(w.ToLower(), LanguageLayouts.TransliterationRusToEng);
                return new Word()
                {
                    Text = translateStr,
                    Codes = GetKeyCodes(codeKeys, translateStr)
                };
            }).ToList();
        }

        var result = new List<Tuple<string, string, double, int>>();
        foreach (LanguageSet sampl in Samples)
        {
            int languageType = 1;
            double cost = GetRangePhrase(sampl.Rus, originalSearchObj, false);
            double tempCost = GetRangePhrase(sampl.Eng, originalSearchObj, false);
            if (cost > tempCost)
            {
                cost = tempCost;
                languageType = 3;
            }

            // Check transliterated string
            tempCost = GetRangePhrase(sampl.Rus, translationSearchObj, true);
            if (cost > tempCost)
            {
                cost = tempCost;
                languageType = 2;
            }

            tempCost = GetRangePhrase(sampl.Eng, translationSearchObj, true);
            if (cost > tempCost)
            {
                cost = tempCost;
                languageType = 3;
            }

            result.Add(new Tuple<string, string, double, int>(sampl.Rus.Original.ToLower(),
                sampl.Eng.Original.ToLower(), cost, languageType));
        }

        return result;
    }

    private double GetRangePhrase(AnalysisObject source, AnalysisObject search, bool translation)
    {
        if (!source.Words.Any())
        {
            if (!search.Words.Any())
                return 0;
            return search.Words.Sum(w => w.Text.Length) * 2 * 100;
        }

        if (!search.Words.Any())
        {
            return source.Words.Sum(w => w.Text.Length) * 2 * 100;
        }

        double result = 0;
        for (int i = 0; i < search.Words.Count; i++)
        {
            double minRangeWord = double.MaxValue;
            int minIndex = 0;
            for (int j = 0; j < source.Words.Count; j++)
            {
                double currentRangeWord = GetRangeWord(source.Words[j], search.Words[i], translation);
                if (currentRangeWord < minRangeWord)
                {
                    minRangeWord = currentRangeWord;
                    minIndex = j;
                }
            }

            result += minRangeWord * 100 + Math.Abs(i - minIndex) / 10.0;
        }

        return result;
    }

    private double GetRangeWord(Word source, Word target, bool translation)
    {
        double minDistance = double.MaxValue;
        Word croppedSource = new Word();
        int length = Math.Min(source.Text.Length, target.Text.Length + 1);
        for (int i = 0; i <= source.Text.Length - length; i++)
        {
            croppedSource.Text = source.Text.Substring(i, length);
            croppedSource.Codes = source.Codes.Skip(i).Take(length).ToList();
            minDistance = Math.Min(minDistance,
                LevenshteinDistance(croppedSource, target, croppedSource.Text.Length == source.Text.Length,
                    translation) + i * 2 / 10.0);
        }

        return minDistance;
    }

    private int LevenshteinDistance(Word source, Word target, bool fullWord, bool translation)
    {
        if (string.IsNullOrEmpty(source.Text))
        {
            if (string.IsNullOrEmpty(target.Text))
                return 0;
            return target.Text.Length * 2;
        }

        if (string.IsNullOrEmpty(target.Text))
            return source.Text.Length * 2;
        int n = source.Text.Length;
        int m = target.Text.Length;

        int[,] distance = new int[3, m + 1];
        // Initialize the distance 'matrix'
        for (var j = 1; j <= m; j++)
            distance[0, j] = j * 2;
        var currentRow = 0;
        for (var i = 1; i <= n; ++i)
        {
            currentRow = i % 3;
            var previousRow = (i - 1) % 3;
            distance[currentRow, 0] = i * 2;
            for (var j = 1; j <= m; j++)
            {
                distance[currentRow, j] = Math.Min(Math.Min(
                        distance[previousRow, j] + (!fullWord && i == n ? 2 - 1 : 2),
                        distance[currentRow, j - 1] + (!fullWord && i == n ? 2 - 1 : 2)),
                    distance[previousRow, j - 1] + CostDistanceSymbol(source, i - 1, target, j - 1, translation));

                if (i > 1 && j > 1 && source.Text[i - 1] == target.Text[j - 2]
                    && source.Text[i - 2] == target.Text[j - 1])
                {
                    distance[currentRow, j] = Math.Min(distance[currentRow, j], distance[(i - 2) % 3, j - 2] + 2);
                }
            }
        }

        return distance[currentRow, m];
    }

    private int CostDistanceSymbol(Word source, int sourcePosition, Word search, int searchPosition, bool translation)
    {
        if (source.Text[sourcePosition] == search.Text[searchPosition])
            return 0;
        if (translation)
            return 2;
        if (source.Codes[sourcePosition] != 0 && source.Codes[sourcePosition] == search.Codes[searchPosition])
            return 0;
        int resultWeight = 0;
        List<int> nearKeys;
        if (!LanguageLayouts.DistanceCodeKey.TryGetValue(source.Codes[sourcePosition], out nearKeys))
            resultWeight = 2;
        else
            resultWeight = nearKeys.Contains(search.Codes[searchPosition]) ? 1 : 2;
        List<char> phoneticGroups;
        if (PhoneticGroupsRus.TryGetValue(search.Text[searchPosition], out phoneticGroups))
            resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
        if (PhoneticGroupsEng.TryGetValue(search.Text[searchPosition], out phoneticGroups))
            resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
        return resultWeight;
    }

    private List<int> GetKeyCodes(List<KeyValuePair<char, int>> codeKeys, string word)
    {
        return word.ToLower().Select(ch => codeKeys.FirstOrDefault(ck => ck.Key == ch).Value).ToList();
    }

    private string Transliterate(string text, Dictionary<char, string> cultureFrom)
    {
        IEnumerable<char> translateText = text.SelectMany(t =>
        {
            string translateChar;
            if (cultureFrom.TryGetValue(t, out translateChar))
                return translateChar;
            return t.ToString();
        });
        return string.Concat(translateText);
    }

    #region Блок Фонетических групп

    static Dictionary<char, List<char>> PhoneticGroupsRus = new Dictionary<char, List<char>>();
    static Dictionary<char, List<char>> PhoneticGroupsEng = new Dictionary<char, List<char>>();

    #endregion

    static DistanceLevenshtein()
    {
        SetPhoneticGroups(PhoneticGroupsRus, new List<string>() { "ыий", "эе", "ая", "оёе", "ую", "шщ", "оа" });
        SetPhoneticGroups(PhoneticGroupsEng,
            new List<string>() { "aeiouy", "bp", "ckq", "dt", "lr", "mn", "gj", "fpv", "sxz", "csz" });
    }

    private static void SetPhoneticGroups(Dictionary<char, List<char>> resultPhoneticGroups,
        List<string> phoneticGroups)
    {
        foreach (string group in phoneticGroups)
        foreach (char symbol in group)
            if (!resultPhoneticGroups.ContainsKey(symbol))
                resultPhoneticGroups.Add(symbol,
                    phoneticGroups.Where(pg => pg.Contains(symbol)).SelectMany(pg => pg).Distinct()
                        .Where(ch => ch != symbol).ToList());
    }
}
