namespace Pepegov.MicroserviceFramerwork.Patterns.Search.Temp.FuzzySearch;

public class AnalysisObject
{
    public string Original { get; set; }
    public List<Word> Words { get; set; } = new List<Word>();
}