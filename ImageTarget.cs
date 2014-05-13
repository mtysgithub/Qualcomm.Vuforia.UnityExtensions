using System;
using System.Collections.Generic;
using UnityEngine;

public interface ImageTarget : ExtendedTrackable, Trackable
{
    VirtualButton CreateVirtualButton(string name, RectangleData area);
    bool DestroyVirtualButton(VirtualButton vb);
    Vector2 GetSize();
    VirtualButton GetVirtualButtonByName(string name);
    IEnumerable<VirtualButton> GetVirtualButtons();
    void SetSize(Vector2 size);

    ImageTargetType ImageTargetType { get; }
}

