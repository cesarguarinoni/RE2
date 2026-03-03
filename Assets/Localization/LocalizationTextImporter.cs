// Assets/Scripts/Localization/Editor/LocalizationTextImporter.cs
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LocalizationTextImporter
{
    private const char Separator = ',';

    [MenuItem("Tools/Localization/Import Text CSV")]
    public static void Import()
    {
        // Adjust these paths as needed
        string csvPath = "Assets/Localization/LocalizationText.csv";
        string assetPath = "Assets/Localization/LocalizationTextTable.asset";

        var csvAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
        if (csvAsset == null)
        {
            Debug.LogError($"CSV not found at path: {csvPath}");
            return;
        }

        var table = AssetDatabase.LoadAssetAtPath<LocalizationTextTable>(assetPath);
        if (table == null)
        {
            table = ScriptableObject.CreateInstance<LocalizationTextTable>();
            AssetDatabase.CreateAsset(table, assetPath);
        }

        table.rows.Clear();

        using (StringReader reader = new StringReader(csvAsset.text))
        {
            string line;
            bool isHeader = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var cols = line.Split(Separator);

                if (isHeader)
                {
                    // Expect: Key, English, Japanese
                    isHeader = false;
                    continue;
                }

                // basic safety
                if (cols.Length < 3)
                    continue;

                var row = new LocalizedTextRow
                {
                    key = cols[0].Trim(),
                    english = cols[1].Trim(),
                    japanese = cols[2].Trim()
                };

                if (!string.IsNullOrEmpty(row.key))
                    table.rows.Add(row);
            }
        }

        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();

        Debug.Log($"Localization CSV imported. Rows: {table.rows.Count}");
    }
}
#endif
