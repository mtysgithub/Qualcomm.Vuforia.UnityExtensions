using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetBGCameraLayerAbstractBehaviour), true)]
public class SetBGCameraLayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SetBGCameraLayerAbstractBehaviour target = (SetBGCameraLayerAbstractBehaviour) base.target;
        EditorGUILayout.HelpBox("Here you can enter the index of the layer that will be used as the layer for background that is rendered for this camera. \nThe ARCamera will also be configured to not draw this layer.", MessageType.None);
        target.CameraLayer = EditorGUILayout.IntField("Render Texture Layer", target.CameraLayer, new GUILayoutOption[0]);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
            {
                SceneManager.Instance.SceneUpdated();
            }
        }
    }
}

