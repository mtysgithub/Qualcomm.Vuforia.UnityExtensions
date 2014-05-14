using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MarkerAbstractBehaviour), true)]
public class MarkerEditor : Editor
{
    private static void CheckMesh(MarkerAbstractBehaviour mb)
    {
        MeshFilter component = mb.gameObject.GetComponent<MeshFilter>();
        if ((component == null) || (component.sharedMesh == null))
        {
            CreateMesh(mb);
        }
    }

    public static void CreateMesh(MarkerAbstractBehaviour marker)
    {
        GameObject gameObject = marker.gameObject;
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
        mesh.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };
        mesh.normals = new Vector3[mesh.vertices.Length];
        mesh.RecalculateNormals();
        component.mesh = mesh;
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        }
        EditorUtility.UnloadUnusedAssets();
    }

    public void OnEnable()
    {
        MarkerAbstractBehaviour target = (MarkerAbstractBehaviour) base.target;
        if (QCARUtilities.GetPrefabType(target) != PrefabType.Prefab)
        {
            if (!SceneManager.Instance.SceneInitialized)
            {
                SceneManager.Instance.InitScene();
            }
            IEditorMarkerBehaviour behaviour2 = target;
            if (!behaviour2.InitializedInEditor && !EditorApplication.isPlaying)
            {
                behaviour2.SetMarkerID(SceneManager.Instance.GetNextFrameMarkerID());
                CreateMesh(target);
                behaviour2.SetNameForTrackable("FrameMarker" + behaviour2.MarkerID);
                target.name = "FrameMarker" + behaviour2.MarkerID;
                behaviour2.SetInitializedInEditor(true);
                EditorUtility.SetDirty(target);
                SceneManager.Instance.SceneUpdated();
            }
            else if (!EditorApplication.isPlaying)
            {
                CheckMesh(target);
            }
            behaviour2.SetPreviousScale(target.transform.localScale);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.DrawDefaultInspector();
        MarkerAbstractBehaviour target = (MarkerAbstractBehaviour) base.target;
        IEditorMarkerBehaviour behaviour2 = target;
        if (QCARUtilities.GetPrefabType(target) == PrefabType.Prefab)
        {
            behaviour2.SetMarkerID(-1);
            EditorGUILayout.IntField("Marker ID", behaviour2.MarkerID, new GUILayoutOption[0]);
            EditorGUILayout.Toggle("Preserve child size", behaviour2.PreserveChildSize, new GUILayoutOption[0]);
        }
        else
        {
            int markerID = EditorGUILayout.IntField("Marker ID", behaviour2.MarkerID, new GUILayoutOption[0]);
            if (markerID < 0)
            {
                markerID = 0;
            }
            else if (markerID >= 0x200)
            {
                markerID = 0x1ff;
            }
            if (markerID != behaviour2.MarkerID)
            {
                behaviour2.SetMarkerID(markerID);
                behaviour2.SetNameForTrackable("FrameMarker" + markerID);
            }
            behaviour2.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", behaviour2.PreserveChildSize, new GUILayoutOption[0]));
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            SceneManager.Instance.SceneUpdated();
        }
    }

    public static void UpdateScale(IEditorMarkerBehaviour marker, Vector2 size)
    {
        float num = marker.transform.localScale.x / size[0];
        marker.transform.localScale = new Vector3(size[0], size[0], size[0]);
        if (marker.PreserveChildSize)
        {
            foreach (Transform transform in marker.transform)
            {
                transform.localPosition = new Vector3(transform.localPosition.x * num, transform.localPosition.y * num, transform.localPosition.z * num);
                transform.localScale = new Vector3(transform.localScale.x * num, transform.localScale.y * num, transform.localScale.z * num);
            }
        }
    }
}

