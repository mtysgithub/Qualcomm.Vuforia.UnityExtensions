using System;
using UnityEngine;

public interface Word : Trackable
{
    RectangleData[] GetLetterBoundingBoxes();
    Image GetLetterMask();

    Vector2 Size { get; }

    string StringValue { get; }
}

