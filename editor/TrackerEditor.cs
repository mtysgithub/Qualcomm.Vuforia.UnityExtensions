using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QCARAbstractBehaviour), true)]
public class TrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        QCARAbstractBehaviour target = (QCARAbstractBehaviour) base.target;
        base.DrawDefaultInspector();
        target.SetWorldCenterMode((QCARAbstractBehaviour.WorldCenterMode) EditorGUILayout.EnumPopup("World Center Mode", target.WorldCenterModeSetting, new GUILayoutOption[0]));
        bool allowSceneObjects = !EditorUtility.IsPersistent(base.target);
        if (target.WorldCenterModeSetting == QCARAbstractBehaviour.WorldCenterMode.SPECIFIC_TARGET)
        {
            TrackableBehaviour behaviour2 = (TrackableBehaviour) EditorGUILayout.ObjectField("World Center", target.WorldCenter, typeof(TrackableBehaviour), allowSceneObjects, new GUILayoutOption[0]);
            if (behaviour2 is WordAbstractBehaviour)
            {
                behaviour2 = null;
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Word behaviours cannot be selected as world center."));
            }
            target.SetWorldCenter(behaviour2);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

