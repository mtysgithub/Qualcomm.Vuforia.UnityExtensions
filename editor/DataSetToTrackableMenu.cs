using System;
using UnityEditor;

public class DataSetToTrackableMenu : Editor
{
    [UnityEditor.MenuItem("Vuforia/Apply Data Set Properties", false, 2)]
    public static void ApplyDataSetProperties()
    {
        SceneManager.Instance.ApplyDataSetProperties();
    }
}

