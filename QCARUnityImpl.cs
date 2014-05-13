using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public static class QCARUnityImpl
{
    public static QCARUnity.InitError CheckInitializationError()
    {
        return (QCARUnity.InitError) QCARWrapper.Instance.GetInitErrorCode();
    }

    public static Matrix4x4 GetProjectionGL(float nearPlane, float farPlane, ScreenOrientation screenOrientation)
    {
        float[] destination = new float[0x10];
        IntPtr projMatrix = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(float)) * destination.Length));
        QCARWrapper.Instance.GetProjectionGL(nearPlane, farPlane, projMatrix, (int) screenOrientation);
        Marshal.Copy(projMatrix, destination, 0, destination.Length);
        Matrix4x4 identity = Matrix4x4.identity;
        for (int i = 0; i < 0x10; i++)
        {
            identity[i] = destination[i];
        }
        Marshal.FreeHGlobal(projMatrix);
        return identity;
    }

    public static bool IsRendererDirty()
    {
        CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            return instance.IsDirty();
        }
        if (QCARWrapper.Instance.IsRendererDirty() != 1)
        {
            return instance.IsDirty();
        }
        return true;
    }

    public static bool RequiresAlpha()
    {
        return (QCARWrapper.Instance.QcarRequiresAlpha() == 1);
    }

    public static bool SetHint(QCARUnity.QCARHint hint, int value)
    {
        Debug.Log("SetHint");
        return (QCARWrapper.Instance.QcarSetHint((int) hint, value) == 1);
    }

    public static void SetUnityVersion(string path, [Optional, DefaultParameterValue(false)] bool setNative)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        string pattern = "[^0-9]";
        string[] strArray = Regex.Split(Application.unityVersion, pattern);
        if (strArray.Length >= 3)
        {
            num = int.Parse(strArray[0]);
            num2 = int.Parse(strArray[1]);
            num3 = int.Parse(strArray[2]);
        }
        try
        {
            File.WriteAllText(Path.Combine(path, "unity.txt"), string.Format("{0}.{1}.{2}", num, num2, num3));
        }
        catch (Exception exception)
        {
            Debug.LogError("Writing Unity version to file failed: " + exception.Message);
        }
        if (setNative)
        {
            QCARWrapper.Instance.SetUnityVersion(num, num2, num3);
        }
    }
}

