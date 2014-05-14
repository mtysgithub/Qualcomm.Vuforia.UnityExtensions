using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiTargetAbstractBehaviour), true)]
public class MultiTargetEditor : Editor
{
    private static void CheckMesh(MultiTargetAbstractBehaviour mtb)
    {
        bool flag = false;
        Transform transform = mtb.transform.Find("ChildTargets");
        if (transform == null)
        {
            flag = true;
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                MeshFilter component = child.GetComponent<MeshFilter>();
                MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                if (((component == null) || (component.sharedMesh == null)) || (((renderer == null) || (renderer.sharedMaterials.Length == 0)) || (renderer.sharedMaterials[0] == null)))
                {
                    flag = true;
                }
            }
        }
        if (flag)
        {
            TrackableAccessor accessor = AccessorFactory.Create(mtb);
            if (accessor != null)
            {
                accessor.ApplyDataSetProperties();
            }
        }
    }

    public void OnEnable()
    {
        MultiTargetAbstractBehaviour target = (MultiTargetAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            IEditorMultiTargetBehaviour mt = target;
            if (!mt.InitializedInEditor && !EditorApplication.isPlaying)
            {
                ConfigData.MultiTargetData data;
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetMultiTarget("--- EMPTY ---", out data);
                mt.SetDataSetPath("--- EMPTY ---");
                mt.SetNameForTrackable("--- EMPTY ---");
                UpdateParts(mt, data.parts.ToArray());
                mt.SetInitializedInEditor(true);
            }
            else if (!EditorApplication.isPlaying)
            {
                CheckMesh(target);
            }
            mt.SetPreviousScale(target.transform.localScale);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        MultiTargetAbstractBehaviour target = (MultiTargetAbstractBehaviour) base.target;
        IEditorMultiTargetBehaviour behaviour2 = target;
        if (QCARUtilities.GetPrefabType(target) == PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.", new GUILayoutOption[0]);
        }
        else if (ConfigDataManager.Instance.NumConfigDataObjects > 1)
        {
            string[] configDataNames = new string[ConfigDataManager.Instance.NumConfigDataObjects];
            ConfigDataManager.Instance.GetConfigDataNames(configDataNames);
            int indexFromString = QCARUtilities.GetIndexFromString(behaviour2.DataSetName, configDataNames);
            if (indexFromString < 0)
            {
                indexFromString = 0;
            }
            int index = EditorGUILayout.Popup("Data Set", indexFromString, configDataNames, new GUILayoutOption[0]);
            string dataSetName = configDataNames[index];
            ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
            string[] arrayToFill = new string[configData.NumMultiTargets];
            configData.CopyMultiTargetNames(arrayToFill, 0);
            int selectedIndex = QCARUtilities.GetIndexFromString(behaviour2.TrackableName, arrayToFill);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }
            int num4 = EditorGUILayout.Popup("Multi Target", selectedIndex, arrayToFill, new GUILayoutOption[0]);
            if ((arrayToFill.Length > 0) && ((index != indexFromString) || (num4 != selectedIndex)))
            {
                behaviour2.SetDataSetPath("QCAR/" + configDataNames[index] + ".xml");
                behaviour2.SetNameForTrackable(arrayToFill[num4]);
            }
            behaviour2.SetExtendedTracking(EditorGUILayout.Toggle("Extended tracking", behaviour2.ExtendedTracking, new GUILayoutOption[0]));
        }
        else if (GUILayout.Button("No targets defined. Press here for target creation!", new GUILayoutOption[0]))
        {
            SceneManager.Instance.GoToTargetManagerPage();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            TrackableAccessor accessor = AccessorFactory.Create(target);
            if (accessor != null)
            {
                accessor.ApplyDataSetProperties();
            }
            SceneManager.Instance.SceneUpdated();
        }
    }

    public void OnSceneGUI()
    {
        TrackableBehaviour target = (TrackableBehaviour) base.target;
        if (((target.transform.localScale.x != 1f) || (target.transform.localScale.y != 1f)) || (target.transform.localScale.z != 1f))
        {
            Debug.LogError("You cannot scale a Multi target in the editor. Please edit the config.xml file to scale this target.");
            target.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private static void UpdateMaterial(IEditorMultiTargetBehaviour mt, GameObject go)
    {
        string assetPath = "Assets/Qualcomm Augmented Reality/Materials/DefaultTarget.mat";
        Material source = (Material) AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
        if (source == null)
        {
            Debug.LogError("Could not find reference material at " + assetPath + " please reimport Unity package.");
        }
        else
        {
            string str2 = "Assets/Editor/QCAR/ImageTargetTextures/";
            string str3 = str2 + mt.DataSetName + "/" + go.name + "_scaled";
            if (File.Exists(str3 + ".png"))
            {
                str3 = str3 + ".png";
            }
            else if (File.Exists(str3 + ".jpg"))
            {
                str3 = str3 + ".jpg";
            }
            Texture2D textured = (Texture2D) AssetDatabase.LoadAssetAtPath(str3, typeof(Texture2D));
            if (textured == null)
            {
                go.renderer.sharedMaterial = source;
            }
            else
            {
                Material material2 = new Material(source) {
                    mainTexture = textured,
                    name = textured.name + "Material",
                    mainTextureScale = new Vector2(-1f, -1f)
                };
                go.renderer.sharedMaterial = material2;
                EditorUtility.UnloadUnusedAssets();
            }
        }
    }

    public static void UpdateParts(IEditorMultiTargetBehaviour mt, ConfigData.MultiTargetPartData[] prtConfigs)
    {
        Transform transform = mt.transform.Find("ChildTargets");
        if (transform != null)
        {
            UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
        GameObject obj2 = new GameObject {
            name = "ChildTargets"
        };
        obj2.transform.parent = mt.transform;
        obj2.hideFlags = HideFlags.NotEditable;
        obj2.transform.localPosition = Vector3.zero;
        obj2.transform.localRotation = Quaternion.identity;
        obj2.transform.localScale = Vector3.one;
        transform = obj2.transform;
        Material material = (Material) AssetDatabase.LoadAssetAtPath("Assets/Qualcomm Augmented Reality/Materials/DepthMask.mat", typeof(Material));
        ConfigData configData = ConfigDataManager.Instance.GetConfigData(mt.DataSetName);
        if (configData == null)
        {
            Debug.LogError("Could not update Multi Target parts. A data set with the given name does not exist.");
        }
        else
        {
            Mesh mesh2 = new Mesh();
            mesh2.vertices = new Vector3[] { new Vector3(-0.5f, 0f, -0.5f), new Vector3(-0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, -0.5f), new Vector3(0.5f, 0f, 0.5f) };
            mesh2.uv = new Vector2[] { new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f) };
            mesh2.normals = new Vector3[4];
            mesh2.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            Mesh mesh = mesh2;
            mesh.RecalculateNormals();
            int length = prtConfigs.Length;
            for (int i = 0; i < length; i++)
            {
                ConfigData.ImageTargetData data2;
                if (!configData.ImageTargetExists(prtConfigs[i].name))
                {
                    Debug.LogError("No Image Target named " + prtConfigs[i].name);
                    return;
                }
                configData.GetImageTarget(prtConfigs[i].name, out data2);
                Vector2 size = data2.size;
                Vector3 vector2 = new Vector3(size.x, 1f, size.y);
                GameObject go = new GameObject(prtConfigs[i].name);
                go.AddComponent<MeshRenderer>();
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.transform.parent = transform.transform;
                go.transform.localPosition = prtConfigs[i].translation;
                go.transform.localRotation = prtConfigs[i].rotation;
                go.transform.localScale = vector2;
                UpdateMaterial(mt, go);
                go.hideFlags = HideFlags.NotEditable;
                BehaviourComponentFactory.Instance.AddMaskOutBehaviour(go).maskMaterial = material;
            }
        }
    }
}

