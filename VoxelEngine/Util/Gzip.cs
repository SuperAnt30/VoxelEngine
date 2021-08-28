using System.IO;
using System.IO.Compression;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект архивации
    /// </summary>
    public class Gzip
    {
        /// <summary>
        /// Компрессия
        /// </summary>
        public static byte[] Compress(byte[] src)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (MemoryStream mStream = new MemoryStream(src))
                    mStream.CopyTo(tinyStream);
                return outStream.ToArray();
            }
        }

        /// <summary>
        /// Декомпрессия
        /// </summary>
        public static byte[] Decompress(byte[] src)
        {
            using (MemoryStream inStream = new MemoryStream(src))
            using (GZipStream bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (MemoryStream bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                return bigStreamOut.ToArray();
            }
        }
    }
}
