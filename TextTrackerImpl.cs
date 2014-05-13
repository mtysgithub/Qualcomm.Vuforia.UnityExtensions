using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TextTrackerImpl : TextTracker
{
    private readonly WordList mWordList = new WordListImpl();

    public override bool GetRegionOfInterest(out Rect detectionRegion, out Rect trackingRegion)
    {
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
            detectionRegion = new Rect();
            trackingRegion = new Rect();
            return false;
        }
        Rect viewportRectangle = behaviour.GetViewportRectangle();
        bool videoBackGroundMirrored = behaviour.VideoBackGroundMirrored;
        CameraDevice.VideoModeData videoMode = CameraDevice.Instance.GetVideoMode(behaviour.CameraDeviceMode);
        IntPtr detectionROI = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleIntData)));
        IntPtr trackingROI = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleIntData)));
        QCARWrapper.Instance.TextTrackerGetRegionOfInterest(detectionROI, trackingROI);
        RectangleIntData camSpaceRectData = (RectangleIntData) Marshal.PtrToStructure(detectionROI, typeof(RectangleIntData));
        RectangleIntData data3 = (RectangleIntData) Marshal.PtrToStructure(trackingROI, typeof(RectangleIntData));
        Marshal.FreeHGlobal(detectionROI);
        Marshal.FreeHGlobal(trackingROI);
        detectionRegion = this.ScreenSpaceRectFromCamSpaceRectData(camSpaceRectData, viewportRectangle, videoBackGroundMirrored, videoMode);
        trackingRegion = this.ScreenSpaceRectFromCamSpaceRectData(data3, viewportRectangle, videoBackGroundMirrored, videoMode);
        return true;
    }

    private Rect ScreenSpaceRectFromCamSpaceRectData(RectangleIntData camSpaceRectData, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        Vector2 topLeft = QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(new Vector2((float) camSpaceRectData.leftTopX, (float) camSpaceRectData.leftTopY), bgTextureViewPortRect, isTextureMirrored, videoModeData);
        Vector2 bottomRight = QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(new Vector2((float) camSpaceRectData.rightBottomX, (float) camSpaceRectData.rightBottomY), bgTextureViewPortRect, isTextureMirrored, videoModeData);
        return QCARRuntimeUtilities.CalculateRectFromLandscapeLeftCorners(topLeft, bottomRight, isTextureMirrored);
    }

    public override bool SetRegionOfInterest(Rect detectionRegion, Rect trackingRegion)
    {
        Vector2 vector;
        Vector2 vector2;
        Vector2 vector3;
        Vector2 vector4;
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
            return false;
        }
        Rect viewportRectangle = behaviour.GetViewportRectangle();
        bool videoBackGroundMirrored = behaviour.VideoBackGroundMirrored;
        CameraDevice.VideoModeData videoMode = CameraDevice.Instance.GetVideoMode(behaviour.CameraDeviceMode);
        QCARRuntimeUtilities.SelectRectTopLeftAndBottomRightForLandscapeLeft(detectionRegion, videoBackGroundMirrored, out vector, out vector2);
        QCARRuntimeUtilities.SelectRectTopLeftAndBottomRightForLandscapeLeft(trackingRegion, videoBackGroundMirrored, out vector3, out vector4);
        QCARRenderer.Vec2I veci = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(vector, viewportRectangle, videoBackGroundMirrored, videoMode);
        QCARRenderer.Vec2I veci2 = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(vector2, viewportRectangle, videoBackGroundMirrored, videoMode);
        QCARRenderer.Vec2I veci3 = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(vector3, viewportRectangle, videoBackGroundMirrored, videoMode);
        QCARRenderer.Vec2I veci4 = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(vector4, viewportRectangle, videoBackGroundMirrored, videoMode);
        if (QCARWrapper.Instance.TextTrackerSetRegionOfInterest(veci.x, veci.y, veci2.x, veci2.y, veci3.x, veci3.y, veci4.x, veci4.y, (int) this.CurrentUpDirection) == 0)
        {
            Debug.LogError(string.Format("Could not set region of interest: ({0}, {1}, {2}, {3}) - ({4}, {5}, {6}, {7})", new object[] { detectionRegion.x, detectionRegion.y, detectionRegion.width, detectionRegion.height, trackingRegion.x, trackingRegion.y, trackingRegion.width, trackingRegion.height }));
            return false;
        }
        return true;
    }

    public override bool Start()
    {
        if (QCARWrapper.Instance.TextTrackerStart() == 0)
        {
            Debug.LogError("Could not start tracker.");
            return false;
        }
        return true;
    }

    public override void Stop()
    {
        QCARWrapper.Instance.TextTrackerStop();
        ((WordManagerImpl) TrackerManager.Instance.GetStateManager().GetWordManager()).SetWordBehavioursToNotFound();
    }

    private UpDirection CurrentUpDirection
    {
        get
        {
            switch (QCARRuntimeUtilities.ScreenOrientation)
            {
                case ScreenOrientation.Portrait:
                    return UpDirection.TEXTTRACKER_UP_IS_9_HRS;

                case ScreenOrientation.PortraitUpsideDown:
                    return UpDirection.TEXTTRACKER_UP_IS_3_HRS;

                case ScreenOrientation.LandscapeRight:
                    return UpDirection.TEXTTRACKER_UP_IS_6_HRS;
            }
            return UpDirection.TEXTTRACKER_UP_IS_0_HRS;
        }
    }

    public override WordList WordList
    {
        get
        {
            return this.mWordList;
        }
    }

    private enum UpDirection
    {
        TEXTTRACKER_UP_IS_0_HRS = 1,
        TEXTTRACKER_UP_IS_3_HRS = 2,
        TEXTTRACKER_UP_IS_6_HRS = 3,
        TEXTTRACKER_UP_IS_9_HRS = 4
    }
}

