using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WordAbstractBehaviour), true)]
public class WordEditor : Editor
{
    public static bool CheckForDuplicates()
    {
        bool flag = false;
        WordAbstractBehaviour[] behaviourArray = (WordAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(WordAbstractBehaviour));
        for (int i = 0; i < behaviourArray.Length; i++)
        {
            IEditorWordBehaviour behaviour = behaviourArray[i];
            for (int j = i + 1; j < behaviourArray.Length; j++)
            {
                IEditorWordBehaviour behaviour2 = behaviourArray[j];
                if (behaviour.IsTemplateMode && behaviour2.IsTemplateMode)
                {
                    Debug.LogWarning("Duplicate template word target found. Only one of the Trackables and its respective Augmentation will be selected for use at runtime - that selection is indeterminate her.");
                    flag = true;
                }
                else if ((!behaviour.IsTemplateMode && !behaviour2.IsTemplateMode) && (behaviour.SpecificWord == behaviour2.SpecificWord))
                {
                    Debug.LogWarning("Duplicate word target \"" + behaviour.SpecificWord + "\"found. Only one of the Trackables and its respective Augmentation will be selected for use at runtime - that selection is indeterminate her.");
                    flag = true;
                }
            }
        }
        return flag;
    }

    private static Bounds GetBoundsForAxisAlignedTextMesh(TextMesh textMesh)
    {
        textMesh.anchor = TextAnchor.UpperRight;
        Bounds bounds = textMesh.renderer.bounds;
        textMesh.anchor = TextAnchor.LowerLeft;
        Bounds bounds2 = textMesh.renderer.bounds;
        Vector3 size = bounds2.min - bounds.min;
        return new Bounds(new Vector3(bounds2.min.x, bounds2.min.z, 0f) + ((Vector3) (0.5f * size)), size);
    }

    public void OnEnable()
    {
        WordAbstractBehaviour target = (WordAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            IEditorWordBehaviour behaviour2 = target;
            if (!behaviour2.InitializedInEditor && !EditorApplication.isPlaying)
            {
                behaviour2.SetMode(WordTemplateMode.Template);
                behaviour2.SetSpecificWord("Word");
                UpdateMesh(target);
                behaviour2.SetInitializedInEditor(true);
                EditorUtility.SetDirty(target);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        WordAbstractBehaviour target = (WordAbstractBehaviour) base.target;
        IEditorWordBehaviour behaviour2 = target;
        if (QCARUtilities.GetPrefabType(target) == PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.", new GUILayoutOption[0]);
        }
        else if (EditorGUILayout.Toggle("Template", behaviour2.IsTemplateMode, new GUILayoutOption[0]))
        {
            behaviour2.SetMode(WordTemplateMode.Template);
        }
        else
        {
            behaviour2.SetMode(WordTemplateMode.SpecificWord);
            string word = EditorGUILayout.TextField("Specific Word", behaviour2.SpecificWord, new GUILayoutOption[0]);
            if (word != behaviour2.SpecificWord)
            {
                if (word.Length == 0)
                {
                    Debug.LogWarning("Empty string used as word: This trackable and its augmentation will never be selected at runtime.");
                }
                behaviour2.SetSpecificWord(word);
            }
        }
        if (GUI.changed)
        {
            UpdateMesh(target);
            EditorUtility.SetDirty(target);
            SceneManager.Instance.SceneUpdated();
        }
    }

    private static void UpdateMesh(WordAbstractBehaviour behaviour)
    {
        GameObject obj3;
        GameObject gameObject = behaviour.gameObject;
        IEditorWordBehaviour behaviour2 = behaviour;
        Transform transform = gameObject.transform.FindChild("Text");
        if (transform == null)
        {
            obj3 = new GameObject("Text") {
                transform = { parent = gameObject.transform }
            };
        }
        else
        {
            obj3 = transform.gameObject;
        }
        obj3.transform.localScale = Vector3.one;
        obj3.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
        obj3.transform.localPosition = Vector3.zero;
        TextMesh component = obj3.GetComponent<TextMesh>();
        if (component == null)
        {
            component = obj3.AddComponent<TextMesh>();
        }
        string str = "Assets/Qualcomm Augmented Reality/Fonts/";
        Font font = (Font) AssetDatabase.LoadAssetAtPath(str + "SourceSansPro.ttf", typeof(Font));
        if (font != null)
        {
            component.fontSize = 0;
            component.font = font;
        }
        else
        {
            Debug.LogWarning("Standard font for Word-prefabs were not found. You might not be able to use it during runtime.");
            component.font = Resources.Load("Arial", typeof(Font)) as Font;
            component.fontSize = 0x24;
        }
        MeshRenderer renderer = obj3.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = obj3.AddComponent<MeshRenderer>();
        }
        Material material = new Material(component.font.material) {
            color = Color.black,
            shader = Shader.Find("Custom/Text3D")
        };
        renderer.sharedMaterial = material;
        component.text = behaviour2.IsTemplateMode ? "\"AnyWord\"" : behaviour2.SpecificWord;
        Quaternion localRotation = gameObject.transform.localRotation;
        Vector3 localScale = gameObject.transform.localScale;
        Vector3 localPosition = gameObject.transform.localPosition;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localPosition = Vector3.zero;
        Bounds boundsForAxisAlignedTextMesh = GetBoundsForAxisAlignedTextMesh(component);
        UpdateRectangleMesh(gameObject, boundsForAxisAlignedTextMesh);
        UpdateRectangleMaterial(gameObject);
        gameObject.transform.localRotation = localRotation;
        gameObject.transform.localScale = localScale;
        gameObject.transform.localPosition = localPosition;
    }

    private static void UpdateRectangleMaterial(GameObject gameObject)
    {
        MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
        if (component == null)
        {
            component = gameObject.AddComponent<MeshRenderer>();
        }
        component.hideFlags = HideFlags.NotEditable;
        string assetPath = "Assets/Qualcomm Augmented Reality/Materials/DefaultTarget.mat";
        Material source = (Material) AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
        Material material2 = new Material(source) {
            name = "Text",
            shader = Shader.Find("Unlit/Texture")
        };
        material2.SetColor("_Color", Color.white);
        component.sharedMaterial = material2;
    }

    private static void UpdateRectangleMesh(GameObject gameObject, Bounds bounds)
    {
        MeshFilter component = gameObject.GetComponent<MeshFilter>();
        if (component == null)
        {
            component = gameObject.AddComponent<MeshFilter>();
        }
        component.hideFlags = HideFlags.NotEditable;
        Vector3[] vectorArray = new Vector3[] { new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, bounds.max.y, bounds.max.z) };
        Vector3[] vectorArray2 = new Vector3[vectorArray.Length];
        Vector2[] vectorArray3 = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };
        Mesh mesh = new Mesh {
            vertices = vectorArray,
            normals = vectorArray2,
            uv = vectorArray3,
            triangles = new int[] { 0, 1, 2, 2, 1, 3 }
        };
        mesh.RecalculateNormals();
        component.sharedMesh = mesh;
        EditorUtility.UnloadUnusedAssets();
    }
}

