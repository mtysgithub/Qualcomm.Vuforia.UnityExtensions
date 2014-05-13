using System;
using UnityEngine;

internal class TextureRenderer
{
    private Camera mTextureBufferCamera;
    private int mTextureHeight;
    private int mTextureWidth;

    public TextureRenderer(Texture textureToRender, int renderTextureLayer, QCARRenderer.Vec2I requestedTextureSize)
    {
        if (renderTextureLayer > 0x1f)
        {
            Debug.LogError("WebCamBehaviour.SetupTextureBufferCamera: configured layer > 31 is not supported by Unity!");
        }
        else
        {
            this.mTextureWidth = requestedTextureSize.x;
            this.mTextureHeight = requestedTextureSize.y;
            float y = (((float) this.mTextureHeight) / ((float) this.mTextureWidth)) * 0.5f;
            GameObject target = new GameObject("TextureBufferCamera");
            this.mTextureBufferCamera = target.AddComponent<Camera>();
            this.mTextureBufferCamera.isOrthoGraphic = true;
            this.mTextureBufferCamera.orthographicSize = y;
            this.mTextureBufferCamera.aspect = ((float) this.mTextureWidth) / ((float) this.mTextureHeight);
            this.mTextureBufferCamera.nearClipPlane = 0.5f;
            this.mTextureBufferCamera.farClipPlane = 1.5f;
            this.mTextureBufferCamera.cullingMask = ((int) 1) << renderTextureLayer;
            this.mTextureBufferCamera.enabled = false;
            if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepARCameraAlive)
            {
                UnityEngine.Object.DontDestroyOnLoad(target);
            }
            GameObject obj3 = new GameObject("TextureBufferMesh", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) }) {
                transform = { parent = target.transform },
                layer = renderTextureLayer
            };
            Mesh mesh2 = new Mesh();
            mesh2.vertices = new Vector3[] { new Vector3(-0.5f, y, 1f), new Vector3(0.5f, y, 1f), new Vector3(-0.5f, -y, 1f), new Vector3(0.5f, -y, 1f) };
            mesh2.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
            mesh2.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            Mesh mesh = mesh2;
            MeshRenderer component = obj3.GetComponent<MeshRenderer>();
            component.material = new Material(Shader.Find("Unlit/Texture"));
            component.material.mainTexture = textureToRender;
            obj3.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    public void Destroy()
    {
        if (this.mTextureBufferCamera != null)
        {
            UnityEngine.Object.Destroy(this.mTextureBufferCamera.gameObject);
        }
    }

    public RenderTexture Render()
    {
        RenderTexture temporary = RenderTexture.GetTemporary(this.mTextureWidth, this.mTextureHeight);
        this.mTextureBufferCamera.targetTexture = temporary;
        this.mTextureBufferCamera.Render();
        return temporary;
    }

    public int Height
    {
        get
        {
            return this.mTextureHeight;
        }
    }

    public int Width
    {
        get
        {
            return this.mTextureWidth;
        }
    }
}

