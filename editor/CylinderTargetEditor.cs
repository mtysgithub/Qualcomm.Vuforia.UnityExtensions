using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CylinderTargetAbstractBehaviour), true)]
public class CylinderTargetEditor : Editor
{
    private const bool INSIDE_MATERIAL = true;
    private const int NUM_PERIMETER_VERTICES = 0x20;

    private static void CheckMesh(IEditorCylinderTargetBehaviour editorCtb)
    {
        GameObject gameObject = editorCtb.gameObject;
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (((component == null) || (component.sharedMesh == null)) || (((renderer == null) || (renderer.sharedMaterials.Length == 0)) || (renderer.sharedMaterials[0] == null)))
        {
            ConfigData.CylinderTargetData data2;
            ConfigDataManager.Instance.GetConfigData(editorCtb.DataSetName).GetCylinderTarget(editorCtb.TrackableName, out data2);
            UpdateAspectRatio(editorCtb, data2);
        }
    }

    public void OnEnable()
    {
        CylinderTargetAbstractBehaviour target = (CylinderTargetAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            IEditorCylinderTargetBehaviour ct = target;
            if (!ct.InitializedInEditor && !EditorApplication.isPlaying)
            {
                ConfigData.CylinderTargetData data;
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetCylinderTarget("--- EMPTY ---", out data);
                ct.SetDataSetPath("--- EMPTY ---");
                ct.SetNameForTrackable("--- EMPTY ---");
                UpdateAspectRatio(ct, data);
                UpdateScale(ct, data.sideLength);
                ct.SetInitializedInEditor(true);
            }
            else if (!EditorApplication.isPlaying)
            {
                CheckMesh(ct);
            }
            ct.SetPreviousScale(target.transform.localScale);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        CylinderTargetAbstractBehaviour target = (CylinderTargetAbstractBehaviour) base.target;
        IEditorCylinderTargetBehaviour ct = target;
        if (QCARUtilities.GetPrefabType(target) == PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.", new GUILayoutOption[0]);
        }
        else if (ConfigDataManager.Instance.NumConfigDataObjects > 1)
        {
            string[] configDataNames = new string[ConfigDataManager.Instance.NumConfigDataObjects];
            ConfigDataManager.Instance.GetConfigDataNames(configDataNames);
            int indexFromString = QCARUtilities.GetIndexFromString(ct.DataSetName, configDataNames);
            if (indexFromString < 0)
            {
                indexFromString = 0;
            }
            int index = EditorGUILayout.Popup("Data Set", indexFromString, configDataNames, new GUILayoutOption[0]);
            string dataSetName = configDataNames[index];
            ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
            string[] arrayToFill = new string[configData.NumCylinderTargets];
            configData.CopyCylinderTargetNames(arrayToFill, 0);
            int selectedIndex = QCARUtilities.GetIndexFromString(ct.TrackableName, arrayToFill);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }
            int num4 = EditorGUILayout.Popup("Cylinder Target", selectedIndex, arrayToFill, new GUILayoutOption[0]);
            if ((arrayToFill.Length > 0) && ((index != indexFromString) || (num4 != selectedIndex)))
            {
                ConfigData.CylinderTargetData data2;
                ct.SetDataSetPath("QCAR/" + configDataNames[index] + ".xml");
                ct.SetNameForTrackable(arrayToFill[num4]);
                configData.GetCylinderTarget(target.TrackableName, out data2);
                UpdateAspectRatio(ct, data2);
                UpdateScale(ct, data2.sideLength);
            }
            float num5 = EditorGUILayout.FloatField("Side Length", target.SideLength, new GUILayoutOption[0]);
            if (num5 != target.SideLength)
            {
                target.SetSideLength(num5);
            }
            float num6 = EditorGUILayout.FloatField("Top Diameter ", target.TopDiameter, new GUILayoutOption[0]);
            if (num6 != target.TopDiameter)
            {
                target.SetTopDiameter(num6);
            }
            float num7 = EditorGUILayout.FloatField("Bottom Diameter", target.BottomDiameter, new GUILayoutOption[0]);
            if (num7 != target.BottomDiameter)
            {
                target.SetBottomDiameter(num7);
            }
            ct.SetExtendedTracking(EditorGUILayout.Toggle("Extended tracking", ct.ExtendedTracking, new GUILayoutOption[0]));
            ct.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", ct.PreserveChildSize, new GUILayoutOption[0]));
        }
        else if (GUILayout.Button("No targets defined. Press here for target creation!", new GUILayoutOption[0]))
        {
            SceneManager.Instance.GoToTargetManagerPage();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            SceneManager.Instance.SceneUpdated();
        }
    }

    public static void UpdateAspectRatio(IEditorCylinderTargetBehaviour ct, ConfigData.CylinderTargetData ctConfig)
    {
        float topRatio = ctConfig.topDiameter / ctConfig.sideLength;
        float bottomRatio = ctConfig.bottomDiameter / ctConfig.sideLength;
        ct.SetAspectRatio(topRatio, bottomRatio);
        UpdateGeometry(ct, 1f, topRatio, bottomRatio, ctConfig.hasTopGeometry, ctConfig.hasBottomGeometry);
        UpdateMaterials(ct, ctConfig.hasBottomGeometry, ctConfig.hasTopGeometry, true);
    }

    private static void UpdateGeometry(IEditorCylinderTargetBehaviour ct, float sideLength, float topDiameter, float bottomDiameter, bool hasTopGeometry, bool hasBottomGeometry)
    {
        GameObject gameObject = ct.gameObject;
        MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
        if (component == null)
        {
            component = gameObject.AddComponent<MeshRenderer>();
        }
        component.hideFlags = HideFlags.NotEditable;
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }
        filter.hideFlags = HideFlags.NotEditable;
        Mesh mesh = CylinderMeshFactory.CreateCylinderMesh(sideLength, topDiameter, bottomDiameter, 0x20, hasTopGeometry, hasBottomGeometry, true);
        filter.sharedMesh = mesh;
        MaskOutAbstractBehaviour behaviour = gameObject.GetComponent<MaskOutAbstractBehaviour>();
        if (behaviour == null)
        {
            Material material = (Material) AssetDatabase.LoadAssetAtPath("Assets/Qualcomm Augmented Reality/Materials/DepthMask.mat", typeof(Material));
            behaviour = BehaviourComponentFactory.Instance.AddMaskOutBehaviour(gameObject);
            behaviour.maskMaterial = material;
        }
        behaviour.hideFlags = HideFlags.NotEditable;
        EditorUtility.UnloadUnusedAssets();
    }

    private static void UpdateMaterials(IEditorCylinderTargetBehaviour ct, bool hasBottomGeometry, bool hasTopGeometry, bool insideMaterial)
    {
        string assetPath = "Assets/Qualcomm Augmented Reality/Materials/DefaultTarget.mat";
        Material source = (Material) AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
        if (source == null)
        {
            Debug.LogError("Could not find reference material at " + assetPath + " please reimport Unity package.");
        }
        else
        {
            string str2 = "Assets/Editor/QCAR/CylinderTargetTextures/" + ct.DataSetName + "/" + ct.TrackableName;
            List<string> list = new List<string> {
                str2 + ".Body_scaled"
            };
            if (hasBottomGeometry)
            {
                list.Add(str2 + ".Bottom_scaled");
            }
            if (hasTopGeometry)
            {
                list.Add(str2 + ".Top_scaled");
            }
            int count = list.Count;
            Material[] materialArray = new Material[insideMaterial ? (count * 2) : count];
            for (int i = 0; i < list.Count; i++)
            {
                string str3 = list[i];
                if (File.Exists(str3 + ".png"))
                {
                    str3 = str3 + ".png";
                }
                else if (File.Exists(str3 + ".jpg"))
                {
                    str3 = str3 + ".jpg";
                }
                else if (File.Exists(str3 + ".jpeg"))
                {
                    str3 = str3 + ".jpeg";
                }
                else
                {
                    materialArray[i] = source;
                    if (insideMaterial)
                    {
                        materialArray[i + count] = source;
                    }
                    continue;
                }
                Texture2D textured = (Texture2D) AssetDatabase.LoadAssetAtPath(str3, typeof(Texture2D));
                Material material2 = new Material(source);
                if (textured != null)
                {
                    material2.mainTexture = textured;
                    material2.name = textured.name + "Material";
                    material2.mainTextureScale = new Vector2(-1f, -1f);
                }
                materialArray[i] = material2;
                if (insideMaterial)
                {
                    Material material3 = new Material(material2) {
                        shader = Shader.Find("Custom/BrightTexture")
                    };
                    materialArray[i + count] = material3;
                }
            }
            ct.renderer.sharedMaterials = materialArray;
            Resources.UnloadUnusedAssets();
        }
    }

    public static void UpdateScale(IEditorCylinderTargetBehaviour ct, float scale)
    {
        float num = ct.transform.localScale.x / scale;
        ct.transform.localScale = new Vector3(scale, scale, scale);
        if (ct.PreserveChildSize)
        {
            foreach (Transform transform in ct.transform)
            {
                transform.localPosition = new Vector3(transform.localPosition.x * num, transform.localPosition.y * num, transform.localPosition.z * num);
                transform.localScale = new Vector3(transform.localScale.x * num, transform.localScale.y * num, transform.localScale.z * num);
            }
        }
    }
}

