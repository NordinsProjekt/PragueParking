using System;
using System.Collections.Generic;
using System.Text;

namespace Parkering
{
    //Klassen som skall hantera hela menysystemet.
    //Snyggare meny.
    class Meny
    {

        public static string[] MainMenu(string titel,string meddelande)
        {
            string[] text = { 
                titel.ToString() ,
                "(1) Parkera",
                "(2) Flytta fordon",
                "(3) Hitta fordon",
                "(4) Avsluta parkering",
                "(5) Lista parkeringsplatserna",
                "(6) Avsluta programmet",
                meddelande.ToString(),
                "?: "};
            return text;
        }
        public static string[] ParkeringMenu(string titel)
        {
            string[] text = {
                titel.ToString(),
                "Vad ska du parkera?",
                "(1) Bil",
                "(2) MC",
                "?: "
                };
            return text;
        }
        public static string[] RegNrMeny(string titel)
        {
            string[] text = {
                titel.ToString(),
                "Skriv in Registreringsnummer",
                "ex (ABC123",
                "?: "
                };
            return text;
        }
        public static string[] FlyttaFordon(string titel)
        {
            string[] text = {
                titel.ToString(),
                "Skriv in önskad plats",
                "ex (7)",
                "?: "
                };
            return text;
        }
        public static void AsciiCar()
        {
            //Coola loggan.
            string[] text =
            {
            @"    ______ ",  
            @"   /|_||_\`.__",
            @"  (   _    _ _\",
            @"  =`-(_)--(_)-'"
            };
            for (int i = 0; i < text.Length; i++)
            {
                Console.SetCursorPosition(62, 9 + i);
                Console.Write(text[i]);
            }
        }
        public static void Draw(string[] text,bool car)
        {
            //Ritar upp all text som skickas till den.
            //Enligt en specifik standard.
            if (car)
                AsciiCar();
            for (int i = 0; i < text.Length; i++)
            {
                Console.SetCursorPosition(5, 2 + i);
                Console.Write(text[i]);
            }
           
        }
        public static void Window(int alternativ)
        {
            int width = 0;
            int height = 0;
            int border = 2;
            switch(alternativ)
            {
                case 1:
                    //Ritar en grå ruta runt hela menyn.
                    width = 80;
                    height = 13;
                    break;
                case 2:
                    //Ritar en större grå ruta runt hela menyn.
                    width = 90;
                    height = 25;
                    break;
            }
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.Write(" ");
                Console.SetCursorPosition(i, height);
                Console.Write(" ");
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < border; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write(" ");
                    Console.SetCursorPosition(j +(width-border), i);
                    Console.Write(" ");
                }
            }
            //Skriver ut Versions nummer till skärmen.
            if (alternativ == 1)
            {
                Console.SetCursorPosition(width-15, height);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("Version 3.0");
            }
            else
            {
                Console.SetCursorPosition(width-15, height);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("Version 3.0");
            }
            DefaultConsoleSettings();

        }
        public static void DefaultConsoleSettings()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

}
