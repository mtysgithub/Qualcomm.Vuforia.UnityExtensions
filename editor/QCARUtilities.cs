using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class QCARUtilities
{
    public static ConfigData.ImageTargetData CreateDefaultImageTarget()
    {
        return new ConfigData.ImageTargetData { size = new Vector2(200f, 200f), virtualButtons = new List<ConfigData.VirtualButtonData>() };
    }

    public static int GetIndexFromString(string stringToFind, string[] availableStrings)
    {
        int num = -1;
        for (int i = 0; i < availableStrings.Length; i++)
        {
            if (string.Compare(availableStrings[i], stringToFind, true) == 0)
            {
                num = i;
            }
        }
        return num;
    }

    public static PrefabType GetPrefabType(UnityEngine.Object target)
    {
        return EditorUtility.GetPrefabType(target);
    }

    public static bool OrientationFromStringArray(out Quaternion result, string[] valuesToParse)
    {
        result = Quaternion.identity;
        bool flag = false;
        if (valuesToParse != null)
        {
            if (valuesToParse.Length == 5)
            {
                flag = true;
            }
            else if (valuesToParse.Length == 4)
            {
                Debug.LogError("Direct parsing of Quaternions is not supported. Use Axis-Angle Degrees (AD:) or Axis-Angle Radians (AR:) instead.");
            }
        }
        if (!flag)
        {
            return false;
        }
        try
        {
            float angle = float.Parse(valuesToParse[4]);
            Vector3 axis = new Vector3(-float.Parse(valuesToParse[1]), float.Parse(valuesToParse[3]), -float.Parse(valuesToParse[2]));
            if (string.Compare(valuesToParse[0], "ad:", true) == 0)
            {
                result = Quaternion.AngleAxis(angle, axis);
            }
            else if (string.Compare(valuesToParse[0], "ar:", true) == 0)
            {
                result = Quaternion.AngleAxis(57.29578f * angle, axis);
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static bool RectangleFromStringArray(out Vector4 result, string[] valuesToParse)
    {
        result = Vector4.zero;
        bool flag = false;
        if ((valuesToParse != null) && (valuesToParse.Length == 4))
        {
            flag = true;
        }
        if (!flag)
        {
            return false;
        }
        try
        {
            result = new Vector4(float.Parse(valuesToParse[0]), float.Parse(valuesToParse[1]), float.Parse(valuesToParse[2]), float.Parse(valuesToParse[3]));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static bool SizeFromStringArray(out Vector2 result, string[] valuesToParse)
    {
        result = Vector2.zero;
        bool flag = false;
        if ((valuesToParse != null) && (valuesToParse.Length == 2))
        {
            flag = true;
        }
        if (!flag)
        {
            return false;
        }
        try
        {
            result = new Vector2(float.Parse(valuesToParse[0]), float.Parse(valuesToParse[1]));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static bool TransformFromStringArray(out Vector3 result, string[] valuesToParse)
    {
        result = Vector3.zero;
        bool flag = false;
        if ((valuesToParse != null) && (valuesToParse.Length == 3))
        {
            flag = true;
        }
        if (!flag)
        {
            return false;
        }
        try
        {
            result = new Vector3(float.Parse(valuesToParse[0]), float.Parse(valuesToParse[2]), float.Parse(valuesToParse[1]));
        }
        catch
        {
            return false;
        }
        return true;
    }

    [StructLayout(LayoutKind.Sequential, Size=1)]
    public struct GlobalVars
    {
        public const string DATA_SET_PATH = "Assets/StreamingAssets/QCAR/";
        public const string DAT_PATH = "Assets/StreamingAssets/QCAR/qcar-resources.dat";
        public const string CONFIG_XML_PATH = "Assets/StreamingAssets/QCAR/config.xml";
        public const string TARGET_TEXTURES_PATH = "Assets/Editor/QCAR/ImageTargetTextures/";
        public const string CYLINDER_TARGET_TEXTURES_PATH = "Assets/Editor/QCAR/CylinderTargetTextures/";
        public const string REFERENCE_MATERIAL_PATH = "Assets/Qualcomm Augmented Reality/Materials/DefaultTarget.mat";
        public const string MASK_MATERIAL_PATH = "Assets/Qualcomm Augmented Reality/Materials/DepthMask.mat";
        public const string VIRTUAL_BUTTON_MATERIAL_PATH = "Assets/Editor/QCAR/VirtualButtonTextures/VirtualButtonPreviewMaterial.mat";
        public const string UDT_MATERIAL_PATH = "Assets/Qualcomm Augmented Reality/Materials/UserDefinedTarget.mat";
        public const string CL_MATERIAL_PATH = "Assets/Qualcomm Augmented Reality/Materials/CloudRecoTarget.mat";
        public const string FONT_PATH = "Assets/Qualcomm Augmented Reality/Fonts/";
        public const string PREFABS_PATH = "Assets/Qualcomm Augmented Reality/Prefabs/";
        public const string DEFAULT_TRACKABLE_NAME = "--- EMPTY ---";
        public const string DEFAULT_DATA_SET_NAME = "--- EMPTY ---";
        public const string PREDEFINED_TARGET_DROPDOWN_TEXT = "Predefined";
        public const string USER_CREATED_TARGET_DROPDOWN_TEXT = "User Defined";
        public const string CLOUD_RECO_DROPDOWN_TEXT = "Cloud Reco";
        public const int MAX_NUM_FRAME_MARKERS = 0x200;
    }
}

