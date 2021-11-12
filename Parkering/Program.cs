using System;

namespace Parkering
{
    class Program
    {
        static Parkering parkingArea = new Parkering(20,"Pragborgen");
        static void Main(string[] args)
        {
            ////Inställningar i konsollen.
            Console.WindowWidth = 90;
            Console.WindowHeight = 30;
            Console.Title = "Prag Parking C# Tenta - Markus Nordin";
            //TestData();
            parkingArea.LoadFromDB();
            ParkeringMeny();
        }
        static void TestData()
        {
            //3 testbilar och 3 testmc
            Fordon[] testFordon = new Fordon[6];
            testFordon[0] = new Fordon("ABC123", FordonTyp.Bil);
            testFordon[1] = new Fordon("DEF456", FordonTyp.Bil);
            testFordon[2] = new Fordon("GHI789", FordonTyp.Bil);
            testFordon[3] = new Fordon("QWE147", FordonTyp.MC);
            testFordon[4] = new Fordon("ASD258", FordonTyp.MC);
            testFordon[5] = new Fordon("ZXC369", FordonTyp.MC);
            for (int i = 0; i < testFordon.Length; i++)
            {
                parkingArea.Parkera(testFordon[i]);
            }
        }
        static void ParkeringMeny()
        {
            //Meny som bara accepterar 1-5 i string
            string meddelande = "";
            bool loopMeny = true;
            while(loopMeny)
            {
                string val = HuvudMeny(meddelande);
                switch(val)
                {
                    case "1":
                        //Typ av fordon samt regnr.
                        //Skapa ett tempfordon och skicka det till parkera.
                        meddelande = ParkeraFordonMeny();
                        break;
                    case "2":
                        meddelande = FlyttaFordonMeny();
                        break;
                    case "3":
                        meddelande = HittaFordonMeny();
                        break;
                    case "4":
                        meddelande = AvslutaParkeringMeny();
                        break;
                    case "5":
                        VisaParkeringLista("[Visa fordon]");               
                        break;
                    case "6":
                        loopMeny = false;
                        //Sparar ändringarna
                        parkingArea.SparaTillDatabas();
                        Console.SetCursorPosition(0, 15);
                        break;
                    default:
                        meddelande = "Använd siffrorna 1-6 eller piltangenterna bekräfta med ENTER.";
                        break;
                }
            }
        }

        static string HuvudMeny(string meddelande)
        {
            string titel = "[Parkering " + parkingArea.NamnParkering + "]";
            Console.Clear();
            Meny.Window(1);
            Meny.DefaultConsoleSettings();
            parkingArea.ParkeringInfo();
            Meny.Draw(Meny.MainMenu(titel,meddelande),true);
            return Console.ReadLine();
        }
        static string ParkeraFordonMeny()
        {
            Console.Clear();
            string meddelande = "";
            Meny.Window(1);
            Meny.DefaultConsoleSettings();
            parkingArea.ParkeringInfo();
            Meny.Draw(Meny.ParkeringMenu("[Parkera fordon]"),true);

            string typ = Console.ReadLine();
            if (typ == "1" || typ == "2")
            {
                string regNr = RegNrMeny("[Parkera fordon]");
                if (regNr.Length > 5)                      
                {
                    int platsID = parkingArea.HittaFordonIndex(regNr);
                    if (platsID < 0)
                    {
                        FordonTyp t = new FordonTyp();
                        if (typ.Equals("1"))
                            t = FordonTyp.Bil;
                        if (typ.Equals("2"))
                            t = FordonTyp.MC;
                        Fordon temp = new Fordon(regNr, t);
                        if (parkingArea.Parkera(temp))
                            meddelande = "Fordon: " + temp.Typ + " [" + temp.RegNr + "] lades till";
                        else
                            meddelande = "Parkeringen är full";
                    }
                    else
                        meddelande = "Registreringsnummret är redan registrerat";
                }
                else
                    meddelande = "Registreringsnummret är för kort.";
            }
            else
                meddelande = "Du kan bara använda siffrorna 1 eller 2.";
            return meddelande;
        }
        static string FlyttaFordonMeny()
        {
            string regNr = RegNrMeny("[Flytta fordon]");
            if (regNr.Length > 5)
            {
                int platsID = parkingArea.HittaFordonIndex(regNr);
                if (platsID >= 0)
                {
                    //Användaren skriver in plats nummer, systemet kommer använda -1 för att få indexen.
                    Console.Clear();
                    Meny.Window(1);
                    Meny.DefaultConsoleSettings();
                    parkingArea.ParkeringInfo();
                    Meny.Draw(Meny.FlyttaFordon("[Flytta fordon]"),true);

                    int nyPlatsID;
                    if (int.TryParse(Console.ReadLine(), out nyPlatsID) && (nyPlatsID - 1) >= 0 && nyPlatsID <= parkingArea.MaxIndex())
                    {
                            if (parkingArea.FlyttaFordon(regNr, platsID, nyPlatsID - 1))
                                return "Fordonet flyttades från plats " + (platsID + 1) + " till " + (nyPlatsID);
                            else
                                return "Platsen är inte ledig";
                    }
                    else
                        return "Fel inmatning, accepterar bara heltal";
                }
                else
                    return "Registreringsnummret finns inte.";
            }
            else
                return "Registreringsnummret är för kort.";
        }
        static string HittaFordonMeny()
        {
            string regNr = RegNrMeny("[Hitta fordon]");
            if (regNr.Length > 5)
            {
                Fordon temp = parkingArea.HittaFordon(regNr);
                if (temp.Typ.Equals(FordonTyp.None))
                    return "Hittade inte fordonet";
                else
                {
                    int platsID = parkingArea.HittaFordonIndex(regNr);
                    return "Hittade " + temp.Typ + " ["+temp.RegNr+"] på plats " + (platsID+1);
                }
            }
            else
                return "Registreringsnummret är för kort.";
        }
        static string AvslutaParkeringMeny()
        {
            string meddelande;
            string regNr = RegNrMeny("[Avsluta parkering]");
            if (regNr.Length > 5)
               meddelande = parkingArea.AvslutaParkering(regNr);
            else
                return "Registreringsnummret är för kort.";
            return meddelande;
        }
        static void VisaParkeringLista(string titel)
        {
            Console.Clear();
            Meny.Window(2);
            Meny.DefaultConsoleSettings();
            string fordonLista = titel + '\r' +parkingArea.ToString()+'\r' +"Tryck på en tangent för att fortsätta";
            Meny.Draw(fordonLista.Replace("\n", "").Split('\r'),false);
            Console.ReadKey();
        }
        static string RegNrMeny(string titel)
        {
            Console.Clear();
            Meny.Window(1);
            Meny.DefaultConsoleSettings();
            parkingArea.ParkeringInfo();
            Meny.Draw(Meny.RegNrMeny(titel),true);
            string regNr = Console.ReadLine();
            //Skrubbar stringen från dåliga inmatningar.
            regNr = regNr.ToUpper().Replace(" ","").Replace("\t", "");
            return regNr;
        }
    }
}
