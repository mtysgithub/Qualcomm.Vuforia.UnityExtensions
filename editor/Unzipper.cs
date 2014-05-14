using System;
using System.IO;

public class Unzipper
{
    private static IUnzipper sInstance;

    public static IUnzipper Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new NullUnzipper();
            }
            return sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    private class NullUnzipper : IUnzipper
    {
        public Stream UnzipFile(string path, string fileNameinZip)
        {
            return null;
        }
    }
}

