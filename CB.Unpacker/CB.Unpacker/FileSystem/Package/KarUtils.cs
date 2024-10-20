using System;
using System.IO;

namespace CB.Unpacker
{
    class KarUtils
    {
        private static String iGetDefaultPassword()
        {
            return "Jpy.beJdfC";
        }

        public static String iGetPassword(String m_File)
        {
            String m_Result = Path.GetFileName(m_File);
            switch (m_Result)
            {
                case "oni.dat":
                case "launch.dat": return "dhfuhsudfh98vhdsovnfdhiouer8u8hgjbkjciudsuifsjdiosajfn";

                // Default password
                default: return iGetDefaultPassword();
            }
        }
    }
}
