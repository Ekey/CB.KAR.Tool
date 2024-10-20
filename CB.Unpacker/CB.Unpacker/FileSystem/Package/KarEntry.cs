using System;

namespace CB.Unpacker
{
    class KarEntry
    {
        public String m_FirstPath { get; set; }
        public String m_SecondPath { get; set; }
        public String m_FilePassword { get; set; }
        public Int32 dwSize { get; set; }
        public Int64 dwOffset { get; set; }
        public UInt32 dwSeed { get; set; }
    }
}
