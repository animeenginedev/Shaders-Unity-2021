using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class TSVPostProcessor : AssetPostprocessor
{
    public static string[] replacements =
    {
        "FileFromGoogle", "simple_name",
        "SecondFileFromGoogle", "second_simple_name"

    };

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            bool foundTsv = false;
            if (str.EndsWith(".tsv"))
            {
                foundTsv = true;

                FileInfo fi = new FileInfo(str);
                if (fi.Exists)
                {
                    string replaceName = str.Replace(".tsv", ".csv");
                    for (int i = 0; i < replacements.Length; i += 2)
                    {
                        replaceName = replaceName.Replace(replacements[i], replacements[i + 1]);
                    }

                    FileInfo fle = new FileInfo(replaceName);
                    if (fle.Exists)
                    {
                        AssetDatabase.DeleteAsset(replaceName);
                        fle.Delete();
                    }

                    fi.MoveTo(replaceName);
                    FileInfo fimeta = new FileInfo(str + ".meta");
                    if (fimeta.Exists)
                        fimeta.Delete();
                }
            }
            if (foundTsv)
                AssetDatabase.Refresh();
        }

    }
}
