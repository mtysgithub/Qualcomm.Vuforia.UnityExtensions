using System;
using System.Runtime.CompilerServices;

public abstract class TrackableImpl : Trackable
{
    protected TrackableImpl(string name, int id)
    {
        this.Name = name;
        this.ID = id;
    }

    public int ID { get; protected set; }

    public string Name { get; protected set; }
}

