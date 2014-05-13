using System;

public interface Marker : Trackable
{
    float GetSize();
    void SetSize(float size);

    int MarkerID { get; }
}

