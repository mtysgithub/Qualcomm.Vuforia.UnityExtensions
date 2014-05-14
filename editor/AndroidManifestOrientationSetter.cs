using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AndroidManifestOrientationSetter
{
    private static readonly List<string> ActivitiesToModify;
    private const string ACTIVITY_NAME_ATTR = "android:name";
    private const string ACTIVITY_NODE_NAME = "activity";
    private const string ACTIVITY_ORIENTATION_ATTR = "android:screenOrientation";
    private static bool mInitialized = false;
    private static UIOrientation mUIOrientation;
    private static readonly Dictionary<UIOrientation, string> OrientationMapping;

    static AndroidManifestOrientationSetter()
    {
        Dictionary<UIOrientation, string> dictionary = new Dictionary<UIOrientation, string>();
        dictionary.Add(UIOrientation.AutoRotation, "sensor");
        dictionary.Add(UIOrientation.LandscapeLeft, "landscape");
        dictionary.Add(UIOrientation.LandscapeRight, "reverseLandscape");
        dictionary.Add(UIOrientation.Portrait, "portrait");
        dictionary.Add(UIOrientation.PortraitUpsideDown, "reversePortrait");
        OrientationMapping = dictionary;
        ActivitiesToModify = new List<string> { "com.qualcomm.QCARUnityPlayer.QCARPlayerProxyActivity", "com.qualcomm.QCARUnityPlayer.QCARPlayerActivity", "com.qualcomm.QCARUnityPlayer.QCARPlayerNativeActivity", "com.unity3d.player.VideoPlayer" };
        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(AndroidManifestOrientationSetter.CheckAndApplyOrientationSettings));
    }

    private static void CheckAndApplyOrientationSettings()
    {
        if (!mInitialized || (PlayerSettings.defaultInterfaceOrientation != mUIOrientation))
        {
            mInitialized = true;
            mUIOrientation = PlayerSettings.defaultInterfaceOrientation;
            SetOrientationInAndroidManifest(OrientationMapping[mUIOrientation]);
        }
    }

    private static void SetOrientationInAndroidManifest(string orientationValue)
    {
        string str = "Plugins/Android/AndroidManifest.xml";
        try
        {
            string filename = Path.Combine(Application.dataPath, str);
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            bool flag = false;
            foreach (XmlNode node in document.GetElementsByTagName("activity"))
            {
                if (node.Attributes != null)
                {
                    XmlAttribute attribute = node.Attributes["android:name"];
                    if ((attribute != null) && ActivitiesToModify.Contains(attribute.Value))
                    {
                        XmlAttribute attribute2 = node.Attributes["android:screenOrientation"];
                        if (attribute2 != null)
                        {
                            if (attribute2.Value != orientationValue)
                            {
                                attribute2.Value = orientationValue;
                                flag = true;
                            }
                        }
                        else
                        {
                            attribute2 = document.CreateAttribute(":android:screenOrientation");
                            attribute2.Value = orientationValue;
                            node.Attributes.Append(attribute2);
                            flag = true;
                        }
                    }
                }
            }
            if (flag)
            {
                document.Save(filename);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Exception occurred when trying to parse web cam profile file: " + exception.Message);
            Debug.LogError("The selected orientation could not be set for the Vuforia activities in " + str + "\nMake sure to set the required orientation manually.");
        }
    }
}

