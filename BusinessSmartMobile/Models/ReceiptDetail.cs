using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public  class ReceiptDetail
    {
        public string lFisNo { get; set; }
        public string lSiparisNo { get; set; }
        public string sKodu { get; set; }
        public string sAciklama { get; set; }
        public double lMiktari { get; set; }
        public double lFiyati { get; set; }
        public double lTutari { get; set; }
        public double lIskontosuzTutari { get; set; }
        public double nIskontoYuzdesi { get; set; }
    }
}
