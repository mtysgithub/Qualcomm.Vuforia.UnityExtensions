using System;
using UnityEditor;

public class TargetDataPostprocessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool flag = false;
        foreach (string str in importedAssets)
        {
            if (str.IndexOf("Assets/StreamingAssets/QCAR/", StringComparison.OrdinalIgnoreCase) != -1)
            {
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            foreach (string str2 in deletedAssets)
            {
                if (str2.IndexOf("Assets/StreamingAssets/QCAR/", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag)
        {
            foreach (string str3 in movedAssets)
            {
                if (str3.IndexOf("Assets/StreamingAssets/QCAR/", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    flag = true;
                    break;
                }
            }
        }
        if (flag)
        {
            SceneManager.Instance.FilesUpdated();
        }
    }

    public enum ImportState
    {
        NONE,
        ADDED,
        RENAMED,
        DELETED
    }
}

