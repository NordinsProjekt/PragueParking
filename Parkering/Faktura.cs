using System;
using System.Collections.Generic;
using System.Text;

namespace Parkering
{
    class Faktura
    {
        private string regNr;
        private DateTime starttid;
        private DateTime sluttid;
        private decimal summa;
        public Faktura(string regNr, DateTime starttid, DateTime sluttid, Decimal summa)
        {
            this.regNr = regNr;
            this.starttid = starttid;
            this.sluttid = sluttid;
            this.summa = summa;
        }
        public string ToSQL()
        {
            string fd = "yyyy-MM-dd HH:mm:ss.fff";
            return string.Format("'{0}','{1}','{2}',{3}",regNr,starttid.ToString(fd),sluttid.ToString(fd),summa);
        }
    }
}
