using System;
using System.Diagnostics;
using UnityEditor;

public class QCARHelpMenu : Editor
{
    [UnityEditor.MenuItem("Vuforia/Vuforia Documentation", false, 0)]
    public static void browseQCARHelp()
    {
        Process.Start("https://developer.vuforia.com/resources/dev-guide/getting-started");
    }

    [UnityEditor.MenuItem("Vuforia/Release Notes", false, 1)]
    public static void browseQCARReleaseNotes()
    {
        Process.Start("https://developer.vuforia.com/resources/sdk/unity-extensions-vuforia-v20#sdkReleaseNotes");
    }
}

