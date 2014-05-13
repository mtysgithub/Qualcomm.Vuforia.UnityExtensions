using System;
using UnityEngine;

public abstract class BGRenderingAbstractBehaviour : MonoBehaviour
{
    public UnityEngine.Camera Camera;
    private bool mFlipHorizontally;
    private int mScreenHeight;
    private ScreenOrientation mScreenOrientation;
    private int mScreenWidth;
    private QCARRenderer.VideoTextureInfo mTextureInfo;

    protected BGRenderingAbstractBehaviour()
    {
    }

    public void CheckAndSetActive(bool isActive)
    {
        if (this.Camera.gameObject.activeSelf != isActive)
        {
            this.Camera.gameObject.SetActive(isActive);
        }
    }

    private Mesh CreateVideoMesh()
    {
        Mesh mesh = new Mesh {
            vertices = new Vector3[4]
        };
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                float num3 = (((float) k) / 1f) - 0.5f;
                float num4 = (1f - (((float) i) / 1f)) - 0.5f;
                vertices[(i * 2) + k].x = num3 * 2f;
                vertices[(i * 2) + k].y = 0f;
                vertices[(i * 2) + k].z = num4 * 2f;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = new int[0x18];
        int num5 = 0;
        float num6 = ((float) this.mTextureInfo.imageSize.x) / ((float) this.mTextureInfo.textureSize.x);
        float num7 = ((float) this.mTextureInfo.imageSize.y) / ((float) this.mTextureInfo.textureSize.y);
        mesh.uv = new Vector2[4];
        int[] triangles = mesh.triangles;
        Vector2[] uv = mesh.uv;
        for (int j = 0; j < 1; j++)
        {
            for (int m = 0; m < 1; m++)
            {
                int index = (j * 2) + m;
                int num11 = (((j * 2) + m) + 2) + 1;
                int num12 = ((j * 2) + m) + 2;
                int num13 = ((j * 2) + m) + 1;
                triangles[num5++] = index;
                triangles[num5++] = num11;
                triangles[num5++] = num12;
                triangles[num5++] = num11;
                triangles[num5++] = index;
                triangles[num5++] = num13;
                uv[index] = new Vector2((((float) m) / 1f) * num6, (((float) j) / 1f) * num7);
                uv[num11] = new Vector2((((float) (m + 1)) / 1f) * num6, (((float) (j + 1)) / 1f) * num7);
                uv[num12] = new Vector2((((float) m) / 1f) * num6, (((float) (j + 1)) / 1f) * num7);
                uv[num13] = new Vector2((((float) (m + 1)) / 1f) * num6, (((float) j) / 1f) * num7);
                if (this.mFlipHorizontally)
                {
                    uv[index].x = 1f - uv[index].x;
                    uv[num11].x = 1f - uv[num11].x;
                    uv[num12].x = 1f - uv[num12].x;
                    uv[num13].x = 1f - uv[num13].x;
                }
            }
        }
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = new Vector3[mesh.vertices.Length];
        mesh.RecalculateNormals();
        return mesh;
    }

    private void PositionVideoMesh()
    {
        float num;
        this.mScreenOrientation = QCARRuntimeUtilities.ScreenOrientation;
        this.mScreenWidth = Screen.width;
        this.mScreenHeight = Screen.height;
        base.gameObject.transform.localRotation = Quaternion.AngleAxis(270f, Vector3.right);
        if (this.mScreenOrientation == ScreenOrientation.LandscapeLeft)
        {
            Transform transform = base.gameObject.transform;
            transform.localRotation *= Quaternion.identity;
        }
        else if (this.mScreenOrientation == ScreenOrientation.Portrait)
        {
            Transform transform2 = base.gameObject.transform;
            transform2.localRotation *= Quaternion.AngleAxis(90f, Vector3.up);
        }
        else if (this.mScreenOrientation == ScreenOrientation.LandscapeRight)
        {
            Transform transform3 = base.gameObject.transform;
            transform3.localRotation *= Quaternion.AngleAxis(180f, Vector3.up);
        }
        else if (this.mScreenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            Transform transform4 = base.gameObject.transform;
            transform4.localRotation *= Quaternion.AngleAxis(270f, Vector3.up);
        }
        base.gameObject.transform.localScale = new Vector3(1f, 1f, (1f * this.mTextureInfo.imageSize.y) / ((float) this.mTextureInfo.imageSize.x));
        this.Camera.orthographic = true;
        if (this.ShouldFitWidth())
        {
            if (QCARRuntimeUtilities.IsPortraitOrientation)
            {
                num = (((float) this.mTextureInfo.imageSize.y) / ((float) this.mTextureInfo.imageSize.x)) * (((float) this.mScreenHeight) / ((float) this.mScreenWidth));
            }
            else
            {
                num = ((float) this.mScreenHeight) / ((float) this.mScreenWidth);
            }
        }
        else if (QCARRuntimeUtilities.IsPortraitOrientation)
        {
            num = 1f;
        }
        else
        {
            num = ((float) this.mTextureInfo.imageSize.y) / ((float) this.mTextureInfo.imageSize.x);
        }
        this.Camera.orthographicSize = num;
    }

    public void SetFlipHorizontally(bool flip)
    {
        this.mFlipHorizontally = flip;
    }

    public void SetTexture(Texture texture)
    {
        base.renderer.material.mainTexture = texture;
    }

    private bool ShouldFitWidth()
    {
        float num2;
        float num = ((float) this.mScreenWidth) / ((float) this.mScreenHeight);
        if (QCARRuntimeUtilities.IsPortraitOrientation)
        {
            num2 = ((float) this.mTextureInfo.imageSize.y) / ((float) this.mTextureInfo.imageSize.x);
        }
        else
        {
            num2 = ((float) this.mTextureInfo.imageSize.x) / ((float) this.mTextureInfo.imageSize.y);
        }
        return (num >= num2);
    }

    private void Start()
    {
        if (this.Camera == null)
        {
            this.Camera = UnityEngine.Camera.main;
        }
    }

    private void Update()
    {
        if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable())
        {
            QCARRenderer.VideoTextureInfo videoTextureInfo = QCARRenderer.Instance.GetVideoTextureInfo();
            if (!this.mTextureInfo.imageSize.Equals(videoTextureInfo.imageSize) || !this.mTextureInfo.textureSize.Equals(videoTextureInfo.textureSize))
            {
                this.mTextureInfo = videoTextureInfo;
                Debug.Log(string.Concat(new object[] { "VideoTextureInfo ", videoTextureInfo.textureSize.x, " ", videoTextureInfo.textureSize.y, " ", videoTextureInfo.imageSize.x, " ", videoTextureInfo.imageSize.y }));
                MeshFilter component = base.GetComponent<MeshFilter>();
                if (component == null)
                {
                    component = base.gameObject.AddComponent<MeshFilter>();
                }
                component.mesh = this.CreateVideoMesh();
                this.PositionVideoMesh();
            }
            else if (((this.mScreenOrientation != QCARRuntimeUtilities.ScreenOrientation) || (this.mScreenWidth != Screen.width)) || (this.mScreenHeight != Screen.height))
            {
                this.PositionVideoMesh();
            }
        }
    }
}

