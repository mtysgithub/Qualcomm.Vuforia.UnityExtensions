using System;

public abstract class Tracker
{
    protected Tracker()
    {
    }

    public abstract bool Start();
    public abstract void Stop();
}

