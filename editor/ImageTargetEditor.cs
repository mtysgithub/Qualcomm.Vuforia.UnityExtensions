using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ImageTargetAbstractBehaviour), true)]
public class ImageTargetEditor : Editor
{
    private static void AddVirtualButton(ImageTargetAbstractBehaviour it, ConfigData.VirtualButtonData vb)
    {
        IEditorVirtualButtonBehaviour behaviour = it.CreateVirtualButton(vb.name, new Vector2(0f, 0f), new Vector2(1f, 1f));
        if (behaviour != null)
        {
            behaviour.SetPosAndScaleFromButtonArea(new Vector2(vb.rectangle[0], vb.rectangle[1]), new Vector2(vb.rectangle[2], vb.rectangle[3]));
            VirtualButtonEditor.CreateVBMesh(behaviour);
            VirtualButtonEditor.CreateMaterial(behaviour);
            behaviour.enabled = vb.enabled;
            BehaviourComponentFactory.Instance.AddTurnOffBehaviour(behaviour.gameObject);
            behaviour.UpdatePose();
        }
        else
        {
            Debug.LogError("VirtualButton could not be added!");
        }
    }

    public static void AddVirtualButtons(ImageTargetAbstractBehaviour it, ConfigData.VirtualButtonData[] vbs)
    {
        for (int i = 0; i < vbs.Length; i++)
        {
            AddVirtualButton(it, vbs[i]);
        }
    }

    private static void CheckMesh(IEditorImageTargetBehaviour it)
    {
        GameObject gameObject = it.gameObject;
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        if ((component == null) || (component.sharedMesh == null))
        {
            UpdateMesh(it);
        }
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (((renderer == null) || (renderer.sharedMaterials.Length == 0)) || (renderer.sharedMaterials[0] == null))
        {
            UpdateMaterial(it);
        }
    }

    private void DrawCloudRecoTargetInspectorUI(IEditorImageTargetBehaviour itb, bool typeChanged)
    {
        if (typeChanged)
        {
            ConfigData.ImageTargetData data = QCARUtilities.CreateDefaultImageTarget();
            itb.SetNameForTrackable(string.Empty);
            UpdateAspectRatio(itb, data.size);
            UpdateMaterial(itb);
        }
        itb.SetExtendedTracking(EditorGUILayout.Toggle("Extended tracking", itb.ExtendedTracking, new GUILayoutOption[0]));
        itb.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", itb.PreserveChildSize, new GUILayoutOption[0]));
    }

    private void DrawPredefinedTargetInsprectorUI(IEditorImageTargetBehaviour itb, bool typeChanged)
    {
        if (typeChanged)
        {
            UpdateMaterial(itb);
        }
        if (ConfigDataManager.Instance.NumConfigDataObjects > 1)
        {
            string[] configDataNames = new string[ConfigDataManager.Instance.NumConfigDataObjects];
            ConfigDataManager.Instance.GetConfigDataNames(configDataNames);
            int indexFromString = QCARUtilities.GetIndexFromString(itb.DataSetName, configDataNames);
            if (indexFromString < 0)
            {
                indexFromString = 0;
            }
            int index = EditorGUILayout.Popup("Data Set", indexFromString, configDataNames, new GUILayoutOption[0]);
            string dataSetName = configDataNames[index];
            ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
            string[] arrayToFill = new string[configData.NumImageTargets];
            configData.CopyImageTargetNames(arrayToFill, 0);
            int selectedIndex = QCARUtilities.GetIndexFromString(itb.TrackableName, arrayToFill);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }
            if (index != indexFromString)
            {
                selectedIndex = 0;
            }
            int num5 = EditorGUILayout.Popup("Image Target", selectedIndex, arrayToFill, new GUILayoutOption[0]);
            float width = EditorGUILayout.FloatField("Width", itb.GetSize().x, new GUILayoutOption[0]);
            if (width != itb.GetSize().x)
            {
                itb.SetWidth(width);
            }
            float height = EditorGUILayout.FloatField("Height ", itb.GetSize().y, new GUILayoutOption[0]);
            if (height != itb.GetSize().y)
            {
                itb.SetHeight(height);
            }
            itb.SetExtendedTracking(EditorGUILayout.Toggle("Extended tracking", itb.ExtendedTracking, new GUILayoutOption[0]));
            itb.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", itb.PreserveChildSize, new GUILayoutOption[0]));
            if ((arrayToFill.Length > 0) && (((index != indexFromString) || (num5 != selectedIndex)) || typeChanged))
            {
                ConfigData.ImageTargetData data2;
                itb.SetDataSetPath("QCAR/" + configDataNames[index] + ".xml");
                string name = arrayToFill[num5];
                itb.SetNameForTrackable(name);
                configData.GetImageTarget(itb.TrackableName, out data2);
                UpdateAspectRatio(itb, data2.size);
                UpdateScale(itb, data2.size);
                UpdateMaterial(itb);
            }
        }
        else if (GUILayout.Button("No targets defined. Press here for target creation!", new GUILayoutOption[0]))
        {
            SceneManager.Instance.GoToTargetManagerPage();
        }
    }

    private void DrawUserDefinedTargetInspectorUI(IEditorImageTargetBehaviour itb, bool typeChanged)
    {
        if (typeChanged)
        {
            ConfigData.ImageTargetData data = QCARUtilities.CreateDefaultImageTarget();
            itb.SetNameForTrackable(string.Empty);
            UpdateAspectRatio(itb, data.size);
            UpdateMaterial(itb);
        }
        if (itb.TrackableName.Length > 0x40)
        {
            EditorGUILayout.HelpBox("Target name must not exceed 64 character limit!", MessageType.Error);
        }
        itb.SetNameForTrackable(EditorGUILayout.TextField("Target Name", itb.TrackableName, new GUILayoutOption[0]));
        itb.SetExtendedTracking(EditorGUILayout.Toggle("Extended tracking", itb.ExtendedTracking, new GUILayoutOption[0]));
        itb.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", itb.PreserveChildSize, new GUILayoutOption[0]));
    }

    public void OnEnable()
    {
        ImageTargetAbstractBehaviour target = (ImageTargetAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            IEditorImageTargetBehaviour it = target;
            if (!it.InitializedInEditor && !EditorApplication.isPlaying)
            {
                ConfigData.ImageTargetData data;
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetImageTarget("--- EMPTY ---", out data);
                UpdateAspectRatio(target, data.size);
                UpdateScale(target, data.size);
                UpdateMaterial(target);
                it.SetDataSetPath("--- EMPTY ---");
                it.SetNameForTrackable("--- EMPTY ---");
                it.SetInitializedInEditor(true);
            }
            else if (!EditorApplication.isPlaying)
            {
                CheckMesh(it);
            }
            it.SetPreviousScale(target.transform.localScale);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        ImageTargetAbstractBehaviour target = (ImageTargetAbstractBehaviour) base.target;
        IEditorImageTargetBehaviour behaviour2 = target;
        if (QCARUtilities.GetPrefabType(target) == PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.", new GUILayoutOption[0]);
        }
        else
        {
            ImageTargetType imageTargetType = behaviour2.ImageTargetType;
            string[] displayedOptions = new string[] { "Predefined", "User Defined", "Cloud Reco" };
            ImageTargetType[] typeArray2 = new ImageTargetType[3];
            typeArray2[1] = ImageTargetType.USER_DEFINED;
            typeArray2[2] = ImageTargetType.CLOUD_RECO;
            ImageTargetType[] source = typeArray2;
            behaviour2.SetImageTargetType(source[EditorGUILayout.Popup("Type", source.ToList<ImageTargetType>().IndexOf(behaviour2.ImageTargetType), displayedOptions, new GUILayoutOption[0])]);
            bool typeChanged = behaviour2.ImageTargetType != imageTargetType;
            if (behaviour2.ImageTargetType == ImageTargetType.PREDEFINED)
            {
                this.DrawPredefinedTargetInsprectorUI(target, typeChanged);
            }
            else if (behaviour2.ImageTargetType == ImageTargetType.USER_DEFINED)
            {
                this.DrawUserDefinedTargetInspectorUI(target, typeChanged);
            }
            else
            {
                this.DrawCloudRecoTargetInspectorUI(target, typeChanged);
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            SceneManager.Instance.SceneUpdated();
        }
    }

    public static void UpdateAspectRatio(IEditorImageTargetBehaviour it, Vector2 size)
    {
        it.SetAspectRatio(size[1] / size[0]);
        UpdateMesh(it);
    }

    public static void UpdateMaterial(IEditorImageTargetBehaviour it)
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
            string str3 = str2 + it.DataSetName + "/" + it.TrackableName + "_scaled";
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
                if (it.ImageTargetType == ImageTargetType.USER_DEFINED)
                {
                    it.renderer.sharedMaterial = (Material) AssetDatabase.LoadAssetAtPath("Assets/Qualcomm Augmented Reality/Materials/UserDefinedTarget.mat", typeof(Material));
                }
                else if (it.ImageTargetType == ImageTargetType.CLOUD_RECO)
                {
                    it.renderer.sharedMaterial = (Material) AssetDatabase.LoadAssetAtPath("Assets/Qualcomm Augmented Reality/Materials/CloudRecoTarget.mat", typeof(Material));
                }
                else
                {
                    it.renderer.sharedMaterial = source;
                }
            }
            else
            {
                Material material2 = new Material(source) {
                    mainTexture = textured,
                    name = textured.name + "Material",
                    mainTextureScale = new Vector2(1f, 1f)
                };
                it.renderer.sharedMaterial = material2;
                EditorUtility.UnloadUnusedAssets();
            }
        }
    }

    private static void UpdateMesh(IEditorImageTargetBehaviour it)
    {
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        Vector3 vector4;
        GameObject gameObject = it.gameObject;
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        if (component == null)
        {
            component = gameObject.AddComponent<MeshFilter>();
        }
        if (it.AspectRatio <= 1f)
        {
            vector = new Vector3(-0.5f, 0f, -it.AspectRatio * 0.5f);
            vector2 = new Vector3(-0.5f, 0f, it.AspectRatio * 0.5f);
            vector3 = new Vector3(0.5f, 0f, -it.AspectRatio * 0.5f);
            vector4 = new Vector3(0.5f, 0f, it.AspectRatio * 0.5f);
        }
        else
        {
            float num = 1f / it.AspectRatio;
            vector = new Vector3(-num * 0.5f, 0f, -0.5f);
            vector2 = new Vector3(-num * 0.5f, 0f, 0.5f);
            vector3 = new Vector3(num * 0.5f, 0f, -0.5f);
            vector4 = new Vector3(num * 0.5f, 0f, 0.5f);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { vector, vector2, vector3, vector4 };
        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        mesh.normals = new Vector3[mesh.vertices.Length];
        mesh.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };
        mesh.RecalculateNormals();
        component.mesh = mesh;
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        }
        EditorUtility.UnloadUnusedAssets();
    }

    public static void UpdateScale(IEditorImageTargetBehaviour it, Vector2 size)
    {
        float num = it.GetSize()[0] / size[0];
        if (it.AspectRatio <= 1f)
        {
            it.transform.localScale = new Vector3(size[0], size[0], size[0]);
        }
        else
        {
            it.transform.localScale = new Vector3(size[1], size[1], size[1]);
        }
        if (it.PreserveChildSize)
        {
            foreach (Transform transform in it.transform)
            {
                transform.localPosition = new Vector3(transform.localPosition.x * num, transform.localPosition.y * num, transform.localPosition.z * num);
                transform.localScale = new Vector3(transform.localScale.x * num, transform.localScale.y * num, transform.localScale.z * num);
            }
        }
    }

    public static void UpdateVirtualButtons(ImageTargetAbstractBehaviour it, ConfigData.VirtualButtonData[] vbs)
    {
        for (int i = 0; i < vbs.Length; i++)
        {
            IEditorVirtualButtonBehaviour[] componentsInChildren = it.GetComponentsInChildren<VirtualButtonAbstractBehaviour>();
            bool flag = false;
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                if (componentsInChildren[j].VirtualButtonName == vbs[i].name)
                {
                    componentsInChildren[j].enabled = vbs[i].enabled;
                    componentsInChildren[j].SetSensitivitySetting(vbs[i].sensitivity);
                    componentsInChildren[j].SetPosAndScaleFromButtonArea(new Vector2(vbs[i].rectangle[0], vbs[i].rectangle[1]), new Vector2(vbs[i].rectangle[2], vbs[i].rectangle[3]));
                    flag = true;
                }
            }
            if (!flag)
            {
                AddVirtualButton(it, vbs[i]);
            }
        }
    }
}

