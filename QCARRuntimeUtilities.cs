using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class QCARRuntimeUtilities
{
    private static UnityEngine.ScreenOrientation sScreenOrientation;
    private static WebCamUsed sWebCamUsed;

    public static void CacheSurfaceOrientation(UnityEngine.ScreenOrientation surfaceOrientation)
    {
        sScreenOrientation = surfaceOrientation;
    }

    public static Rect CalculateRectFromLandscapeLeftCorners(Vector2 topLeft, Vector2 bottomRight, bool isMirrored)
    {
        if (isMirrored)
        {
            switch (ScreenOrientation)
            {
                case UnityEngine.ScreenOrientation.Portrait:
                    return new Rect(bottomRight.x, bottomRight.y, topLeft.x - bottomRight.x, topLeft.y - bottomRight.y);

                case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                    return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);

                case UnityEngine.ScreenOrientation.LandscapeRight:
                    return new Rect(topLeft.x, bottomRight.y, bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
            }
            return new Rect(bottomRight.x, topLeft.y, topLeft.x - bottomRight.x, bottomRight.y - topLeft.y);
        }
        switch (ScreenOrientation)
        {
            case UnityEngine.ScreenOrientation.Portrait:
                return new Rect(bottomRight.x, topLeft.y, topLeft.x - bottomRight.x, bottomRight.y - topLeft.y);

            case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                return new Rect(topLeft.x, bottomRight.y, bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);

            case UnityEngine.ScreenOrientation.LandscapeRight:
                return new Rect(bottomRight.x, bottomRight.y, topLeft.x - bottomRight.x, topLeft.y - bottomRight.y);
        }
        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    public static OrientedBoundingBox CameraFrameToScreenSpaceCoordinates(OrientedBoundingBox cameraFrameObb, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        bool flag = false;
        float num = 0f;
        switch (ScreenOrientation)
        {
            case UnityEngine.ScreenOrientation.Portrait:
                num += 90f;
                flag = true;
                break;

            case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                num += 270f;
                flag = true;
                break;

            case UnityEngine.ScreenOrientation.LandscapeRight:
                num += 180f;
                break;
        }
        float num2 = bgTextureViewPortRect.width / (flag ? ((float) videoModeData.height) : ((float) videoModeData.width));
        float num3 = bgTextureViewPortRect.height / (flag ? ((float) videoModeData.width) : ((float) videoModeData.height));
        Vector2 center = CameraFrameToScreenSpaceCoordinates(cameraFrameObb.Center, bgTextureViewPortRect, isTextureMirrored, videoModeData);
        Vector2 halfExtents = new Vector2(cameraFrameObb.HalfExtents.x * num2, cameraFrameObb.HalfExtents.y * num3);
        float rotation = cameraFrameObb.Rotation;
        if (isTextureMirrored)
        {
            rotation = -rotation;
        }
        return new OrientedBoundingBox(center, halfExtents, ((rotation * 180f) / 3.141593f) + num);
    }

    public static Vector2 CameraFrameToScreenSpaceCoordinates(Vector2 cameraFrameCoordinate, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        float xMin = bgTextureViewPortRect.xMin;
        float yMin = bgTextureViewPortRect.yMin;
        float width = bgTextureViewPortRect.width;
        float height = bgTextureViewPortRect.height;
        bool isPortrait = false;
        float num5 = videoModeData.width;
        float num6 = videoModeData.height;
        float prefixX = 0f;
        float prefixY = 0f;
        float inversionMultiplierX = 0f;
        float inversionMultiplierY = 0f;
        PrepareCoordinateConversion(isTextureMirrored, ref prefixX, ref prefixY, ref inversionMultiplierX, ref inversionMultiplierY, ref isPortrait);
        float num11 = ((cameraFrameCoordinate.x / num5) - prefixX) / inversionMultiplierX;
        float num12 = ((cameraFrameCoordinate.y / num6) - prefixY) / inversionMultiplierY;
        if (isPortrait)
        {
            return new Vector2((width * num12) + xMin, (height * num11) + yMin);
        }
        return new Vector2((width * num11) + xMin, (height * num12) + yMin);
    }

    public static void DisableSleepMode()
    {
        Screen.sleepTimeout = -1;
    }

    public static void ForceDisableTrackables()
    {
        TrackableBehaviour[] behaviourArray = (TrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));
        if (behaviourArray != null)
        {
            for (int i = 0; i < behaviourArray.Length; i++)
            {
                behaviourArray[i].enabled = false;
            }
        }
    }

    public static bool IsPlayMode()
    {
        return Application.isEditor;
    }

    public static bool IsQCAREnabled()
    {
        if (!IsPlayMode())
        {
            return true;
        }
        if (sWebCamUsed == WebCamUsed.UNKNOWN)
        {
            WebCamAbstractBehaviour behaviour = (WebCamAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(WebCamAbstractBehaviour));
            sWebCamUsed = behaviour.IsWebCamUsed() ? WebCamUsed.TRUE : WebCamUsed.FALSE;
        }
        return (sWebCamUsed == WebCamUsed.TRUE);
    }

    private static void PrepareCoordinateConversion(bool isTextureMirrored, ref float prefixX, ref float prefixY, ref float inversionMultiplierX, ref float inversionMultiplierY, ref bool isPortrait)
    {
        switch (ScreenOrientation)
        {
            case UnityEngine.ScreenOrientation.Portrait:
                isPortrait = true;
                if (isTextureMirrored)
                {
                    prefixX = 1f;
                    prefixY = 1f;
                    inversionMultiplierX = -1f;
                    inversionMultiplierY = -1f;
                    return;
                }
                prefixX = 0f;
                prefixY = 1f;
                inversionMultiplierX = 1f;
                inversionMultiplierY = -1f;
                return;

            case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                isPortrait = true;
                if (isTextureMirrored)
                {
                    prefixX = 0f;
                    prefixY = 0f;
                    inversionMultiplierX = 1f;
                    inversionMultiplierY = 1f;
                    return;
                }
                prefixX = 1f;
                prefixY = 0f;
                inversionMultiplierX = -1f;
                inversionMultiplierY = 1f;
                return;

            case UnityEngine.ScreenOrientation.LandscapeRight:
                isPortrait = false;
                if (isTextureMirrored)
                {
                    prefixX = 0f;
                    prefixY = 1f;
                    inversionMultiplierX = 1f;
                    inversionMultiplierY = -1f;
                    return;
                }
                prefixX = 1f;
                prefixY = 1f;
                inversionMultiplierX = -1f;
                inversionMultiplierY = -1f;
                return;
        }
        isPortrait = false;
        if (!isTextureMirrored)
        {
            prefixX = 0f;
            prefixY = 0f;
            inversionMultiplierX = 1f;
            inversionMultiplierY = 1f;
        }
        else
        {
            prefixX = 1f;
            prefixY = 0f;
            inversionMultiplierX = -1f;
            inversionMultiplierY = 1f;
        }
    }

    public static void ResetSleepMode()
    {
        Screen.sleepTimeout = -2;
    }

    public static QCARRenderer.Vec2I ScreenSpaceToCameraFrameCoordinates(Vector2 screenSpaceCoordinate, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        float xMin = bgTextureViewPortRect.xMin;
        float yMin = bgTextureViewPortRect.yMin;
        float width = bgTextureViewPortRect.width;
        float height = bgTextureViewPortRect.height;
        bool isPortrait = false;
        float num5 = videoModeData.width;
        float num6 = videoModeData.height;
        float prefixX = 0f;
        float prefixY = 0f;
        float inversionMultiplierX = 0f;
        float inversionMultiplierY = 0f;
        PrepareCoordinateConversion(isTextureMirrored, ref prefixX, ref prefixY, ref inversionMultiplierX, ref inversionMultiplierY, ref isPortrait);
        float num11 = (screenSpaceCoordinate.x - xMin) / width;
        float num12 = (screenSpaceCoordinate.y - yMin) / height;
        if (isPortrait)
        {
            return new QCARRenderer.Vec2I(Mathf.RoundToInt((prefixX + (inversionMultiplierX * num12)) * num5), Mathf.RoundToInt((prefixY + (inversionMultiplierY * num11)) * num6));
        }
        return new QCARRenderer.Vec2I(Mathf.RoundToInt((prefixX + (inversionMultiplierX * num11)) * num5), Mathf.RoundToInt((prefixY + (inversionMultiplierY * num12)) * num6));
    }

    public static void SelectRectTopLeftAndBottomRightForLandscapeLeft(Rect screenSpaceRect, bool isMirrored, out Vector2 topLeft, out Vector2 bottomRight)
    {
        if (isMirrored)
        {
            switch (ScreenOrientation)
            {
                case UnityEngine.ScreenOrientation.Portrait:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    return;

                case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    return;

                case UnityEngine.ScreenOrientation.LandscapeRight:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    return;
            }
        }
        else
        {
            switch (ScreenOrientation)
            {
                case UnityEngine.ScreenOrientation.Portrait:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    return;

                case UnityEngine.ScreenOrientation.PortraitUpsideDown:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    return;

                case UnityEngine.ScreenOrientation.LandscapeRight:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    return;
            }
            topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
            bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
            return;
        }
        topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
        bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
    }

    public static string StripExtensionFromPath(string fullPath)
    {
        string[] strArray = fullPath.Split(new char[] { '.' });
        if (strArray.Length <= 1)
        {
            return "";
        }
        return strArray[strArray.Length - 1];
    }

    public static string StripFileNameFromPath(string fullPath)
    {
        string[] strArray = fullPath.Split(new char[] { '/' });
        return strArray[strArray.Length - 1];
    }

    public static bool IsLandscapeOrientation
    {
        get
        {
            UnityEngine.ScreenOrientation screenOrientation = ScreenOrientation;
            if ((screenOrientation != UnityEngine.ScreenOrientation.LandscapeLeft) && (screenOrientation != UnityEngine.ScreenOrientation.LandscapeLeft))
            {
                return (screenOrientation == UnityEngine.ScreenOrientation.LandscapeRight);
            }
            return true;
        }
    }

    public static bool IsPortraitOrientation
    {
        get
        {
            return !IsLandscapeOrientation;
        }
    }

    public static UnityEngine.ScreenOrientation ScreenOrientation
    {
        get
        {
            if (IsPlayMode())
            {
                if (sScreenOrientation != UnityEngine.ScreenOrientation.Unknown)
                {
                    return sScreenOrientation;
                }
                return UnityEngine.ScreenOrientation.LandscapeLeft;
            }
            if (sScreenOrientation == UnityEngine.ScreenOrientation.Unknown)
            {
                sScreenOrientation = (UnityEngine.ScreenOrientation) QCARWrapper.Instance.GetSurfaceOrientation();
            }
            return sScreenOrientation;
        }
    }

    private enum WebCamUsed
    {
        UNKNOWN,
        TRUE,
        FALSE
    }
}

