using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public  class SalesAnalysisDetail
    {
        public string dteTarih { get; set; }
        public string sMusteriAdi { get; set; }
        public string sKodu { get; set; }
        public string sStokAciklama { get; set; }
        public string sRenk { get; set; }
        public string sBeden { get; set; }
        public double Miktar { get; set; }
        public double Fiyat { get; set; }
        public double MALIYET { get; set; }
        public double Iskonto { get; set; }
        public double lNetTutar { get; set; }
        public string sFiyatTipi{ get; set; }
        public string satici{ get; set; }
        public string sMagaza{ get; set; }
    }
}
