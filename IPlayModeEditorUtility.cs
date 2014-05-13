using System;

internal interface IPlayModeEditorUtility
{
    void DisplayDialog(string title, string message, string ok);
    WebCamProfile.ProfileCollection LoadAndParseWebcamProfiles(string path);
    void RestartPlayMode();
}

