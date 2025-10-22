public class TbSiparis
{
    public int nSiparisID { get; set; }
    public string lSiparisNo { get; set; }
    public string dteSiparisTarihi { get; set; }
    public double lMiktari { get; set; }
    public string lSevkiyat { get; set; }
    public double lKalan { get; set; }
    public double lTutari { get; set; }
    public double lIskontoTutari { get; set; }
    public double nIskontoYuzdesi { get; set; }
    public string sFirmaAciklama { get; set; }
    public double nKdvOrani { get; set; }

    public string sKodu { get; set; }
    public string sBirimCinsi { get; set; }
    public string sSiparisiVeren { get; set; }
    public string sFirmaKodu { get; set; }
    public string nStokID { get; set; }
    public string nSiparisTipi { get; set; } = "1";
    public string sKullaniciAdi { get; set; }
    public string sSaticiRumuzu { get; set; } 
    public string sDepo { get; set; } 
    public double lFiyati { get; set; }

    public string opt { get; set; } = ">";
    public DateTime? beginDate { get; set; } = DateTime.Today;
    public DateTime? endDate { get; set; } = DateTime.Today;
}
