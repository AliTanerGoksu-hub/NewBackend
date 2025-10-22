namespace BarkodBackend.Models
{
    public class TbDeliveryReport
    {
        public string dteTeslimTarihi{ get; set; }
        public string dteSiparisTarihi{ get; set; }
        public string  sFirmaAciklama{ get; set; }
        public string  sSiparisiAlan{ get; set; }
        public string  sKodu{ get; set; }
        public string  sAciklama{ get; set; }
        public double  lMiktari{ get; set; }
        public double nBirimCarpan { get; set; }
        public double  lMevcut{ get; set; }
        public double  lSevkiyat{ get; set; }
        public double  lBekleyen{ get; set; }
        public double  lKalan{ get; set; }
        public string  sAciklama2{ get; set; }
        public string sAciklama3 { get; set; }

    }
}
