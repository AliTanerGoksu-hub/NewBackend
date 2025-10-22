using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class TbSalesTurnover
    {
        public string periyod { get; set; }
        public double tutar { get; set; }
        public double iskonto { get; set; }
        public double toplamKdv { get; set; }
        public double miktar { get; set; }
        public double netCiro { get; set; }
        public double araToplam { get; set; }
        public int musteriSayisi { get; set; }
    }
}
