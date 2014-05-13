using System;

internal class PlayModeEditorUtility
{
    private static IPlayModeEditorUtility sInstance;

    public static IPlayModeEditorUtility Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new NullPlayModeEditorUtility();
            }
            return sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    private class NullPlayModeEditorUtility : IPlayModeEditorUtility
    {
        public void DisplayDialog(string title, string message, string ok)
        {
        }

        public WebCamProfile.ProfileCollection LoadAndParseWebcamProfiles(string path)
        {
            return new WebCamProfile.ProfileCollection();
        }

        public void RestartPlayMode()
        {
        }
    }
}

