using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Parkering
{
    class Filhantering
    {
        public static void SparaTillLogg(string path,string text)
        {
            //Sparar en avslutad parkering.
            try
            {
                using StreamWriter file = new StreamWriter(path + ".txt", append: true);
                file.WriteLineAsync(text);
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, 15);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public static void SkapaCSVFil(string pathFull, string[] text)
        {
            //Backup av parkeringshuset innan DELETE FROM körs.
            using StreamWriter sw = new StreamWriter(pathFull, false);
            {
                for (int i = 0; i < text.Length; i++)
                {
                    sw.WriteLine(text[i]);
                }
                sw.Dispose();
            }
        }
    }
}
