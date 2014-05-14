using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebCamAbstractBehaviour), true)]
public class WebCamEditor : Editor
{
    private const string NO_CAMERAS_TEXT = "NO CAMERAS FOUND";
    private static DateTime sLastDeviceListRefresh = (DateTime.Now - TimeSpan.FromDays(1.0));
    private static string[] sWebCamDeviceNames = new string[0];
    private static bool sWebCamDevicesReadOnce = false;
    private static WebCamProfile sWebCamProfiles;

    private string[] GetDeviceNames()
    {
        if ((!EditorApplication.isPlaying || !sWebCamDevicesReadOnce) && ((DateTime.Now - DeviceListRefreshInterval) > sLastDeviceListRefresh))
        {
            try
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                int length = WebCamTexture.devices.Length;
                if (length > 0)
                {
                    sWebCamDeviceNames = new string[length];
                    for (int i = 0; i < length; i++)
                    {
                        sWebCamDeviceNames[i] = devices[i].name;
                    }
                }
                else
                {
                    sWebCamDeviceNames = new string[] { "NO CAMERAS FOUND" };
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
            }
            sWebCamDevicesReadOnce = true;
            sLastDeviceListRefresh = DateTime.Now;
        }
        return sWebCamDeviceNames;
    }

    public void OnEnable()
    {
        WebCamAbstractBehaviour target = (WebCamAbstractBehaviour) base.target;
        if ((QCARUtilities.GetPrefabType(target) != PrefabType.Prefab) && !SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }
    }

    public override void OnInspectorGUI()
    {
        if (!EditorApplication.isPlaying)
        {
            WebCamAbstractBehaviour target = (WebCamAbstractBehaviour) base.target;
            if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
            {
                if (target.BackgroundCameraPrefab == null)
                {
                    UnityEngine.Object obj2 = AssetDatabase.LoadAssetAtPath("Assets/Qualcomm Augmented Reality/Prefabs/Internal/BackgroundCamera.prefab", typeof(Camera));
                    if (obj2 != null)
                    {
                        target.BackgroundCameraPrefab = obj2 as Camera;
                    }
                }
                target.TurnOffWebCam = EditorGUILayout.Toggle("Don't use for Play Mode", target.TurnOffWebCam, new GUILayoutOption[0]);
                if (!target.TurnOffWebCam)
                {
                    if (!target.CheckNativePluginSupport())
                    {
                        EditorGUILayout.HelpBox("Play Mode requires a Unity Pro license!", MessageType.Warning);
                    }
                    int index = 0;
                    string[] deviceNames = this.GetDeviceNames();
                    for (int i = 0; i < deviceNames.Length; i++)
                    {
                        if ((deviceNames[i] != null) && deviceNames[i].Equals(target.DeviceName))
                        {
                            index = i;
                        }
                    }
                    if (sWebCamProfiles == null)
                    {
                        sWebCamProfiles = new WebCamProfile();
                    }
                    string str = deviceNames[index];
                    if (str.Equals("NO CAMERAS FOUND"))
                    {
                        EditorGUILayout.HelpBox("No camera connected!\nTo run your application using Play Mode, please connect a webcam to your computer.", MessageType.Warning);
                    }
                    else if (!sWebCamProfiles.ProfileAvailable(deviceNames[index]))
                    {
                        EditorGUILayout.HelpBox("Webcam profile not found!\nUnfortunately there is no profile for your webcam model: '" + deviceNames[index] + "'.\n\nThe default profile will been used. You can configure a profile yourself by editing '" + Path.Combine(Application.dataPath, "Editor/QCAR/WebcamProfiles/profiles.xml") + "'.", MessageType.Warning);
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel("Camera Device");
                    int num3 = EditorGUILayout.Popup(index, deviceNames, new GUILayoutOption[0]);
                    if ((num3 != index) && !deviceNames[num3].Equals("NO CAMERAS FOUND"))
                    {
                        target.DeviceName = deviceNames[num3];
                    }
                    EditorGUILayout.EndHorizontal();
                    target.FlipHorizontally = EditorGUILayout.Toggle("Flip Horizontally", target.FlipHorizontally, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Here you can enter the index of the layer that will be used internally for our render to texture functionality. the ARCamera will be configured to not draw this layer.", MessageType.None);
                    target.RenderTextureLayer = EditorGUILayout.IntField("Render Texture Layer", target.RenderTextureLayer, new GUILayoutOption[0]);
                }
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                    SceneManager.Instance.SceneUpdated();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Webcam settings cannot be changed during Play Mode.", MessageType.Info);
            }
        }
    }

    private static TimeSpan DeviceListRefreshInterval
    {
        get
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                return TimeSpan.FromMilliseconds(500.0);
            }
            return TimeSpan.FromMilliseconds(5000.0);
        }
    }
}

