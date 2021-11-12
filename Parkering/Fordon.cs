using System;
using System.Collections.Generic;
using System.Text;

namespace Parkering
{
    public enum FordonTyp
    {
        None,
        Bil,
        MC
    }
    class Fordon
    {
        private string regNr;
        private FordonTyp typ;
        private DateTime ankomsttid;

        public Fordon(string regNr,FordonTyp typ)
        {
            RegNr = regNr;
            Typ = typ;
            ankomsttid = new DateTime();
        }
        public Fordon()
        {
            RegNr = "";
            Typ = FordonTyp.None;
            ankomsttid = new DateTime();
            //Används av parkeringplatsen för att "reservera" ett framtida fordon.
        }
        public string RegNr
        {
            get { return regNr; }
            private set //skrubbar stringen från dåliga inmatningar.
            { regNr = value.ToUpper().Replace(" ", "").Replace("\t", ""); ; }
        }
        public FordonTyp Typ
        {
            get { return typ; }
            private set { typ = value; }
        }
        public DateTime Ankomsttid
        {
            set { ankomsttid = value; }
            get { return ankomsttid; }
        }
        public static Fordon ByggFordon(string regNr,string fordonTyp, string ankomsttid)
        {
            Fordon f = new Fordon();
            f.RegNr = regNr;
            switch (fordonTyp)
            {
                case "Bil":
                    f.Typ = FordonTyp.Bil;
                    break;
                case "MC":
                    f.Typ = FordonTyp.MC;
                    break;
                default:
                    f.Typ = FordonTyp.None;
                    break;
            }
            f.Ankomsttid = DateTime.Parse(ankomsttid);
            return f;
        }
        public string ToSQL()
        {
            string fd = "yyyy-MM-dd HH:mm:ss.fff";
            return string.Format("'{0}','{1}','{2}'", RegNr, Typ, Ankomsttid.ToString(fd));
        }
        public static string ToSQL(Fordon f)
        {
            string fd = "yyyy-MM-dd HH:mm:ss.fff";
            return string.Format("'{0}','{1}','{2}'",f.RegNr,f.Typ,f.Ankomsttid.ToString(fd));
        }

    }
}
