using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserDefinedTargetBuildingAbstractBehaviour), true)]
public class UserDefinedTargetBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UserDefinedTargetBuildingAbstractBehaviour target = (UserDefinedTargetBuildingAbstractBehaviour) base.target;
        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.HelpBox("If this is enabled, the Target Builder will begin to automatically scan the frame for feature points on startup.", MessageType.None);
        EditorGUIUtility.LookLikeInspector();
        target.StartScanningAutomatically = EditorGUILayout.Toggle("Start scanning automatically", target.StartScanningAutomatically, new GUILayoutOption[0]);
        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.HelpBox("Check this if you want to automatically disable the ImageTracker while the Target Builder is scanning. Once scanning mode is stopped, the ImageTracker will be enabled again.", MessageType.None);
        EditorGUIUtility.LookLikeInspector();
        target.StopTrackerWhileScanning = EditorGUILayout.Toggle("Stop tracker while scanning", target.StopTrackerWhileScanning, new GUILayoutOption[0]);
        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.HelpBox("If this is enabled, scanning will be automatically stopped when a new target has been created.", MessageType.None);
        EditorGUIUtility.LookLikeInspector();
        target.StopScanningWhenFinshedBuilding = EditorGUILayout.Toggle("Stop scanning after creating target", target.StopScanningWhenFinshedBuilding, new GUILayoutOption[0]);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    public void OnSceneGUI()
    {
        UserDefinedTargetBuildingAbstractBehaviour target = (UserDefinedTargetBuildingAbstractBehaviour) base.target;
        GUIStyle style2 = new GUIStyle {
            alignment = TextAnchor.LowerRight,
            fontSize = 0x12
        };
        style2.normal.textColor = Color.white;
        GUIStyle style = style2;
        Handles.Label(target.transform.position, "User Defined\n      Target Builder", style);
    }
}

