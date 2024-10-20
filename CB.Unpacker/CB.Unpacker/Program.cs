using System;

namespace CB.Unpacker
{
    class Program
    {
        private static String m_Title = "CosmicBreak KAR Unpacker";

        static void Main(String[] args)
        {
            Console.Title = m_Title;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(m_Title);
            Console.WriteLine("(c) 2024 Ekey (h4x0r) / v{0}\n", Utils.iGetApplicationVersion());
            Console.ResetColor();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Usage]");
                Console.WriteLine("    CB.Unpacker <m_KarFile> <m_Directory>");
                Console.WriteLine("    m_KarFile - Source of KAR file");
                Console.WriteLine("    m_Directory - Destination directory\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Examples]");
                Console.WriteLine("    CB.Unpacker E:\\Games\\CosmicBreak\\resources\\image.kar D:\\Unpacked");
                Console.ResetColor();
                return;
            }

            String m_Input = args[0];
            String m_Output = Utils.iCheckArgumentsPath(args[1]);

            KarUnpack.iDoIt(m_Input, m_Output);
        }
    }
}
