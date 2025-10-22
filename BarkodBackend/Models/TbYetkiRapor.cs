namespace BarkodBackend.Models
{
    public class TbYetkiRapor
    {
        public int ID { get; set; }
        public string PERSONELKODU { get; set; }
        public string PERSONELADI { get; set; }
        public int RaporID { get; set; }
        public string RaporAciklama { get; set; }
        public bool YetkisiVar { get; set; }
    }
}
