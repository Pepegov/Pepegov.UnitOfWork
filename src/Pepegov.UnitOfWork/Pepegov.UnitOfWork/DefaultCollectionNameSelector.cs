using System.Text.RegularExpressions;

namespace Pepegov.UnitOfWork;

/// <summary>
/// Default collection name selector
/// </summary>
public sealed class DefaultCollectionNameSelector : ICollectionNameSelector
{
    /// <summary>
    /// Returns a name for entity by it type
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns>name</returns>
    public string GetCollectionName(string typeName)
    {
        var words = typeName.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
        var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)", m => m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value);

        var tailWords = words.Skip(1)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .ToArray();

        return $"{leadWord}{string.Join(string.Empty, tailWords)}";
    }
}