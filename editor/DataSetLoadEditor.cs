using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataSetLoadAbstractBehaviour), true)]
public class DataSetLoadEditor : Editor
{
    private void DrawDataSets(DataSetLoadAbstractBehaviour dslb, string[] dataSetList)
    {
        foreach (string str in dataSetList)
        {
            bool flag = dslb.mDataSetsToLoad.Contains(str);
            bool flag2 = dslb.mDataSetsToActivate.Contains(str);
            bool flag3 = EditorGUILayout.Toggle("Load Data Set " + str, flag, new GUILayoutOption[0]);
            bool flag4 = false;
            if (flag3)
            {
                flag4 = EditorGUILayout.Toggle("                     Activate", flag2, new GUILayoutOption[0]);
            }
            if (str != dataSetList[dataSetList.Length - 1])
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
            if (flag && !flag3)
            {
                dslb.mDataSetsToLoad.Remove(str);
            }
            else if (!flag && flag3)
            {
                dslb.mDataSetsToLoad.Add(str);
            }
            if (flag2 && !flag4)
            {
                dslb.mDataSetsToActivate.Remove(str);
            }
            else if (!flag2 && flag4)
            {
                dslb.mDataSetsToActivate.Add(str);
            }
        }
    }

    public static void OnConfigDataChanged()
    {
        Predicate<string> match = null;
        Predicate<string> predicate2 = null;
        string[] dataSetList = new string[ConfigDataManager.Instance.NumConfigDataObjects - 1];
        ConfigDataManager.Instance.GetConfigDataNames(dataSetList, false);
        DataSetLoadAbstractBehaviour[] behaviourArray = (DataSetLoadAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(DataSetLoadAbstractBehaviour));
        foreach (DataSetLoadAbstractBehaviour behaviour in behaviourArray)
        {
            if (match == null)
            {
                match = s => Array.Find<string>(dataSetList, str => str.Equals(s)) == null;
            }
            behaviour.mDataSetsToActivate.RemoveAll(match);
            if (predicate2 == null)
            {
                predicate2 = s => Array.Find<string>(dataSetList, str => str.Equals(s)) == null;
            }
            behaviour.mDataSetsToLoad.RemoveAll(predicate2);
        }
    }

    public void OnEnable()
    {
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        DataSetLoadAbstractBehaviour target = (DataSetLoadAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            string[] configDataNames = new string[ConfigDataManager.Instance.NumConfigDataObjects - 1];
            ConfigDataManager.Instance.GetConfigDataNames(configDataNames, false);
            this.DrawDataSets(target, configDataNames);
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

