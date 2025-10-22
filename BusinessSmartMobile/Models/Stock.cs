namespace BusinessSmartMobile.Models
{
    public class Stock
    {
        public string nStokID { get; set; }
        public string sAciklama { get; set; }
        public string sKodu { get; set; }
        public string sOzelNot { get; set; }
        public string sKisaAdi { get; set; }

        public string sFiyatTipi { get; set; }
        public int Miktar { get; set; } = 1;

        public double lMevcut { get; set; }
        public double lMevcut2 { get; set; }

        public string sBirimCinsi1 { get; set; }
        public string sBirimCinsi2 { get; set; }
        public double nBirimCarpan { get; set; }

        public string sBeden { get; set; }
        public string sRenkAdi { get; set; }

        public double lFiyat1 { get; set; }
        public double nIskontoYuzdesi { get; set; }
        public double nKdvOrani { get; set; }

        public string sBarkod { get; set; }
        public string sDepo { get; set; }
        public bool getAll { get; set; } = false;
        public string opt { get; set; } = ">";

        // 🔹 Sadece R2 URL
        public string ImageUrl { get; set; }
        public bool HasResim { get; set; }
    }
}
