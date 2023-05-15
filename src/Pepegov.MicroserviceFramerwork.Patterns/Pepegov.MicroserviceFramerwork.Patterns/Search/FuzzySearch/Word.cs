namespace Pepegov.MicroserviceFramerwork.Patterns.Search.FuzzySearch;

public class Word
{
    public string Text { get; set; }
    public List<int> Codes { get; set; } = new List<int>();
}