using System.IO;

namespace L_Commander.Common.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream, bool disposeSourceStream = true)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                if (disposeSourceStream)
                    stream.Dispose();

                return memoryStream.ToArray();
            }
        }
    }
}
