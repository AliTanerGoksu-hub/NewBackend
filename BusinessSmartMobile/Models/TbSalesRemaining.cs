using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class TbSalesRemaining
    {
        public string sKodu { get; set; }
        public string sStokAciklama { get; set; }
        public double miktar { get; set; }
        public double envanter { get; set; }
        public double mevcut { get; set; }
        public double Bekleyen { get; set; }
        public double NetMevcut { get; set; }
        public double siparis { get; set; }
        public string sonAlisTarihi { get; set; }
        public double sonAlisMiktari { get; set; }
        public string satici { get; set; }
    }
}
