using System;
using System.Runtime.InteropServices;
using System.Text;

internal class QCARNativeIosWrapper : IQCARWrapper
{
    [DllImport("__Internal")]
    private static extern int cameraDeviceDeinitCamera();
    public int CameraDeviceDeinitCamera()
    {
        return cameraDeviceDeinitCamera();
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceGetNumVideoModes();
    public int CameraDeviceGetNumVideoModes()
    {
        return cameraDeviceGetNumVideoModes();
    }

    [DllImport("__Internal")]
    private static extern void cameraDeviceGetVideoMode(int idx, [In, Out] IntPtr videoMode);
    public void CameraDeviceGetVideoMode(int idx, [In, Out] IntPtr videoMode)
    {
        cameraDeviceGetVideoMode(idx, videoMode);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceInitCamera(int camera);
    public int CameraDeviceInitCamera(int camera)
    {
        return cameraDeviceInitCamera(camera);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceSelectVideoMode(int idx);
    public int CameraDeviceSelectVideoMode(int idx)
    {
        return cameraDeviceSelectVideoMode(idx);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceSetCameraConfiguration(int width, int height);
    public int CameraDeviceSetCameraConfiguration(int width, int height)
    {
        return cameraDeviceSetCameraConfiguration(width, height);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceSetFlashTorchMode(int on);
    public int CameraDeviceSetFlashTorchMode(int on)
    {
        return cameraDeviceSetFlashTorchMode(on);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceSetFocusMode(int focusMode);
    public int CameraDeviceSetFocusMode(int focusMode)
    {
        return cameraDeviceSetFocusMode(focusMode);
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceStartCamera();
    public int CameraDeviceStartCamera()
    {
        return cameraDeviceStartCamera();
    }

    [DllImport("__Internal")]
    private static extern int cameraDeviceStopCamera();
    public int CameraDeviceStopCamera()
    {
        return cameraDeviceStopCamera();
    }

    [DllImport("__Internal")]
    private static extern int cylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr dimensions);
    public int CylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr dimensions)
    {
        return cylinderTargetGetSize(dataSetPtr, trackableName, dimensions);
    }

    [DllImport("__Internal")]
    private static extern int cylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter);
    public int CylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter)
    {
        return cylinderTargetSetBottomDiameter(dataSetPtr, trackableName, bottomDiameter);
    }

    [DllImport("__Internal")]
    private static extern int cylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength);
    public int CylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength)
    {
        return cylinderTargetSetSideLength(dataSetPtr, trackableName, sideLength);
    }

    [DllImport("__Internal")]
    private static extern int cylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter);
    public int CylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter)
    {
        return cylinderTargetSetTopDiameter(dataSetPtr, trackableName, topDiameter);
    }

    [DllImport("__Internal")]
    private static extern int dataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, StringBuilder trackableName, int nameMaxLength, [In, Out] IntPtr trackableData);
    public int DataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, StringBuilder trackableName, int nameMaxLength, [In, Out] IntPtr trackableData)
    {
        return dataSetCreateTrackable(dataSetPtr, trackableSourcePtr, trackableName, nameMaxLength, trackableData);
    }

    [DllImport("__Internal")]
    private static extern int dataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId);
    public int DataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId)
    {
        return dataSetDestroyTrackable(dataSetPtr, trackableId);
    }

    [DllImport("__Internal")]
    private static extern int dataSetExists(string relativePath, int storageType);
    public int DataSetExists(string relativePath, int storageType)
    {
        return dataSetExists(relativePath, storageType);
    }

    [DllImport("__Internal")]
    private static extern int dataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr);
    public int DataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr)
    {
        return dataSetGetNumTrackableType(trackableType, dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern int dataSetGetTrackableName(IntPtr dataSetPtr, int trackableId, StringBuilder trackableName, int nameMaxLength);
    public int DataSetGetTrackableName(IntPtr dataSetPtr, int trackableId, StringBuilder trackableName, int nameMaxLength)
    {
        return dataSetGetTrackableName(dataSetPtr, trackableId, trackableName, nameMaxLength);
    }

    [DllImport("__Internal")]
    private static extern int dataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray, int trackableDataArrayLength, IntPtr dataSetPtr);
    public int DataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray, int trackableDataArrayLength, IntPtr dataSetPtr)
    {
        return dataSetGetTrackablesOfType(trackableType, trackableDataArray, trackableDataArrayLength, dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern int dataSetHasReachedTrackableLimit(IntPtr dataSetPtr);
    public int DataSetHasReachedTrackableLimit(IntPtr dataSetPtr)
    {
        return dataSetHasReachedTrackableLimit(dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern int dataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr);
    public int DataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr)
    {
        return dataSetLoad(relativePath, storageType, dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern void deinitFrameState([In, Out] IntPtr frameIndex);
    public void DeinitFrameState([In, Out] IntPtr frameIndex)
    {
        deinitFrameState(frameIndex);
    }

    [DllImport("__Internal")]
    private static extern int getInitErrorCode();
    public int GetInitErrorCode()
    {
        return getInitErrorCode();
    }

    [DllImport("__Internal")]
    private static extern int getProjectionGL(float nearClip, float farClip, [In, Out] IntPtr projMatrix, int screenOrientation);
    public int GetProjectionGL(float nearClip, float farClip, [In, Out] IntPtr projMatrix, int screenOrientation)
    {
        return getProjectionGL(nearClip, farClip, projMatrix, screenOrientation);
    }

    [DllImport("__Internal")]
    private static extern int getSurfaceOrientation();
    public int GetSurfaceOrientation()
    {
        return getSurfaceOrientation();
    }

    [DllImport("__Internal")]
    private static extern int hasSurfaceBeenRecreated();
    public bool HasSurfaceBeenRecreated()
    {
        return (hasSurfaceBeenRecreated() == 1);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetBuilderBuild(string targetName, float sceenSizeWidth);
    public int ImageTargetBuilderBuild(string targetName, float sceenSizeWidth)
    {
        return imageTargetBuilderBuild(targetName, sceenSizeWidth);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetBuilderGetFrameQuality();
    public int ImageTargetBuilderGetFrameQuality()
    {
        return imageTargetBuilderGetFrameQuality();
    }

    [DllImport("__Internal")]
    private static extern IntPtr imageTargetBuilderGetTrackableSource();
    public IntPtr ImageTargetBuilderGetTrackableSource()
    {
        return imageTargetBuilderGetTrackableSource();
    }

    [DllImport("__Internal")]
    private static extern void imageTargetBuilderStartScan();
    public void ImageTargetBuilderStartScan()
    {
        imageTargetBuilderStartScan();
    }

    [DllImport("__Internal")]
    private static extern void imageTargetBuilderStopScan();
    public void ImageTargetBuilderStopScan()
    {
        imageTargetBuilderStopScan();
    }

    [DllImport("__Internal")]
    private static extern int imageTargetCreateVirtualButton(IntPtr dataSetPtr, string trackableName, string virtualButtonName, [In, Out] IntPtr rectData);
    public int ImageTargetCreateVirtualButton(IntPtr dataSetPtr, string trackableName, string virtualButtonName, [In, Out] IntPtr rectData)
    {
        return imageTargetCreateVirtualButton(dataSetPtr, trackableName, virtualButtonName, rectData);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetDestroyVirtualButton(IntPtr dataSetPtr, string trackableName, string virtualButtonName);
    public int ImageTargetDestroyVirtualButton(IntPtr dataSetPtr, string trackableName, string virtualButtonName)
    {
        return imageTargetDestroyVirtualButton(dataSetPtr, trackableName, virtualButtonName);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName);
    public int ImageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName)
    {
        return imageTargetGetNumVirtualButtons(dataSetPtr, trackableName);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr size);
    public int ImageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr size)
    {
        return imageTargetGetSize(dataSetPtr, trackableName, size);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetGetVirtualButtonName(IntPtr dataSetPtr, string trackableName, int idx, StringBuilder vbName, int nameMaxLength);
    public int ImageTargetGetVirtualButtonName(IntPtr dataSetPtr, string trackableName, int idx, StringBuilder vbName, int nameMaxLength)
    {
        return imageTargetGetVirtualButtonName(dataSetPtr, trackableName, idx, vbName, nameMaxLength);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetGetVirtualButtons([In, Out] IntPtr virtualButtonDataArray, [In, Out] IntPtr rectangleDataArray, int virtualButtonDataArrayLength, IntPtr dataSetPtr, string trackableName);
    public int ImageTargetGetVirtualButtons([In, Out] IntPtr virtualButtonDataArray, [In, Out] IntPtr rectangleDataArray, int virtualButtonDataArrayLength, IntPtr dataSetPtr, string trackableName)
    {
        return imageTargetGetVirtualButtons(virtualButtonDataArray, rectangleDataArray, virtualButtonDataArrayLength, dataSetPtr, trackableName);
    }

    [DllImport("__Internal")]
    private static extern int imageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr size);
    public int ImageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr size)
    {
        return imageTargetSetSize(dataSetPtr, trackableName, size);
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerActivateDataSet(IntPtr dataSetPtr);
    public int ImageTrackerActivateDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerActivateDataSet(dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern IntPtr imageTrackerCreateDataSet();
    public IntPtr ImageTrackerCreateDataSet()
    {
        return imageTrackerCreateDataSet();
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerDeactivateDataSet(IntPtr dataSetPtr);
    public int ImageTrackerDeactivateDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerDeactivateDataSet(dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerDestroyDataSet(IntPtr dataSetPtr);
    public int ImageTrackerDestroyDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerDestroyDataSet(dataSetPtr);
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerPersistExtendedTracking(int on);
    public int ImageTrackerPersistExtendedTracking(int on)
    {
        return imageTrackerPersistExtendedTracking(on);
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerResetExtendedTracking();
    public int ImageTrackerResetExtendedTracking()
    {
        return imageTrackerResetExtendedTracking();
    }

    [DllImport("__Internal")]
    private static extern int imageTrackerStart();
    public int ImageTrackerStart()
    {
        return imageTrackerStart();
    }

    [DllImport("__Internal")]
    private static extern void imageTrackerStop();
    public void ImageTrackerStop()
    {
        imageTrackerStop();
    }

    [DllImport("__Internal")]
    private static extern void initFrameState([In, Out] IntPtr frameIndex);
    public void InitFrameState([In, Out] IntPtr frameIndex)
    {
        initFrameState(frameIndex);
    }

    [DllImport("__Internal")]
    private static extern int isRendererDirty();
    public int IsRendererDirty()
    {
        return isRendererDirty();
    }

    [DllImport("__Internal")]
    private static extern int markerSetSize(int trackableIndex, float size);
    public int MarkerSetSize(int trackableIndex, float size)
    {
        return markerSetSize(trackableIndex, size);
    }

    [DllImport("__Internal")]
    private static extern int markerTrackerCreateMarker(int id, string trackableName, float size);
    public int MarkerTrackerCreateMarker(int id, string trackableName, float size)
    {
        return markerTrackerCreateMarker(id, trackableName, size);
    }

    [DllImport("__Internal")]
    private static extern int markerTrackerDestroyMarker(int trackableId);
    public int MarkerTrackerDestroyMarker(int trackableId)
    {
        return markerTrackerDestroyMarker(trackableId);
    }

    [DllImport("__Internal")]
    private static extern int markerTrackerStart();
    public int MarkerTrackerStart()
    {
        return markerTrackerStart();
    }

    [DllImport("__Internal")]
    private static extern void markerTrackerStop();
    public void MarkerTrackerStop()
    {
        markerTrackerStop();
    }

    [DllImport("__Internal")]
    private static extern void onSurfaceChanged(int width, int height);
    public void OnSurfaceChanged(int width, int height)
    {
        onSurfaceChanged(width, height);
    }

    [DllImport("__Internal")]
    private static extern void onSurfaceCreated();
    public void OnSurfaceCreated()
    {
        onSurfaceCreated();
    }

    [DllImport("__Internal")]
    private static extern int pausedUpdateQCAR();
    public int PausedUpdateQCAR()
    {
        return pausedUpdateQCAR();
    }

    [DllImport("__Internal")]
    private static extern void qcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally);
    public void QcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally)
    {
        qcarAddCameraFrame(pixels, width, height, format, stride, frameIdx, flipHorizontally);
    }

    [DllImport("__Internal")]
    private static extern int qcarDeinit();
    public int QcarDeinit()
    {
        return qcarDeinit();
    }

    [DllImport("__Internal")]
    private static extern int qcarGetBufferSize(int width, int height, int format);
    public int QcarGetBufferSize(int width, int height, int format)
    {
        return qcarGetBufferSize(width, height, format);
    }

    [DllImport("__Internal")]
    private static extern int qcarRequiresAlpha();
    public int QcarRequiresAlpha()
    {
        return qcarRequiresAlpha();
    }

    [DllImport("__Internal")]
    private static extern int qcarSetFrameFormat(int format, int enabled);
    public int QcarSetFrameFormat(int format, int enabled)
    {
        return qcarSetFrameFormat(format, enabled);
    }

    [DllImport("__Internal")]
    private static extern int qcarSetHint(int hint, int value);
    public int QcarSetHint(int hint, int value)
    {
        return qcarSetHint(hint, value);
    }

    [DllImport("__Internal")]
    private static extern void rendererEnd();
    public void RendererEnd()
    {
        rendererEnd();
    }

    [DllImport("__Internal")]
    private static extern void rendererGetVideoBackgroundCfg([In, Out] IntPtr bgCfg);
    public void RendererGetVideoBackgroundCfg([In, Out] IntPtr bgCfg)
    {
        rendererGetVideoBackgroundCfg(bgCfg);
    }

    [DllImport("__Internal")]
    private static extern void rendererGetVideoBackgroundTextureInfo([In, Out] IntPtr texInfo);
    public void RendererGetVideoBackgroundTextureInfo([In, Out] IntPtr texInfo)
    {
        rendererGetVideoBackgroundTextureInfo(texInfo);
    }

    [DllImport("__Internal")]
    private static extern int rendererIsVideoBackgroundTextureInfoAvailable();
    public int RendererIsVideoBackgroundTextureInfoAvailable()
    {
        return rendererIsVideoBackgroundTextureInfoAvailable();
    }

    [DllImport("__Internal")]
    private static extern void rendererRenderVideoBackground(int bindVideoBackground);
    public void RendererRenderVideoBackground(int bindVideoBackground)
    {
        rendererRenderVideoBackground(bindVideoBackground);
    }

    [DllImport("__Internal")]
    private static extern void rendererSetVideoBackgroundCfg([In, Out] IntPtr bgCfg);
    public void RendererSetVideoBackgroundCfg([In, Out] IntPtr bgCfg)
    {
        rendererSetVideoBackgroundCfg(bgCfg);
    }

    [DllImport("__Internal")]
    private static extern int rendererSetVideoBackgroundTextureID(int textureID);
    public int RendererSetVideoBackgroundTextureID(int textureID)
    {
        return rendererSetVideoBackgroundTextureID(textureID);
    }

    [DllImport("__Internal")]
    private static extern void setSurfaceOrientation(int orientation);
    public void SetSurfaceOrientation(int orientation)
    {
        setSurfaceOrientation(orientation);
    }

    [DllImport("__Internal")]
    private static extern void setUnityVersion(int major, int minor, int change);
    public void SetUnityVersion(int major, int minor, int change)
    {
        setUnityVersion(major, minor, change);
    }

    [DllImport("__Internal")]
    private static extern int startExtendedTracking(IntPtr dataSetPtr, int trackableId);
    public int StartExtendedTracking(IntPtr dataSetPtr, int trackableId)
    {
        return startExtendedTracking(dataSetPtr, trackableId);
    }

    [DllImport("__Internal")]
    private static extern int stopExtendedTracking(IntPtr dataSetPtr, int trackableId);
    public int StopExtendedTracking(IntPtr dataSetPtr, int trackableId)
    {
        return stopExtendedTracking(dataSetPtr, trackableId);
    }

    [DllImport("__Internal")]
    private static extern void targetFinderClearTrackables();
    public void TargetFinderClearTrackables()
    {
        targetFinderClearTrackables();
    }

    [DllImport("__Internal")]
    private static extern int targetFinderDeinit();
    public int TargetFinderDeinit()
    {
        return targetFinderDeinit();
    }

    [DllImport("__Internal")]
    private static extern int targetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData);
    public int TargetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData)
    {
        return targetFinderEnableTracking(searchResult, trackableData);
    }

    [DllImport("__Internal")]
    private static extern void targetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength);
    public void TargetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength)
    {
        targetFinderGetImageTargets(trackableIdArray, trackableIdArrayLength);
    }

    [DllImport("__Internal")]
    private static extern int targetFinderGetInitState();
    public int TargetFinderGetInitState()
    {
        return targetFinderGetInitState();
    }

    [DllImport("__Internal")]
    private static extern int targetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength);
    public int TargetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength)
    {
        return targetFinderGetResults(searchResultArray, searchResultArrayLength);
    }

    [DllImport("__Internal")]
    private static extern void targetFinderSetUIPointColor(float r, float g, float b);
    public void TargetFinderSetUIPointColor(float r, float g, float b)
    {
        targetFinderSetUIPointColor(r, g, b);
    }

    [DllImport("__Internal")]
    private static extern void targetFinderSetUIScanlineColor(float r, float g, float b);
    public void TargetFinderSetUIScanlineColor(float r, float g, float b)
    {
        targetFinderSetUIScanlineColor(r, g, b);
    }

    [DllImport("__Internal")]
    private static extern int targetFinderStartInit(string userKey, string secretKey);
    public int TargetFinderStartInit(string userKey, string secretKey)
    {
        return targetFinderStartInit(userKey, secretKey);
    }

    [DllImport("__Internal")]
    private static extern int targetFinderStartRecognition();
    public int TargetFinderStartRecognition()
    {
        return targetFinderStartRecognition();
    }

    [DllImport("__Internal")]
    private static extern int targetFinderStop();
    public int TargetFinderStop()
    {
        return targetFinderStop();
    }

    [DllImport("__Internal")]
    private static extern void targetFinderUpdate([In, Out] IntPtr targetFinderState);
    public void TargetFinderUpdate([In, Out] IntPtr targetFinderState)
    {
        targetFinderUpdate(targetFinderState);
    }

    [DllImport("__Internal")]
    private static extern int textTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI);
    public void TextTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI)
    {
        textTrackerGetRegionOfInterest(detectionROI, trackingROI);
    }

    [DllImport("__Internal")]
    private static extern int textTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY, int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection);
    public int TextTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY, int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection)
    {
        return textTrackerSetRegionOfInterest(detectionLeftTopX, detectionLeftTopY, detectionRightBottomX, detectionRightBottomY, trackingLeftTopX, trackingLeftTopY, trackingRightBottomX, trackingRightBottomY, upDirection);
    }

    [DllImport("__Internal")]
    private static extern int textTrackerStart();
    public int TextTrackerStart()
    {
        return textTrackerStart();
    }

    [DllImport("__Internal")]
    private static extern void textTrackerStop();
    public void TextTrackerStop()
    {
        textTrackerStop();
    }

    [DllImport("__Internal")]
    private static extern int trackerManagerDeinitTracker(int trackerType);
    public int TrackerManagerDeinitTracker(int trackerType)
    {
        return trackerManagerDeinitTracker(trackerType);
    }

    [DllImport("__Internal")]
    private static extern int trackerManagerInitTracker(int trackerType);
    public int TrackerManagerInitTracker(int trackerType)
    {
        return trackerManagerInitTracker(trackerType);
    }

    [DllImport("__Internal")]
    private static extern int updateQCAR([In, Out] IntPtr imageHeaderDataArray, int imageHeaderArrayLength, [In, Out] IntPtr frameIndex, int screenOrientation, int videoModeIdx);
    public int UpdateQCAR([In, Out] IntPtr imageHeaderDataArray, int imageHeaderArrayLength, [In, Out] IntPtr frameIndex, int screenOrientation, int videoModeIdx)
    {
        return updateQCAR(imageHeaderDataArray, imageHeaderArrayLength, frameIndex, screenOrientation, videoModeIdx);
    }

    [DllImport("__Internal")]
    private static extern int virtualButtonGetId(IntPtr dataSetPtr, string trackableName, string virtualButtonName);
    public int VirtualButtonGetId(IntPtr dataSetPtr, string trackableName, string virtualButtonName)
    {
        return virtualButtonGetId(dataSetPtr, trackableName, virtualButtonName);
    }

    [DllImport("__Internal")]
    private static extern int virtualButtonSetAreaRectangle(IntPtr dataSetPtr, string trackableName, string virtualButtonName, [In, Out] IntPtr rectData);
    public int VirtualButtonSetAreaRectangle(IntPtr dataSetPtr, string trackableName, string virtualButtonName, [In, Out] IntPtr rectData)
    {
        return virtualButtonSetAreaRectangle(dataSetPtr, trackableName, virtualButtonName, rectData);
    }

    [DllImport("__Internal")]
    private static extern int virtualButtonSetEnabled(IntPtr dataSetPtr, string trackableName, string virtualButtonName, int enabled);
    public int VirtualButtonSetEnabled(IntPtr dataSetPtr, string trackableName, string virtualButtonName, int enabled)
    {
        return virtualButtonSetEnabled(dataSetPtr, trackableName, virtualButtonName, enabled);
    }

    [DllImport("__Internal")]
    private static extern int virtualButtonSetSensitivity(IntPtr dataSetPtr, string trackableName, string virtualButtonName, int sensitivity);
    public int VirtualButtonSetSensitivity(IntPtr dataSetPtr, string trackableName, string virtualButtonName, int sensitivity)
    {
        return virtualButtonSetSensitivity(dataSetPtr, trackableName, virtualButtonName, sensitivity);
    }

    [DllImport("__Internal")]
    private static extern int wordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes);
    public int WordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes)
    {
        return wordGetLetterBoundingBoxes(wordID, letterBoundingBoxes);
    }

    [DllImport("__Internal")]
    private static extern int wordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage);
    public int WordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage)
    {
        return wordGetLetterMask(wordID, letterMaskImage);
    }

    [DllImport("__Internal")]
    private static extern int wordListAddWordsFromFile(string path, int storageType);
    public int WordListAddWordsFromFile(string path, int storagetType)
    {
        return wordListAddWordsFromFile(path, storagetType);
    }

    [DllImport("__Internal")]
    private static extern int wordListAddWordToFilterListU(IntPtr word);
    public int WordListAddWordToFilterListU(IntPtr word)
    {
        return wordListAddWordToFilterListU(word);
    }

    [DllImport("__Internal")]
    private static extern int wordListAddWordU(IntPtr word);
    public int WordListAddWordU(IntPtr word)
    {
        return wordListAddWordU(word);
    }

    [DllImport("__Internal")]
    private static extern int wordListClearFilterList();
    public int WordListClearFilterList()
    {
        return wordListClearFilterList();
    }

    [DllImport("__Internal")]
    private static extern int wordListContainsWordU(IntPtr word);
    public int WordListContainsWordU(IntPtr word)
    {
        return wordListContainsWordU(word);
    }

    [DllImport("__Internal")]
    private static extern int wordListGetFilterListWordCount();
    public int WordListGetFilterListWordCount()
    {
        return wordListGetFilterListWordCount();
    }

    [DllImport("__Internal")]
    private static extern IntPtr wordListGetFilterListWordU(int i);
    public IntPtr WordListGetFilterListWordU(int i)
    {
        return wordListGetFilterListWordU(i);
    }

    [DllImport("__Internal")]
    private static extern int wordListGetFilterMode();
    public int WordListGetFilterMode()
    {
        return wordListGetFilterMode();
    }

    [DllImport("__Internal")]
    private static extern int wordListLoadFilterList(string path, int storageType);
    public int WordListLoadFilterList(string path, int storageType)
    {
        return wordListLoadFilterList(path, storageType);
    }

    [DllImport("__Internal")]
    private static extern int wordListLoadWordList(string path, int storageType);
    public int WordListLoadWordList(string path, int storageType)
    {
        return wordListLoadWordList(path, storageType);
    }

    [DllImport("__Internal")]
    private static extern int wordListRemoveWordFromFilterListU(IntPtr word);
    public int WordListRemoveWordFromFilterListU(IntPtr word)
    {
        return wordListRemoveWordFromFilterListU(word);
    }

    [DllImport("__Internal")]
    private static extern int wordListRemoveWordU(IntPtr word);
    public int WordListRemoveWordU(IntPtr word)
    {
        return wordListRemoveWordU(word);
    }

    [DllImport("__Internal")]
    private static extern int wordListSetFilterMode(int mode);
    public int WordListSetFilterMode(int mode)
    {
        return wordListSetFilterMode(mode);
    }

    [DllImport("__Internal")]
    private static extern int wordListUnloadAllLists();
    public int WordListUnloadAllLists()
    {
        return wordListUnloadAllLists();
    }
}

