using System;
using System.IO;

public interface IUnzipper
{
    Stream UnzipFile(string path, string fileNameinZip);
}

