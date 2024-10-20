using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CB.Unpacker
{
    class KarUnpack
    {
        private static List<KarEntry> m_EntryTable = new List<KarEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            using (FileStream TKarStream = File.OpenRead(m_Archive))
            {
                var m_Header = new KarHeader();

                m_Header.dwTableSize = TKarStream.ReadUInt32(true);
                m_Header.dwTableSize ^= 0x85FD91A1;

                if (m_Header.dwTableSize <= 0 || m_Header.dwTableSize >= TKarStream.Length)
                {
                    Utils.iSetError("[ERROR]: Invalid KAR archive file");
                    return;
                }

                m_Header.dwTableOffset = (UInt32)TKarStream.Length;
                m_Header.dwTableOffset -= (UInt32)m_Header.dwTableSize;
                m_Header.dwTableSeed = KarHash.iGetHash(m_Header.dwTableOffset.ToString());
                m_Header.m_Password = KarUtils.iGetPassword(Path.GetFileName(m_Archive));

                TKarStream.Seek(m_Header.dwTableOffset, SeekOrigin.Begin);
                var lpEntryTable = TKarStream.ReadBytes((Int32)m_Header.dwTableSize);

                lpEntryTable = KarCipher.iDecrypt(lpEntryTable, m_Header.dwTableSeed);

                if (Path.GetExtension(m_Archive) == ".dat")
                {
                    lpEntryTable = iRebuildTable(lpEntryTable);
                }

                using (var TEntryReader = new MemoryStream(lpEntryTable))
                {
                    m_Header.bUnknown1 = TEntryReader.ReadByte(); // 0x1, 0x3
                    if (m_Header.bUnknown1 == 3)
                    {
                        m_Header.bUnknown2 = TEntryReader.ReadByte(); // 0x9
                    }

                    Int16 sHashLength = TEntryReader.ReadInt16(true);
                    m_Header.m_MD5Hash = TEntryReader.ReadString(sHashLength);
                    m_Header.dwTimeStamp = TEntryReader.ReadInt64(true);
                    m_Header.bUnknown3 = TEntryReader.ReadByte(); // 0x1, 0x3

                    if (m_Header.bUnknown3 == 3)
                    {
                        Int16 sCompressionLength = TEntryReader.ReadInt16(true);
                        m_Header.m_Compression = TEntryReader.ReadString(sCompressionLength); // deflate=-1,
                    }
                    m_Header.dwTotalFiles = TEntryReader.ReadInt32(true);

                    m_EntryTable.Clear();
                    for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                    {
                        var m_Entry = new KarEntry();

                        Int16 sFirstPathLength = TEntryReader.ReadInt16(true);
                        m_Entry.m_FirstPath = TEntryReader.ReadString(sFirstPathLength, Encoding.UTF8);
                        Int16 sSecondPathLength = TEntryReader.ReadInt16(true);
                        m_Entry.m_SecondPath = TEntryReader.ReadString(sFirstPathLength, Encoding.UTF8);
                        m_Entry.dwSize = TEntryReader.ReadInt32(true);
                        m_Entry.dwOffset = TEntryReader.ReadInt64(true);
                        m_Entry.m_FilePassword = String.Format("{0}{1}{2}", m_Header.m_Password, m_Entry.m_FirstPath, m_Header.dwTimeStamp.ToString());
                        m_Entry.dwSeed = KarHash.iGetHash(m_Entry.m_FilePassword);

                        if (Path.GetFileName(m_Entry.m_FirstPath) == "")
                        {
                            continue;
                        }

                        m_EntryTable.Add(m_Entry);
                    }

                    TEntryReader.Dispose();
                }

                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FileName = m_Entry.m_FirstPath.Replace("/", @"\");
                    String m_FullPath = m_DstFolder + m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    TKarStream.Seek(m_Entry.dwOffset, SeekOrigin.Begin);

                    var lpBuffer = TKarStream.ReadBytes(m_Entry.dwSize);
                    lpBuffer = KarCipher.iDecrypt(lpBuffer, m_Entry.dwSeed);

                    if (m_Header.bUnknown3 == 1 || m_Header.bUnknown3 == 3)
                    {
                        var lpResult = GZip.iDecompress(lpBuffer);
                        File.WriteAllBytes(m_FullPath, lpResult);
                    }
                    else
                    {
                        File.WriteAllBytes(m_FullPath, lpBuffer);
                    }
                }

                TKarStream.Dispose();
            }
        }

        private static Byte[] iRebuildTable(Byte[] lpBuffer)
        {
            Int32 dwOffset = 0;
            Int32 dwBlocks = lpBuffer.Length / 1024 + 1;
            Byte[] lpResult = new Byte[lpBuffer.Length];

            using (var TMemoryStream = new MemoryStream(lpBuffer) { Position = 2 })
            {
                Int16 bUnknown1 = TMemoryStream.ReadInt16(true); // 0x5

                for (Int32 i = 0; i < dwBlocks; i++)
                {
                    Int32 bUnknown2 = TMemoryStream.ReadByte(); // 0x7A
                    Int32 dwBlockSize = TMemoryStream.ReadInt32(true);

                    var lpBlock = TMemoryStream.ReadBytes(dwBlockSize);

                    Array.Copy(lpBlock, 0, lpResult, dwOffset, lpBlock.Length);
                    dwOffset += lpBlock.Length;
                }
            }

            return lpResult;
        }
    }
}
