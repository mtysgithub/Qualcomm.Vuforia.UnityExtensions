using System;
using UnityEngine;

public static class QCARWrapper
{
    private static IQCARWrapper sWrapper;

    public static void Create()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                sWrapper = new QCARNativeIosWrapper();
            }
            else
            {
                sWrapper = new QCARNativeWrapper();
            }
        }
        else
        {
            sWrapper = new QCARNullWrapper();
        }
    }

    public static void SetImplementation(IQCARWrapper implementation)
    {
        sWrapper = implementation;
    }

    public static IQCARWrapper Instance
    {
        get
        {
            if (sWrapper == null)
            {
                Create();
            }
            return sWrapper;
        }
    }
}

