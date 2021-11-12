using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Parkering
{
    public enum PrisTimme
    {
        Gratis = 0,
        Bil = 40,
        MC = 25
    }

    class ParkeringRuta
    {
        public Fordon[] fordon = new Fordon[2];

        public ParkeringRuta()
        {
            //Varje ruta kan hålla max två fordontyp MC
            fordon[0] = new Fordon();
            fordon[1] = new Fordon();
        }
        public bool ParkeraFordon(Fordon f)
        {
            //är det MC så kolla plats 1 eller 2.
            //Viktigt för ankomsttiden.
            if (f.Typ.Equals(FordonTyp.Bil))
            {
                f.Ankomsttid = DateTime.Now;
                fordon[0] = f;
                return true;
            }
            if (f.Typ.Equals(FordonTyp.MC))
            {
                for (int j = 0; j < fordon.Length; j++)
                {
                    if(fordon[j].Typ.Equals(FordonTyp.None))
                    {
                        f.Ankomsttid = DateTime.Now;
                        fordon[j] = f;
                        return true;
                    }
                }
            }
            return false;
        }
        public int Ledig()
        {
            //Returnerar 2 om platsen är full
            //Returnerar 1 om platsen har en MC
            //Returnerar 0 om platsen är helt ledig
            int sum = 0;
            for (int i = 0; i < fordon.Length; i++)
            {
                switch(fordon[i].Typ)
                {
                    case FordonTyp.Bil:
                        sum += 2;
                        break;
                    case FordonTyp.MC:
                        sum += 1;
                        break;
                    default:
                        break;
                }
            }
            return sum;
        }
        public bool Ledig(FordonTyp typ)
        {
            //Kollar om platserna är lediga baserat på Fordonstypen.
            
            if (typ.Equals(FordonTyp.Bil))
            {
                if (fordon[0].Typ.Equals(FordonTyp.None) && fordon[1].Typ.Equals(FordonTyp.None))
                    return true;
            }
            if (typ.Equals(FordonTyp.MC))
            {
                if (fordon[0].Typ.Equals(FordonTyp.Bil) || fordon[1].Typ.Equals(FordonTyp.Bil))
                    return false;
                if (fordon[0].Typ.Equals(FordonTyp.None) || fordon[1].Typ.Equals(FordonTyp.None))
                    return true;
            }
            return false;
        }

        public Fordon GetFordon(string regNr)
        {
            //Returnerar fordonobjektet som har ett visst regNr.
            Fordon temp = new Fordon();
            for (int i = 0; i < fordon.Length; i++)
            {
                if (fordon[i].RegNr.Equals(regNr))
                {
                    temp = fordon[i];
                    break;
                }
            }
            return temp;
        }
        public bool Insert(Fordon f)
        {
            //Används vid FlyttaBil.
            if (f.Typ.Equals(FordonTyp.Bil))
            {
                fordon[0] = f;
                return true;
            }
            if(f.Typ.Equals(FordonTyp.MC))
                for (int i = 0; i < fordon.Length; i++)
                {
                    if (fordon[i].Typ.Equals(FordonTyp.None))
                    {
                        fordon[i] = f;
                        return true;
                    }
                }
            return false;
        }
        public bool TaBort(Fordon f)
        {
            for (int i = 0; i < fordon.Length; i++)
            {
                if (fordon[i].Equals(f))
                {
                    fordon[i] = new Fordon();
                    return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (fordon[0].Typ.Equals(FordonTyp.Bil))
            {
                sb.Append(fordon[0].Typ + ": " + fordon[0].RegNr + "[" + fordon[0].Ankomsttid + "]");
                return sb.ToString();
            }
            if (fordon[0].Typ.Equals(FordonTyp.None))
                sb.Append("Ledig | ");
            else
                sb.Append(fordon[0].Typ + ": " + fordon[0].RegNr + "[" + fordon[0].Ankomsttid + "] | ");

            if (fordon[1].Typ.Equals(FordonTyp.None))
                sb.Append("Ledig");
            else
                sb.Append(fordon[1].Typ + ": " + fordon[1].RegNr + "[" + fordon[1].Ankomsttid + "]");
            return sb.ToString(); 
        }
    }

    class Parkering
    {
        private ParkeringRuta[] ruta;
        private string namnParkering;
        public Parkering(int antalPlatser,string namnParkering)
        {
            ruta = new ParkeringRuta[antalPlatser];
            NamnParkering = namnParkering;
            for (int i = 0; i < antalPlatser; i++)
            {
                ruta[i] = new ParkeringRuta();
            }
        }
        public string NamnParkering
        {
            get { return namnParkering; }
            set { namnParkering = value; }
        }

        public int HittaFordonIndex(string regNr)
        {
            for (int i = 0; i < ruta.Length; i++)
                for (int j = 0; j < 2; j++)
                    if (ruta[i].fordon[j].RegNr.Equals(regNr))
                        return i;
            return -1;
        }
        public Fordon HittaFordon(string regNr)
        {
            Fordon temp = new Fordon();
            for (int i = 0; i < ruta.Length; i++)
                for (int j = 0; j < 2; j++)
                {
                    temp = ruta[i].fordon[j];
                    if (temp.RegNr.Equals(regNr))
                        return temp;
                }
            return temp;
        }

        public bool Parkera(Fordon fordon)
        {
            int platsIndex = HittaLedigPlats(fordon.Typ);
            if (platsIndex>=0)
            {
                if (ruta[platsIndex].ParkeraFordon(fordon))
                {
                    Optimera();
                    return true;
                }
            }
            return false;
        }
        public void Optimera()
        {
            //Parkering optimering, flyttar ihop MC
            int pIndex = -1;
            Fordon temp = new Fordon();
            for (int i = 0; i < ruta.Length; i++)
            {
                if (ruta[i].Ledig().Equals(1) && pIndex>-1)
                {
                    FlyttaFordon(temp.RegNr, pIndex, i);
                    pIndex = -1;
                }
                if (ruta[i].Ledig().Equals(1) && pIndex.Equals(-1))
                {
                    pIndex = i;
                    for (int j = 0; j < ruta[i].fordon.Length; j++)
                    {
                        if (ruta[i].fordon[j].Typ.Equals(FordonTyp.MC))
                        {
                            temp = ruta[i].fordon[j];
                            break;
                        }
                    }
                }
            }
        }
        public int MaxIndex()
        {
            return ruta.Length;
        }
        public void LoadFromDB()
        {
            //Behövs det kontroll om databas + tabell existerar?
            DatabasHantering.Database = "Parking";
            DatabasHantering.Tabell = "pplats";
            string[] sqlResultat = DatabasHantering.GetCSVFromQuery(string.Format(
                "SELECT plats,regnr,fordontyp,starttid FROM {0}", DatabasHantering.DB()));     
            for (int i = 1; i < sqlResultat.Length-1; i++)
            {
                string[] text = sqlResultat[i].Split(',');
                Fordon f = Fordon.ByggFordon(text[1].Trim(),text[2].Trim(),text[3].Trim());
                ruta[int.Parse(text[0])].Insert(f);
            }
        }
        public void SparaFakturaTillDatabas(Faktura f)
        {
            //När en parkering avslutas så spara all information.
            DatabasHantering.Database = "Parking";
            DatabasHantering.Tabell = "fakturering";
            string sql = string.Format("INSERT INTO {0} (regnr,starttid,sluttid,summa) VALUES " +
                "({1});", DatabasHantering.DB(), f.ToSQL());
            DatabasHantering.SendSqlQuery(sql);
        }
        public void SparaTillDatabas()
        {
            //Sparar ner hela parkeringen till databasen.
            StringBuilder sqlSB = new StringBuilder();
            DatabasHantering.Database = "Parking";
            DatabasHantering.Tabell = "pplats";
            Fordon f = new Fordon();
            DateTime dt = new DateTime();
            //Backup om något går fel.
            string[] sqlResultat = DatabasHantering.GetCSVFromQuery(string.Format(
                "SELECT plats,regnr,fordontyp,starttid FROM {0}", DatabasHantering.DB()));
            Filhantering.SkapaCSVFil("backup_" + NamnParkering + ".csv", sqlResultat);
            //Tömmer tabellen på info för att sedan dumpa ner hela parkeringen igen.
            DatabasHantering.SendSqlQuery(String.Format("DELETE FROM {0}",DatabasHantering.DB()));
            for (int i = 0; i < ruta.Length; i++)
            {
                for (int j = 0; j < ruta[i].fordon.Length; j++)
                {
                    f = ruta[i].fordon[j];
                    if (f.Typ.Equals(FordonTyp.None))
                        continue;
                    else
                    {
                        dt = f.Ankomsttid;
                        sqlSB.Append(String.Format("INSERT INTO {0} (plats,regnr,fordontyp,starttid) VALUES ", DatabasHantering.DB()));
                        sqlSB.Append(String.Format("({0},{1}); ",i, f.ToSQL()));
                    }
                }
            }
            DatabasHantering.SendSqlQuery(sqlSB.ToString());
        }
        public string AvslutaParkering(string regNr)
        {
            //Anropa HittaFordon.
            //MC kostar 25kr och bil 40kr per påbörjad timme.
            //Informationen är sparad i PrisTimme enum
            int kostnad = 0;
            int platsID = HittaFordonIndex(regNr);
            TimeSpan diff = new TimeSpan();
            Fordon temp = new Fordon();
            int pris = 0;
            if (platsID >= 0)
            {
                for (int i = 0; i < ruta[i].fordon.Length; i++)
                {
                    temp = ruta[platsID].fordon[i];
                    if (temp.RegNr.Equals(regNr))
                    {
                        diff = DateTime.Now - temp.Ankomsttid;
                        pris = PrisFordonTyp(temp.Typ);
                        kostnad = ((int)diff.Hours+1) * pris;
                        ruta[platsID].TaBort(temp);
                        if (temp.Typ.Equals(FordonTyp.MC))
                            Optimera();
                        string logg = "[" + DateTime.Now + "] Avslutad parkering för " + temp.Typ + " " 
                            + temp.RegNr + " Ankomsttid: "+temp.Ankomsttid+" Kostnad: " + kostnad + "kr";
                        Filhantering.SparaTillLogg(NamnParkering, logg);
                        //Skapar ett faktura objekt som skickas till databasen.
                        Faktura f = new Faktura(temp.RegNr, temp.Ankomsttid, DateTime.Now, kostnad);
                        SparaFakturaTillDatabas(f);
                        return "Kostnaden blir: " + kostnad + "kr";
                    }
                }
            }
            return "Bilen finns inte";
        }
        private int PrisFordonTyp(FordonTyp typ)
        {
            //Pris per timme baserat på fordonstyp.
            int pris;
            switch (typ)
            {
                case FordonTyp.Bil:
                    pris = (int)PrisTimme.Bil;
                    break;
                case FordonTyp.MC:
                    pris = (int)PrisTimme.MC;
                    break;
                default:
                    pris = (int)PrisTimme.Gratis;
                    break;
            }
            return pris;
        }
        public bool FlyttaFordon(string regNr,int gammalPlats,int nyPlats)
        {
            Fordon f = ruta[gammalPlats].GetFordon(regNr);
            if (f.Typ.Equals(FordonTyp.None))
                return false;
            else
            {
                if (ruta[nyPlats].Ledig(f.Typ))
                {
                    if (ruta[nyPlats].Insert(f))
                        if (ruta[gammalPlats].TaBort(f))
                            return true;
                        else
                            return false;
                    else
                        return false;
                }
                else 
                    return false;
            }
        }
        public int AntalLedigaPlatser(FordonTyp typ)
        {
            //Räknar hur många bilplatser som är lediga
            //eller hur många MC som är lediga.
            //Ledig = 1 Betyder att en MC finns, plats för en till mao
            //Ledig = 0 Betyder att rutan är tom.
            int antal = 0;
            for (int i = 0; i < ruta.Length; i++)
            {
                switch(typ)
                {
                    case FordonTyp.Bil:
                        if (ruta[i].Ledig().Equals(0))
                            antal++;
                        break;
                    case FordonTyp.MC:
                        if (ruta[i].Ledig().Equals(1))
                            antal++;
                        if (ruta[i].Ledig().Equals(0))
                            antal += 2;
                        break;
                }
            }
            return antal;
        }
        private int HittaLedigPlats(FordonTyp typ)
        {
            for (int i = 0; i < ruta.Length; i++)
            {
                switch (typ)
                {
                    case FordonTyp.Bil:
                        if (ruta[i].Ledig().Equals(0))
                            return i;
                        break;
                    case FordonTyp.MC:
                        if (ruta[i].Ledig() <2)
                            return i;
                        break;
                }
            }
            return -1;
        }
        public override string ToString()
        {
            //Skriver ut en logg på vad som står på alla platser.
            //Skriver ut hela parkeringen?
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ruta.Length; i++)
            {
                sb.AppendLine("Plats (" + (i + 1) + "): " + ruta[i].ToString());
            }
            return sb.ToString();
        }
        public void ParkeringInfo()
        {
            //Visa hela platsen med grafik i konsollen?
            int row = 5;
            int col = ruta.Length / row;
            int counter = 0;
            int n = 2;
            int tal;
            int right = 45;
            Console.SetCursorPosition(right, n++);
            Console.Write("Bilplatser kvar: [" + AntalLedigaPlatser(FordonTyp.Bil) + "] ");
            Console.SetCursorPosition(right, n++);
            Console.Write("MC platser kvar: [" + AntalLedigaPlatser(FordonTyp.MC) + "] ");
            Console.CursorVisible = false;
            for (int i = 0; i < row; i++)
            {
                Console.SetCursorPosition(right, n);
                for (int j = 0; j < col; j++)
                {
                    tal = ruta[counter++].Ledig();
                    switch (tal)
                    {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        default:
                            break;
                    }
                    Console.Write("[{0,2}]", counter);
                }
                Console.Write("\n");
                n++;
            }
            DefaultConsoleSettings();
        }
        public void DefaultConsoleSettings()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
