// Assets/Scripts/Localization/LocalizationManager.cs
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    private static LocalizationTextTable _textTable;
    private static Dictionary<string, LocalizedTextRow> _textMap;

    public static Language CurrentLanguage { get; private set; } = Language.English;

    public static void Initialize(LocalizationTextTable table, Language defaultLanguage)
    {
        _textTable = table;
        CurrentLanguage = defaultLanguage;

        _textMap = new Dictionary<string, LocalizedTextRow>();
        foreach (var row in _textTable.rows)
        {
            if (!string.IsNullOrEmpty(row.key))
                _textMap[row.key] = row;
        }
    }

    public static void SetLanguage(Language language)
    {
        CurrentLanguage = language;
        // Optionally: notify listeners to refresh UI
    }

    public static string Get(string key)
    {
        if (_textMap == null || !_textMap.TryGetValue(key, out var row))
        {
            // Fallback: show the key itself so missing entries are obvious
            return key;
        }

        return CurrentLanguage switch
        {
            Language.Japanese => string.IsNullOrEmpty(row.japanese) ? row.english : row.japanese,
            _ => row.english
        };
    }
}
