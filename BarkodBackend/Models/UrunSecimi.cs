namespace BarkodBackend.Models
{
    public class UrunSecimi
    {
        public TbStok Urun { get; set; } = new();
        public double Miktar { get; set; }
        public string SelectedBirimCinsi { get; set; }

    }
}
