using System;
using UnityEngine;

public abstract class WebCamTexAdaptor
{
    protected WebCamTexAdaptor()
    {
    }

    public abstract void Play();
    public abstract void Stop();

    public abstract bool DidUpdateThisFrame { get; }

    public abstract bool IsPlaying { get; }

    public abstract UnityEngine.Texture Texture { get; }
}

