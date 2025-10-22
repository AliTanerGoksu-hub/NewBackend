using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class TbSalesAnalysis
    {
        public string dteTarih { get; set; }
        public string fisTipi { get; set; }
        public string sMusteriAdi { get; set; }
        public string sKodu { get; set; }
        public string sStokAciklama { get; set; }
        public string sRenkAdi { get; set; }
        public string sBeden { get; set; }
        public double miktar { get; set; }
        public double fiyat { get; set; }
        public double lNetTutar { get; set; }
        public string satici { get; set; }
        public string lNo { get; set; }
        public string nAlisVerisID { get; set; }
        public string nFirmaID { get; set; }
    }
}
