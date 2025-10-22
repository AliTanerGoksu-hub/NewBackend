namespace BarkodBackend.Models
{
    public class TbStok
    {
        public string nStokID { get; set; }
        public string sAciklama { get; set; }
        public string sKodu { get; set; }
        public string sOzelNot { get; set; }
        public string sKisaAdi { get; set; }
        public string sFiyatTipi { get; set; }
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
        public string pResim1 { get; set; }
        public string pResim2 { get; set; }
        public string pResim3 { get; set; }
        public string pResim4 { get; set; }
        public string pResim5 { get; set; }
        public string pResim6 { get; set; }
        public string yol1 { get; set; }
        public string yol2 { get; set; }
        public string yol3 { get; set; }
        public string yol4 { get; set; }
        public string yol5 { get; set; }
        public string yol6 { get; set; }
        public byte[] ResimBytes1 => !string.IsNullOrEmpty(pResim1) ? Convert.FromBase64String(pResim1) : null;
        public byte[] ResimBytes2 => !string.IsNullOrEmpty(pResim2) ? Convert.FromBase64String(pResim2) : null;
        public byte[] ResimBytes3 => !string.IsNullOrEmpty(pResim3) ? Convert.FromBase64String(pResim3) : null;
        public byte[] ResimBytes4 => !string.IsNullOrEmpty(pResim4) ? Convert.FromBase64String(pResim4) : null;
        public byte[] ResimBytes5 => !string.IsNullOrEmpty(pResim5) ? Convert.FromBase64String(pResim5) : null;
        public byte[] ResimBytes6 => !string.IsNullOrEmpty(pResim6) ? Convert.FromBase64String(pResim6) : null;
        public string sBarkod { get; set; }
        public string opt { get; set; } = ">";
        public string sDepo { get; set; }
        public bool getAll { get; set; } = false;
    }
}