using System;
using UnityEngine;

internal class NullWebCamTexAdaptor : WebCamTexAdaptor
{
    private const string ERROR_MSG = "No camera connected!\nTo run your application using Play Mode, please connect a webcam to your computer.";
    private DateTime mLastFrame;
    private readonly double mMsBetweenFrames;
    private bool mPseudoPlaying = true;
    private readonly Texture2D mTexture;

    public NullWebCamTexAdaptor(int requestedFPS, QCARRenderer.Vec2I requestedTextureSize)
    {
        this.mTexture = new Texture2D(requestedTextureSize.x, requestedTextureSize.y);
        this.mMsBetweenFrames = 1000.0 / ((double) requestedFPS);
        this.mLastFrame = DateTime.Now - TimeSpan.FromDays(1.0);
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            PlayModeEditorUtility.Instance.DisplayDialog("Error occurred!", "No camera connected!\nTo run your application using Play Mode, please connect a webcam to your computer.", "Ok");
            Debug.LogError("No camera connected!\nTo run your application using Play Mode, please connect a webcam to your computer.");
        }
    }

    public override void Play()
    {
        this.mPseudoPlaying = true;
    }

    public override void Stop()
    {
        this.mPseudoPlaying = false;
    }

    public override bool DidUpdateThisFrame
    {
        get
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this.mLastFrame);
            if (span.TotalMilliseconds > this.mMsBetweenFrames)
            {
                this.mLastFrame = DateTime.Now;
                return true;
            }
            return false;
        }
    }

    public override bool IsPlaying
    {
        get
        {
            return this.mPseudoPlaying;
        }
    }

    public override UnityEngine.Texture Texture
    {
        get
        {
            return this.mTexture;
        }
    }
}

