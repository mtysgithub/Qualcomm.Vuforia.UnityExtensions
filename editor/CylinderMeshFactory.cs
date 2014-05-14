using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class CylinderMeshFactory
{
    private readonly float mBigRadius;
    private readonly float mBottomRadius;
    private readonly bool mFlip;
    private List<Vector3> mNormals;
    private List<Vector3> mPositions;
    private readonly float mSideLength;
    private readonly float mSidelengthBig;
    private readonly float mSidelengthSmall;
    private readonly float mSinTheta;
    private readonly float mSmallRadius;
    private readonly float mTopRadius;
    private readonly float mUMax;
    private List<Vector2> mUVs;
    private readonly float mVSmall;

    private CylinderMeshFactory(float sideLength, float topDiameter, float bottomDiameter)
    {
        this.mSideLength = sideLength;
        this.mTopRadius = topDiameter * 0.5f;
        this.mBottomRadius = bottomDiameter * 0.5f;
        this.mFlip = this.mTopRadius >= this.mBottomRadius;
        if (this.mFlip)
        {
            this.mBigRadius = this.mTopRadius;
            this.mSmallRadius = this.mBottomRadius;
        }
        else
        {
            this.mBigRadius = this.mBottomRadius;
            this.mSmallRadius = this.mTopRadius;
        }
        if ((this.mBigRadius - this.mSmallRadius) >= sideLength)
        {
            this.mSinTheta = 1f;
        }
        else
        {
            this.mSinTheta = (this.mBigRadius - this.mSmallRadius) / sideLength;
        }
        this.mSidelengthSmall = this.IsCylinder() ? 0f : ((sideLength * this.mSmallRadius) / (this.mBigRadius - this.mSmallRadius));
        this.mSidelengthBig = this.mSidelengthSmall + sideLength;
        float f = 3.141593f * this.mSinTheta;
        bool flag = f < 1.5707963267948966;
        this.mUMax = this.IsCylinder() ? (3.141593f * this.mBigRadius) : (flag ? (this.mSidelengthBig * Mathf.Sin(f)) : this.mSidelengthBig);
        this.mVSmall = Mathf.Cos(f) * (flag ? this.mSidelengthSmall : this.mSidelengthBig);
    }

    private List<int> AddBodyTriangles(List<Vector3> bottomPerimeterVertices, List<Vector3> topPerimeterVertices)
    {
        int count = bottomPerimeterVertices.Count;
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            Vector3 vector = bottomPerimeterVertices[i];
            Vector3 vector2 = topPerimeterVertices[i];
            Vector3 rhs = new Vector3(vector.x + vector2.x, 0f, vector.z + vector2.z);
            rhs.Normalize();
            Vector3 axis = (Vector3) (Vector3.Cross(Vector3.up, rhs) * -1f);
            Vector3 to = vector2 - vector;
            Vector3 item = (Vector3) (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, to), axis) * rhs);
            list.Add(item);
        }
        float angleInRadians = -3.141593f;
        float num5 = 6.283185f / ((float) count);
        for (int j = 0; j <= count; j++)
        {
            int num7 = j % count;
            this.mPositions.Add(bottomPerimeterVertices[num7]);
            this.mNormals.Add(list[num7]);
            this.mUVs.Add(this.ConvertToUVCoordinates(angleInRadians, 0f));
            angleInRadians += num5;
        }
        angleInRadians = -3.141593f;
        for (int k = 0; k <= count; k++)
        {
            int num9 = k % count;
            this.mPositions.Add(topPerimeterVertices[num9]);
            this.mNormals.Add(list[num9]);
            this.mUVs.Add(this.ConvertToUVCoordinates(angleInRadians, this.mSideLength));
            angleInRadians += num5;
        }
        int num10 = this.mPositions.Count;
        for (int m = 0; m < this.mPositions.Count; m++)
        {
            this.mNormals.Add(-this.mNormals[m]);
        }
        this.mPositions.AddRange(this.mPositions);
        this.mUVs.AddRange(this.mUVs);
        List<int> list2 = new List<int>();
        for (int n = 1; n <= count; n++)
        {
            list2.AddRange(new int[] { n - 1, n, n + count });
            list2.AddRange(new int[] { (n + count) + 1, n + count, n });
        }
        for (int num13 = 1; num13 <= count; num13++)
        {
            list2.AddRange(new int[] { (num10 + num13) - 1, (num10 + num13) + count, num10 + num13 });
            list2.AddRange(new int[] { ((num10 + num13) + count) + 1, num10 + num13, (num10 + num13) + count });
        }
        return list2;
    }

    private List<int> AddSealingTriangles(List<Vector3> perimeterVertices, bool isTop)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 2; i++)
        {
            bool flag = i == 0;
            Vector3 item = flag ? Vector3.up : Vector3.down;
            int count = this.mPositions.Count;
            this.mPositions.Add(new Vector3(0f, perimeterVertices[0].y, 0f));
            int num3 = this.mPositions.Count;
            int num4 = (num3 + perimeterVertices.Count) - 1;
            this.mPositions.AddRange(perimeterVertices);
            for (int j = 0; j <= perimeterVertices.Count; j++)
            {
                this.mNormals.Add(item);
            }
            this.mUVs.AddRange(CreatePerimeterUVCoordinates(perimeterVertices.Count, isTop));
            for (int k = num3; k < num4; k++)
            {
                list.AddRange(flag ? ((IEnumerable<int>) new int[] { count, k, (k + 1) }) : ((IEnumerable<int>) new int[] { count, (k + 1), k }));
            }
            list.AddRange(flag ? ((IEnumerable<int>) new int[] { count, num4, num3 }) : ((IEnumerable<int>) new int[] { count, num3, num4 }));
        }
        return list;
    }

    private float ComputeHeight(float sideLength)
    {
        return Mathf.Sqrt((sideLength * sideLength) - ((this.mBigRadius - this.mSmallRadius) * (this.mBigRadius - this.mSmallRadius)));
    }

    private Vector2 ConvertToUVCoordinates(float angleInRadians, float slantedYPos)
    {
        float num;
        float num2;
        if (this.mFlip)
        {
            angleInRadians = -angleInRadians;
            slantedYPos = this.mSidelengthSmall + slantedYPos;
        }
        else
        {
            slantedYPos = this.mSidelengthBig - slantedYPos;
        }
        if (this.IsCylinder())
        {
            num = angleInRadians * this.mBigRadius;
            num2 = slantedYPos;
        }
        else
        {
            num = slantedYPos * Mathf.Sin(angleInRadians * this.mSinTheta);
            num2 = slantedYPos * Mathf.Cos(angleInRadians * this.mSinTheta);
        }
        num = (num / (2f * this.mUMax)) + 0.5f;
        num2 = (num2 - this.mVSmall) / (this.mSidelengthBig - this.mVSmall);
        if (this.mFlip)
        {
            num2 = 1f - num2;
            num = 1f - num;
        }
        return new Vector2(num, num2);
    }

    private Mesh CreateCylinderMesh(int numPerimeterVertices, bool hasTopGeometry, bool hasBottomGeometry, bool insideMaterial)
    {
        this.mPositions = new List<Vector3>();
        this.mNormals = new List<Vector3>();
        this.mUVs = new List<Vector2>();
        float height = this.ComputeHeight(this.mSideLength);
        List<Vector3> bottomPerimeterVertices = CreatePerimeterPositions(0f, this.mBottomRadius, numPerimeterVertices);
        List<Vector3> topPerimeterVertices = CreatePerimeterPositions(height, this.mTopRadius, numPerimeterVertices);
        List<int> source = this.AddBodyTriangles(bottomPerimeterVertices, topPerimeterVertices);
        int submesh = 1;
        List<int> list4 = null;
        if (hasBottomGeometry && (this.mBottomRadius > 0f))
        {
            list4 = this.AddSealingTriangles(bottomPerimeterVertices, false);
            submesh++;
        }
        List<int> list5 = null;
        if (hasTopGeometry && (this.mTopRadius > 0f))
        {
            list5 = this.AddSealingTriangles(topPerimeterVertices, true);
            submesh++;
        }
        Mesh mesh = new Mesh {
            vertices = this.mPositions.ToArray(),
            normals = this.mNormals.ToArray(),
            uv = this.mUVs.ToArray(),
            subMeshCount = insideMaterial ? (2 * submesh) : submesh
        };
        int[] numArray = null;
        int[] triangles = null;
        int[] numArray3 = null;
        if (insideMaterial)
        {
            numArray = source.Skip<int>((source.Count / 2)).ToArray<int>();
            source = source.Take<int>((source.Count / 2)).ToList<int>();
            if (list4 != null)
            {
                numArray3 = list4.Take<int>((list4.Count / 2)).ToArray<int>();
                list4 = list4.Skip<int>((list4.Count / 2)).ToList<int>();
            }
            if (list5 != null)
            {
                triangles = list5.Skip<int>((list5.Count / 2)).ToArray<int>();
                list5 = list5.Take<int>((list5.Count / 2)).ToList<int>();
            }
        }
        mesh.SetTriangles(source.ToArray(), 0);
        if (list4 != null)
        {
            mesh.SetTriangles(list4.ToArray(), 1);
        }
        if (list5 != null)
        {
            mesh.SetTriangles(list5.ToArray(), submesh - 1);
        }
        if (insideMaterial)
        {
            mesh.SetTriangles(numArray.ToArray<int>(), submesh);
            if (numArray3 != null)
            {
                mesh.SetTriangles(numArray3, submesh + 1);
            }
            if (triangles != null)
            {
                mesh.SetTriangles(triangles, (submesh + submesh) - 1);
            }
        }
        return mesh;
    }

    public static Mesh CreateCylinderMesh(float sideLength, float topDiameter, float bottomDiameter, int numPerimeterVertices, bool hasTopGeometry, bool hasBottomGeometry, [Optional, DefaultParameterValue(false)] bool insideMaterial)
    {
        CylinderMeshFactory factory = new CylinderMeshFactory(sideLength, topDiameter, bottomDiameter);
        return factory.CreateCylinderMesh(numPerimeterVertices, hasTopGeometry, hasBottomGeometry, insideMaterial);
    }

    private static List<Vector3> CreatePerimeterPositions(float height, float radius, int numPerimeterVertices)
    {
        List<Vector3> list = new List<Vector3>();
        float f = -1.570796f;
        float num2 = 6.283185f / ((float) numPerimeterVertices);
        for (int i = 0; i < numPerimeterVertices; i++)
        {
            list.Add(new Vector3(radius * Mathf.Sin(f), height, radius * Mathf.Cos(f)));
            f += num2;
        }
        return list;
    }

    private static List<Vector2> CreatePerimeterUVCoordinates(int numPerimeterVertices, bool isTop)
    {
        List<Vector2> list = new List<Vector2> {
            new Vector2(0.5f, 0.5f)
        };
        float f = -1.570796f;
        float num2 = 6.283185f / ((float) numPerimeterVertices);
        for (int i = 0; i < numPerimeterVertices; i++)
        {
            float x = (Mathf.Cos(f) * 0.5f) + 0.5f;
            float y = (Mathf.Sin(f) * 0.5f) + 0.5f;
            x = 1f - x;
            if (!isTop)
            {
                y = 1f - y;
            }
            list.Add(new Vector2(x, y));
            f += num2;
        }
        return list;
    }

    private bool IsCylinder()
    {
        return (Math.Abs((float) (this.mBigRadius - this.mSmallRadius)) < 1E-05);
    }
}

