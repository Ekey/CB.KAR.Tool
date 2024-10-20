using System;

namespace CB.Unpacker
{
    class KarHeader
    {
        public UInt32 dwTableSize { get; set; }
        public UInt32 dwTableOffset { get; set; }
        public UInt32 dwTableSeed { get; set; }
        public Int32 dwTotalFiles { get; set; }
        public String m_Password { get; set; }
        public String m_MD5Hash { get; set; }
        public String m_Compression { get; set; }
        public Int64 dwTimeStamp { get; set; }
        public Int32 bUnknown1 { get; set; }
        public Int32 bUnknown2 { get; set; }
        public Int32 bUnknown3 { get; set; }
    }
}
