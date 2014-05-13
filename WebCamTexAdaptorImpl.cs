using System;
using UnityEngine;

internal class WebCamTexAdaptorImpl : WebCamTexAdaptor
{
    private WebCamTexture mWebCamTexture = new WebCamTexture();

    public WebCamTexAdaptorImpl(string deviceName, int requestedFPS, QCARRenderer.Vec2I requestedTextureSize)
    {
        this.mWebCamTexture.deviceName = deviceName;
        this.mWebCamTexture.requestedFPS = requestedFPS;
        this.mWebCamTexture.requestedWidth = requestedTextureSize.x;
        this.mWebCamTexture.requestedHeight = requestedTextureSize.y;
    }

    public override void Play()
    {
        this.mWebCamTexture.Play();
    }

    public override void Stop()
    {
        this.mWebCamTexture.Stop();
    }

    public override bool DidUpdateThisFrame
    {
        get
        {
            return this.mWebCamTexture.didUpdateThisFrame;
        }
    }

    public override bool IsPlaying
    {
        get
        {
            return this.mWebCamTexture.isPlaying;
        }
    }

    public override UnityEngine.Texture Texture
    {
        get
        {
            return this.mWebCamTexture;
        }
    }
}

