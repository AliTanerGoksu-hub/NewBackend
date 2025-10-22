namespace BarkodBackend.Models
{
    public sealed class CatalogStockDto
    {
        public string nStokID { get; set; }
        public string sKodu { get; set; }
        public string sAciklama { get; set; }
        public string sOzelNot { get; set; }
        public string sKisaAdi { get; set; }
        public string sBeden { get; set; }
        public string sRenkAdi { get; set; }
        public string sBarkod { get; set; }
        public decimal nBirimCarpan { get; set; }
        public decimal lMevcut2 { get; set; }
        public decimal lMevcut { get; set; }
        public decimal lFiyat1 { get; set; }
        public decimal lFiyat2 { get; set; }
        public decimal lFiyat3 { get; set; }
        public decimal lFiyat4 { get; set; }
        public decimal FIYAT5 { get; set; }
        public decimal FIYAT6 { get; set; }
        public decimal nKdvOrani { get; set; }

        // Mobil için gösterilecek resim kaynağı:
        // 1) R2 URL varsa o
        // 2) Yoksa (zorunluysa) data:image/jpeg;base64,... (performans için büyük olabilir)
        public string ImageUrl { get; set; }
        public bool HasResim { get; set; }
    }
}