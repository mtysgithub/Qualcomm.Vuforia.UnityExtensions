using System;

public interface CylinderTarget : ExtendedTrackable, Trackable
{
    float GetBottomDiameter();
    float GetSideLength();
    float GetTopDiameter();
    bool SetBottomDiameter(float bottomDiameter);
    bool SetSideLength(float sideLength);
    bool SetTopDiameter(float topDiameter);
}

