using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MTRenderingChecker
{
    private static bool mLoggedErrorOnce;

    static MTRenderingChecker()
    {
        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(MTRenderingChecker.CheckForMTRenderingSetting));
    }

    private static void CheckForMTRenderingSetting()
    {
        if (!mLoggedErrorOnce && Application.unityVersion.StartsWith("4.3"))
        {
            PropertyInfo property = typeof(PlayerSettings).GetProperty("mobileMTRendering", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            if ((property != null) && ((bool) property.GetValue(null, null)))
            {
                Debug.LogWarning("Vuforia does not support multithreaded rendering on Android.\nPlease disable it in the Player Settings before building for Android.");
                mLoggedErrorOnce = true;
            }
        }
    }
}

