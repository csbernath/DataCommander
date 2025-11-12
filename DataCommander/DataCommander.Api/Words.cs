using System;
using System.Diagnostics;

namespace DataCommander.Api;

public sealed class Words
{
    public static void AssertOrder(string[] wordsArray)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        string? previousWord = null;
        foreach (var currentWord in wordsArray)
        {
            if (previousWord != null)
            {
                var comparisonResult = comparer.Compare(previousWord, currentWord);
                if (comparisonResult >= 0)
                    Debug.WriteLine($"previousWord: {previousWord}, currentWord: {currentWord}");
            }

            previousWord = currentWord;
        }
    }
}