using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeepAliveAbstractBehaviour), true)]
public class KeepAliveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        KeepAliveAbstractBehaviour target = (KeepAliveAbstractBehaviour) base.target;
        EditorGUILayout.HelpBox("By keeping the objects checked below alive, they can be rused across multiple scenes.\nKeeping the ARCamera alive will result in keeping all loaded datasets available when a new scene is loaded, incluing user defined targets.\nYou can also keep Trackable prefabs like Image Targets alive, as well as advanced prefabs like e.g. CloudRecognition.", MessageType.Info);
        if (Application.isPlaying)
        {
            GUI.enabled = false;
        }
        target.KeepARCameraAlive = EditorGUILayout.Toggle("Keep AR Camera Alive", target.KeepARCameraAlive, new GUILayoutOption[0]);
        if (target.KeepARCameraAlive)
        {
            target.KeepTrackableBehavioursAlive = EditorGUILayout.Toggle("Keep Trackable Prefabs Alive", target.KeepTrackableBehavioursAlive, new GUILayoutOption[0]);
            target.KeepTextRecoBehaviourAlive = EditorGUILayout.Toggle("Keep Text Reco Prefab Alive", target.KeepTextRecoBehaviourAlive, new GUILayoutOption[0]);
            target.KeepUDTBuildingBehaviourAlive = EditorGUILayout.Toggle("Keep UDT building Prefab Alive", target.KeepUDTBuildingBehaviourAlive, new GUILayoutOption[0]);
            target.KeepCloudRecoBehaviourAlive = EditorGUILayout.Toggle("Keep Cloud Reco Prefab Alive", target.KeepCloudRecoBehaviourAlive, new GUILayoutOption[0]);
        }
        else
        {
            target.KeepTrackableBehavioursAlive = false;
            target.KeepTextRecoBehaviourAlive = false;
            target.KeepUDTBuildingBehaviourAlive = false;
            target.KeepCloudRecoBehaviourAlive = false;
        }
        GUI.enabled = true;
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

