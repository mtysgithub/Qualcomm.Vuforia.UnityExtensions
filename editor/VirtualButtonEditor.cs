using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VirtualButtonAbstractBehaviour), true)]
public class VirtualButtonEditor : Editor
{
    public static bool CorrectPoses(IEditorVirtualButtonBehaviour[] vbs)
    {
        bool flag = false;
        foreach (IEditorVirtualButtonBehaviour behaviour in vbs)
        {
            if (((behaviour.PreviousTransform != behaviour.transform.localToWorldMatrix) || ((behaviour.transform.parent != null) && (behaviour.PreviousParent != behaviour.transform.parent.gameObject))) || !behaviour.HasUpdatedPose)
            {
                if (behaviour.UpdatePose())
                {
                    flag = true;
                }
                behaviour.SetPreviousTransform(behaviour.transform.localToWorldMatrix);
                behaviour.SetPreviousParent((behaviour.transform.parent != null) ? behaviour.transform.parent.gameObject : null);
            }
        }
        return flag;
    }

    public static void CreateMaterial(IEditorVirtualButtonBehaviour vb)
    {
        string assetPath = "Assets/Editor/QCAR/VirtualButtonTextures/VirtualButtonPreviewMaterial.mat";
        Material material = (Material) AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
        if (material == null)
        {
            Debug.LogError("Could not find reference material at " + assetPath + " please reimport Unity package.");
        }
        else
        {
            vb.renderer.sharedMaterial = material;
            EditorUtility.UnloadUnusedAssets();
        }
    }

    public static void CreateVBMesh(IEditorVirtualButtonBehaviour vb)
    {
        GameObject gameObject = vb.gameObject;
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        if (component == null)
        {
            component = gameObject.AddComponent<MeshFilter>();
        }
        Vector3 vector = new Vector3(-0.5f, 0f, -0.5f);
        Vector3 vector2 = new Vector3(-0.5f, 0f, 0.5f);
        Vector3 vector3 = new Vector3(0.5f, 0f, -0.5f);
        Vector3 vector4 = new Vector3(0.5f, 0f, 0.5f);
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { vector, vector2, vector3, vector4 };
        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        mesh.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
        mesh.normals = new Vector3[mesh.vertices.Length];
        mesh.RecalculateNormals();
        mesh.name = "VBPlane";
        component.sharedMesh = mesh;
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        renderer.sharedMaterial = (Material) AssetDatabase.LoadAssetAtPath("Assets/Editor/QCAR/VirtualButtonTextures/VirtualButtonPreviewMaterial.mat", typeof(Material));
        EditorUtility.UnloadUnusedAssets();
    }

    private static void DetectDuplicates(ImageTargetAbstractBehaviour it)
    {
        IEditorImageTargetBehaviour behaviour = it;
        VirtualButtonAbstractBehaviour[] componentsInChildren = it.GetComponentsInChildren<VirtualButtonAbstractBehaviour>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            for (int j = i + 1; j < componentsInChildren.Length; j++)
            {
                if (componentsInChildren[i].VirtualButtonName == componentsInChildren[j].VirtualButtonName)
                {
                    Debug.LogError("Duplicate virtual buttons with name '" + componentsInChildren[i].VirtualButtonName + "' detected in Image Target '" + behaviour.TrackableName + "'.");
                }
            }
        }
    }

    private static bool IsVBMeshCreated(VirtualButtonAbstractBehaviour vb)
    {
        GameObject gameObject = vb.gameObject;
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        return (((component != null) && (renderer != null)) && (component.sharedMesh != null));
    }

    public void OnEnable()
    {
        if (QCARUtilities.GetPrefabType(base.target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            VirtualButtonAbstractBehaviour target = (VirtualButtonAbstractBehaviour) base.target;
            if (!EditorApplication.isPlaying)
            {
                if (!target.HasUpdatedPose)
                {
                    target.UpdatePose();
                }
                if (!IsVBMeshCreated(target))
                {
                    CreateVBMesh(target);
                }
                Validate();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        VirtualButtonAbstractBehaviour target = (VirtualButtonAbstractBehaviour) base.target;
        IEditorVirtualButtonBehaviour behaviour2 = target;
        behaviour2.SetVirtualButtonName(EditorGUILayout.TextField("Name", behaviour2.VirtualButtonName, new GUILayoutOption[0]));
        behaviour2.SetSensitivitySetting((VirtualButton.Sensitivity) EditorGUILayout.EnumPopup("Sensitivity Setting", behaviour2.SensitivitySetting, new GUILayoutOption[0]));
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    public void OnSceneGUI()
    {
        VirtualButtonAbstractBehaviour target = (VirtualButtonAbstractBehaviour) base.target;
        target.transform.localScale = new Vector3(target.transform.localScale[0], 1f, target.transform.localScale[2]);
    }

    public static void Validate()
    {
        ImageTargetAbstractBehaviour[] behaviourArray = (ImageTargetAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(ImageTargetAbstractBehaviour));
        foreach (ImageTargetAbstractBehaviour behaviour in behaviourArray)
        {
            DetectDuplicates(behaviour);
        }
        VirtualButtonAbstractBehaviour[] behaviourArray2 = (VirtualButtonAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(VirtualButtonAbstractBehaviour));
        foreach (VirtualButtonAbstractBehaviour behaviour2 in behaviourArray2)
        {
            IEditorImageTargetBehaviour imageTargetBehaviour = behaviour2.GetImageTargetBehaviour();
            if (imageTargetBehaviour == null)
            {
                Debug.LogError("Virtual Button '" + behaviour2.name + "' doesn't have an Image Target as an ancestor.");
            }
            else if (imageTargetBehaviour.ImageTargetType == ImageTargetType.USER_DEFINED)
            {
                Debug.LogError("Virtual Button '" + behaviour2.name + "' cannot be added to a user defined target.");
            }
        }
    }
}

