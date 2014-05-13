using System;

public abstract class VirtualButton
{
    public const Sensitivity DEFAULT_SENSITIVITY = Sensitivity.LOW;

    protected VirtualButton()
    {
    }

    public abstract bool SetArea(RectangleData area);
    public abstract bool SetEnabled(bool enabled);
    public abstract bool SetSensitivity(Sensitivity sensitivity);

    public abstract RectangleData Area { get; }

    public abstract bool Enabled { get; }

    public abstract int ID { get; }

    public abstract string Name { get; }

    public enum Sensitivity
    {
        HIGH,
        MEDIUM,
        LOW
    }
}

