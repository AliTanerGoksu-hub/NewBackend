using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class UrunSecimi
    {
        public Stock Urun { get; set; } = new();
        public double Miktar { get; set; }
        public string SelectedBirimCinsi { get; set; }
        public List<TbStokBirimCinsi> UnitList { get; set; } = new(); 

    }
}
