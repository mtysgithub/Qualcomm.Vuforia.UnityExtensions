using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

public class WebCamProfile
{
    private readonly ProfileCollection mProfileCollection = PlayModeEditorUtility.Instance.LoadAndParseWebcamProfiles(Path.Combine(UnityEngine.Application.dataPath, "Editor/QCAR/WebcamProfiles/profiles.xml"));

    internal ProfileData GetProfile(string webcamName)
    {
        ProfileData data;
        if (this.mProfileCollection.Profiles.TryGetValue(webcamName.ToLower(), out data))
        {
            return data;
        }
        return this.mProfileCollection.DefaultProfile;
    }

    internal bool ProfileAvailable(string webcamName)
    {
        return this.mProfileCollection.Profiles.ContainsKey(webcamName.ToLower());
    }

    public ProfileData Default
    {
        get
        {
            return this.mProfileCollection.DefaultProfile;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileCollection
    {
        public readonly WebCamProfile.ProfileData DefaultProfile;
        public readonly Dictionary<string, WebCamProfile.ProfileData> Profiles;
        public ProfileCollection(WebCamProfile.ProfileData defaultProfile, Dictionary<string, WebCamProfile.ProfileData> profiles)
        {
            this.DefaultProfile = defaultProfile;
            this.Profiles = profiles;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileData
    {
        public QCARRenderer.Vec2I RequestedTextureSize;
        public QCARRenderer.Vec2I ResampledTextureSize;
        public int RequestedFPS;
    }
}

