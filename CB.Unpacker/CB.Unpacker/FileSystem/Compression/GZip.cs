using System;
using System.IO;
using System.IO.Compression;

namespace CB.Unpacker
{
    class GZip
    {
        public static Byte[] iDecompress(Byte[] lpBuffer)
        {
            var TResult = new MemoryStream();
            using (MemoryStream TMemoryStream = new MemoryStream(lpBuffer))
            {
                using (GZipStream TGZipStream = new GZipStream(TMemoryStream, CompressionMode.Decompress, false))
                {
                    TGZipStream.CopyTo(TResult);
                    TGZipStream.Dispose();
                }

                TMemoryStream.Dispose();
            }

            return TResult.ToArray();
        }
    }
}
