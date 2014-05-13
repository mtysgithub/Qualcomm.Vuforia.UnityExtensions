using System;
using System.Collections.Generic;

public static class TypeMapping
{
    private static Dictionary<Type, ushort> sTypes;

    static TypeMapping()
    {
        Dictionary<Type, ushort> dictionary = new Dictionary<Type, ushort>();
        dictionary.Add(typeof(ImageTarget), 1);
        dictionary.Add(typeof(MultiTarget), 2);
        dictionary.Add(typeof(CylinderTarget), 3);
        dictionary.Add(typeof(Marker), 4);
        dictionary.Add(typeof(Word), 5);
        dictionary.Add(typeof(InternalRigidBodyTarget), 6);
        dictionary.Add(typeof(ImageTracker), 7);
        dictionary.Add(typeof(MarkerTracker), 8);
        dictionary.Add(typeof(TextTracker), 9);
        sTypes = dictionary;
    }

    internal static ushort GetTypeID(Type type)
    {
        return sTypes[type];
    }
}

