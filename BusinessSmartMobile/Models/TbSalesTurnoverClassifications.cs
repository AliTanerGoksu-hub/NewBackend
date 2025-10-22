using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class TbSalesTurnoverClassifications
    {
        public string sAciklama { get; set; }
        public double miktar { get; set; }
        public double iskonto { get; set; }
        public double tutar { get; set; }
        public double maliyet { get; set; }
        public double yuzde { get; set; }
        public int MUISTERISAYISI { get; set; }
        public int FISSAYISI { get; set; }
        public double FISORTALAMA { get; set; }
        public double kar { get; set; }
    }
}
