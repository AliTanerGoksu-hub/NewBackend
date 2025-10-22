namespace BarkodBackend.Models
{
    public class TbFirma
    {
        public string nFirmaId { get; set; }
        public string sKodu { get; set; }
        public string sAciklama { get; set; }
        public string sVergiDairesi { get; set; }
        public string sVergiNo { get; set; }
        public int nHesapId { get; set; }
        public double lBakiye { get; set; }
        public string sAdres1 { get; set; }
        public string sAdres2 { get; set; }
        public string sFiyatTipi { get; set; }
        public string sSemt { get; set; }
        public string sIl { get; set; }
        public string sUlke { get; set; }
        public string sPostaKodu { get; set; }
        public string sSaticiRumuzu { get; set; }
        public double RiskBakiye { get; set; }
        public double SenetRisk { get; set; }
        public double CekRisk { get; set; }
        public double ToplamRisk { get; set; }
        public double lKalanKredi { get; set; }
    }
}
