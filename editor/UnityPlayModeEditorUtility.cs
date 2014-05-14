using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class UnityPlayModeEditorUtility : IPlayModeEditorUtility
{
    static UnityPlayModeEditorUtility()
    {
        PlayModeEditorUtility.Instance = new UnityPlayModeEditorUtility();
    }

    private static void CheckToStartPlayMode()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(UnityPlayModeEditorUtility.CheckToStartPlayMode));
            EditorApplication.isPlaying = true;
            Debug.LogWarning("Restarted Play Mode because scripts have been recompiled.");
        }
    }

    public void DisplayDialog(string title, string message, string ok)
    {
        EditorUtility.DisplayDialog(title, message, ok);
    }

    private static string GetValueOfChildNodeByName(XmlNode parentNode, string name)
    {
        foreach (XmlNode node in parentNode.ChildNodes)
        {
            if (node.Name.Equals(name))
            {
                return node.InnerXml;
            }
        }
        return "";
    }

    public WebCamProfile.ProfileCollection LoadAndParseWebcamProfiles(string path)
    {
        WebCamProfile.ProfileData data;
        Dictionary<string, WebCamProfile.ProfileData> profiles = new Dictionary<string, WebCamProfile.ProfileData>();
        try
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            foreach (XmlNode node in document.GetElementsByTagName("webcam"))
            {
                profiles[node.Attributes["deviceName"].Value.ToLower()] = this.ParseConfigurationEntry(node);
            }
            data = this.ParseConfigurationEntry(document.GetElementsByTagName("default")[0]);
        }
        catch (Exception exception)
        {
            string message = "Exception occurred when trying to parse web cam profile file: " + exception.Message;
            EditorUtility.DisplayDialog("Error occurred!", message, "Ok");
            Debug.LogError(message);
            data = new WebCamProfile.ProfileData {
                RequestedFPS = 30,
                RequestedTextureSize = new QCARRenderer.Vec2I(640, 480),
                ResampledTextureSize = new QCARRenderer.Vec2I(640, 480)
            };
        }
        return new WebCamProfile.ProfileCollection(data, profiles);
    }

    private WebCamProfile.ProfileData ParseConfigurationEntry(XmlNode cameraNode)
    {
        foreach (XmlNode node in cameraNode.ChildNodes)
        {
            string str = "undefined";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str = "windows";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                str = "osx";
            }
            if (node.Name.Equals(str))
            {
                return new WebCamProfile.ProfileData { RequestedTextureSize = new QCARRenderer.Vec2I(int.Parse(GetValueOfChildNodeByName(node, "requestedTextureWidth")), int.Parse(GetValueOfChildNodeByName(node, "requestedTextureHeight"))), ResampledTextureSize = new QCARRenderer.Vec2I(int.Parse(GetValueOfChildNodeByName(node, "resampledTextureWidth")), int.Parse(GetValueOfChildNodeByName(node, "resampledTextureHeight"))), RequestedFPS = 30 };
            }
        }
        throw new Exception("Could not parse webcam profile: " + cameraNode.InnerXml);
    }

    public void RestartPlayMode()
    {
        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(UnityPlayModeEditorUtility.CheckToStartPlayMode));
        EditorApplication.isPlaying = false;
    }
}

