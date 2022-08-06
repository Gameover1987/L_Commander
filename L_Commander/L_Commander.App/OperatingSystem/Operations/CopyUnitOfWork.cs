using System;
using System.IO;
using System.Threading;

namespace L_Commander.App.OperatingSystem.Operations;

public class CopyUnitOfWork : IUnitOfWork
{
    public CopyUnitOfWork(string sourceRoot, string sourcePath, long totalSize, string destinationDirectory)
    {
        SourcePath = sourcePath;

        var sourceWithoutRoot = sourcePath.Substring(sourceRoot.Length + 1);
        DestinationPath = destinationDirectory + "\\" + sourceWithoutRoot;
        TotalSize = totalSize;
        Copied = 0;
    }

    public string SourcePath { get; }

    public long TotalSize { get; }

    public long Copied { get; private set; }

    public int Percent
    {
        get
        {
            var bytesToPercent = TotalSize / 100.0;

            return (int)Math.Round(Copied / bytesToPercent);
        }
    }

    public string DestinationPath { get; }


    public event EventHandler Progress;

    public void Do(CancellationToken cancellationToken)
    {
        var fromFile = new FileStream(SourcePath, FileMode.Open, FileAccess.Read);
        var toFile = new FileStream(DestinationPath, FileMode.Append, FileAccess.Write);

        var bufferSize = 1024 * 1024 * 5;

        if (bufferSize < fromFile.Length)
        {
            var buffer = new byte[bufferSize];
            Copied = 0;
            while (Copied <= fromFile.Length - bufferSize)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    fromFile.Close();
                    toFile.Close();
                    File.Delete(DestinationPath);
                    return;
                }

                var toCopyLength = fromFile.Read(buffer, 0, bufferSize);
                fromFile.Flush();
                toFile.Write(buffer, 0, bufferSize);
                toFile.Flush();

                toFile.Position = fromFile.Position;
                Copied += toCopyLength;
                Progress?.Invoke(this, EventArgs.Empty);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                fromFile.Close();
                toFile.Close();
                File.Delete(DestinationPath);
                return;
            }

            var left = (int)(fromFile.Length - Copied);
            fromFile.Read(buffer, 0, left);
            fromFile.Flush();
            toFile.Write(buffer, 0, left);
            toFile.Flush();
        }
        else
        {
            var buffer = new byte[fromFile.Length];
            fromFile.Read(buffer, 0, buffer.Length);
            fromFile.Flush();
            toFile.Write(buffer, 0, buffer.Length);
            toFile.Flush();
        }

        Copied = TotalSize;
        fromFile.Close();
        toFile.Close();
    }

}
