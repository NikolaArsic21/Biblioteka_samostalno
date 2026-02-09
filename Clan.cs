using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka_samostalno
{
    class Clan
    {
        public int IznajmljivanjeID { get; set; }
        public int BrojClanske { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string NaslovKnjige { get; set; }
        public int PeriodIznajmljivanja { get; set; }

        public Clan()
        {
            Ime = string.Empty;
            Prezime = string.Empty;
            NaslovKnjige = string.Empty;
        }
    }
}
