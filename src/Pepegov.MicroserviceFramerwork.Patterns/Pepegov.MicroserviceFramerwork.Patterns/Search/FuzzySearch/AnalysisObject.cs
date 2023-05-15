namespace Pepegov.MicroserviceFramerwork.Patterns.Search.FuzzySearch;

public class AnalysisObject
{
    public string Original { get; set; }
    public List<Word> Words { get; set; } = new List<Word>();
}