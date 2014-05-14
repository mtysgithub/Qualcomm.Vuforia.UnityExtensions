using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CloudRecoAbstractBehaviour), true)]
public class CloudRecoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CloudRecoAbstractBehaviour target = (CloudRecoAbstractBehaviour) base.target;
        EditorGUILayout.HelpBox("Credentials for authenticating with the CloudReco service.\nThese are read-only access keys for accessing the image database specific to this sample application - the keys should be replaced by your own access keys. You should be very careful how you share your credentials, especially with untrusted third parties, and should take the appropriate steps to protect them within your application code.", MessageType.Info);
        target.AccessKey = EditorGUILayout.TextField("Access Key", target.AccessKey, new GUILayoutOption[0]).Trim();
        target.SecretKey = EditorGUILayout.TextField("Secret Key", target.SecretKey, new GUILayoutOption[0]).Trim();
        EditorGUILayout.HelpBox("You can use these color fields to configure the scanline UI to match the color scheme of your app.", MessageType.None);
        target.ScanlineColor = EditorGUILayout.ColorField("Scanline", target.ScanlineColor, new GUILayoutOption[0]);
        target.FeaturePointColor = EditorGUILayout.ColorField("Feature Points", target.FeaturePointColor, new GUILayoutOption[0]);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    public void OnSceneGUI()
    {
        CloudRecoAbstractBehaviour target = (CloudRecoAbstractBehaviour) base.target;
        GUIStyle style2 = new GUIStyle {
            alignment = TextAnchor.LowerRight,
            fontSize = 0x12
        };
        style2.normal.textColor = Color.white;
        GUIStyle style = style2;
        Handles.Label(target.transform.position, "Cloud\nRecognition", style);
    }
}

