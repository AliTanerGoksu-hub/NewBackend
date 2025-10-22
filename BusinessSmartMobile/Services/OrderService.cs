using BusinessSmartMobile.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.IO;
using Microsoft.Maui.Storage;
using BusinessSmartMobile.Services;
using Azure;

namespace BusinessSmartMobile.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly string _uri;

        public OrderService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _uri = _httpClient.BaseAddress.AbsoluteUri;
        }

        public async Task<(List<TbSiparis>, string)> GetOrders(
            string lSiparisNo,
            string sSiparisiVeren,
            string sFirmaAciklama,
            string opt,
            double lKalan,
            DateTime? beginDate,
            DateTime? endDate,
            string? sSaticiRumuzu = null,
            string? sDepo = null)
        {
            try
            {
                sSaticiRumuzu = sSaticiRumuzu ?? _authService.Auth?.sSaticiRumuzu ?? throw new Exception("Satıcı rumuzu bulunamadı.");
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");

                var url = $"{_uri}api/TbSiparis/GetOrders" +
                          $"?lSiparisNo={lSiparisNo}" +
                          $"&sSiparisiVeren={sSiparisiVeren}" +
                          $"&sFirmaAciklama={sFirmaAciklama}" +
                          $"&opt={opt}" +
                          $"&lKalan={lKalan}" +
                          $"&beginDate={beginDate:yyyy-MM-dd}" +
                          $"&endDate={endDate:yyyy-MM-dd}" +
                          $"&sSaticiRumuzu={sSaticiRumuzu}" +
                          $"&sDepo={sDepo}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var siparisler = await response.Content.ReadFromJsonAsync<List<TbSiparis>>();
                    return (siparisler ?? new List<TbSiparis>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSiparis>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSiparis>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<TbSiparis>, string)> GetOrderDetails(string lSiparisNo, string? sSaticiRumuzu = null)
        {
            try
            {
                sSaticiRumuzu = sSaticiRumuzu ?? _authService.Auth?.sSaticiRumuzu ?? throw new Exception("Satıcı rumuzu bulunamadı.");
                var url = $"{_uri}api/TbSiparis/GetOrderDetails?lSiparisNo={lSiparisNo}&sSaticiRumuzu={sSaticiRumuzu}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var orderDetails = await response.Content.ReadFromJsonAsync<List<TbSiparis>>();
                    return (orderDetails ?? new List<TbSiparis>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSiparis>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSiparis>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<TbFirma>, string)> GetOrderBusinessPartners(string variable, string? sSaticiRumuzu = null)
        {
            try
            {
                sSaticiRumuzu = sSaticiRumuzu ?? _authService.Auth?.sSaticiRumuzu ?? throw new Exception("Satıcı rumuzu bulunamadı.");
                string? sDepo = _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                var response = await _httpClient.GetAsync(
                    $"{_uri}api/TbSiparis/GetOrderBp?variable={variable}&sSaticiRumuzu={sSaticiRumuzu}&sDepo={sDepo}");

                if (response.IsSuccessStatusCode)
                {
                    var firma = await response.Content.ReadFromJsonAsync<List<TbFirma>>();
                    return (firma ?? new List<TbFirma>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbFirma>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbFirma>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<Stock>, string)> GetOrderStocks(int currentPage, int pageSize, string filterString)
        {
            try
            {
                string? sFiyatTipi = _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                string? sDepo = _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                var response = await _httpClient.GetAsync(
                    $"{_uri}api/TbSiparis/GetOrderStocks?currentPage={currentPage}&pageSize={pageSize}&variable={filterString}&sFiyatTipi={sFiyatTipi}&sDepo={sDepo}");

                if (response.IsSuccessStatusCode)
                {
                    var stock = await response.Content.ReadFromJsonAsync<List<Stock>>();
                    return (stock ?? new List<Stock>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<Stock>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<Stock>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<List<Stock>> GetStock(int currentPage, int pageSize, string filterString)
        {
            try
            {
                string? sFiyatTipi = _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                string? sDepo = _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                var response = await _httpClient.GetAsync(
                    $"{_uri}api/TbSiparis/GetStock?currentPage={currentPage}&pageSize={pageSize}&variable={filterString}&sFiyatTipi={sFiyatTipi}&sDepo={sDepo}");

                if (response.IsSuccessStatusCode)
                {
                    var stock = await response.Content.ReadFromJsonAsync<List<Stock>>();
                    return (stock ?? new List<Stock>());
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<Stock>());
                }
            }
            catch (Exception ex)
            {
                return (new List<Stock>());
            }
        }

        public async Task<string> AddOrder(string nFirmaId, List<UrunSecimi> yeniSiparis, string? sFiyatTipi, string? sSaticiRumuzu, string? PERSONELADI, string? sDepo, TbFirmaAdresi adres,TbSiparisAciklamasi siparisAciklamasi)
        {
            try
            {
                sFiyatTipi = sFiyatTipi?.Trim() ?? _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");

                SiparisModel siparisModel = new SiparisModel
                {
                    nFirmaId = nFirmaId,
                    sFiyatTipi = sFiyatTipi,
                    yeniSiparis = yeniSiparis,
                    sDepo = sDepo,
                    sSaticiRumuzu = sSaticiRumuzu,
                    PERSONELADI = PERSONELADI,
                    adres = adres,
                    siparisAciklamasi=siparisAciklamasi
                };


                var response = await _httpClient.PostAsJsonAsync(_uri + $"api/TbSiparis/AddNewOrder", siparisModel);

                if (response.IsSuccessStatusCode)
                {
                    var pdfService = new PdfService();
                    TbFirma tbf = new TbFirma();
                    TbParamGenel tbpg = new TbParamGenel();
                    string errorMessage = "";
                    (tbf, errorMessage) = await GetBp(nFirmaId);
                    (tbpg, errorMessage) = await GetParamGenel();
                    int siparisId = await response.Content.ReadFromJsonAsync<int>();
                    var pdfBytes = pdfService.CreateOrderPdf(yeniSiparis, tbf, tbpg, siparisId, _authService.Auth.PERSONELADI);
                    SavePdfToFile(pdfBytes, "Siparis.pdf");
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sipariş eklenirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Uyarı: {ex.Message}";
            }
        }
        public async Task<string> RePrintPDF(List<UrunSecimi> yeniSiparis, int siparisId)
        {
            try
            {
                var pdfService = new PdfService();
                TbFirma tbf = new TbFirma();
                TbParamGenel tbpg = new TbParamGenel();
                string errorMessage = "";
                (tbf, errorMessage) = await GetBpUsingOrder(siparisId.ToString());
                (tbpg, errorMessage) = await GetParamGenel();
                var pdfBytes = pdfService.CreateOrderPdf(yeniSiparis, tbf, tbpg, siparisId, _authService.Auth.PERSONELADI);
                SavePdfToFile(pdfBytes, "Siparis.pdf");
                return "OK";
            }
            catch (Exception ex)
            {

                return $"Uyarı: {ex.Message}";
            }
        }
        public async Task<(TbFirma, string)> GetBp(string nFirmaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}api/TbFirma/GetBpOrder?nFirmaId={nFirmaId}");
                if (response.IsSuccessStatusCode)
                {
                    var bp = await response.Content.ReadFromJsonAsync<TbFirma>();
                    return (bp ?? new TbFirma(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new TbFirma(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new TbFirma(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(TbFirma, string)> GetBpUsingOrder(string nSiparisId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}api/TbFirma/GetBPOrderUsingOrderId?nSiparisId={nSiparisId}");
                if (response.IsSuccessStatusCode)
                {
                    var bp = await response.Content.ReadFromJsonAsync<TbFirma>();
                    return (bp ?? new TbFirma(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new TbFirma(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new TbFirma(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(TbParamGenel, string)> GetParamGenel()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}api/TbFirma/GetParamGenel");
                if (response.IsSuccessStatusCode)
                {
                    var paramGenel = await response.Content.ReadFromJsonAsync<TbParamGenel>();
                    return (paramGenel ?? new TbParamGenel(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new TbParamGenel(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new TbParamGenel(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public void SavePdfToFile(byte[] pdfBytes, string fileName)
        {
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllBytes(filePath, pdfBytes);
            Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
        }

        public async Task<string> UpdateOrder(int nSiparisID, double lMiktar, double nIskontoYuzdesi, string? sSaticiRumuzu = null, string? sDepo = null)
        {
            try
            {
                sSaticiRumuzu = sSaticiRumuzu ?? _authService.Auth?.sSaticiRumuzu ?? throw new Exception("Satıcı rumuzu bulunamadı.");
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");

                var requestUri = $"{_uri}api/TbSiparis/UpdateOrder?lMiktar={lMiktar}&nSiparisID={nSiparisID}&nIskontoYuzdesi={nIskontoYuzdesi}&sSaticiRumuzu={sSaticiRumuzu}&sDepo={sDepo}";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sipariş güncellenirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }
        public async Task<TbSiparis?> GetById(long id)
        {
            var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Backend’teki muhtemel yolları dene (kendi gerçek yolun farklıysa en üstte onu yaz)
            var endpoints = new[]
            {
        $"api/Orders/GetById?id={id}",
        $"api/Order/GetById?id={id}",
        $"api/TbSiparis/GetById?id={id}"
    };

            foreach (var url in endpoints)
            {
                try
                {
                    var item = await _httpClient.GetFromJsonAsync<TbSiparis>(url, jsonOpts);
                    if (item != null)
                        return item;
                }
                catch
                {
                    // sıradaki endpoint’i dene
                }
            }

            return null;
        }
        public async Task<string> UpdateDiscount(int nSiparisID, double lMiktar, double nIskontoYuzdesi)
        {
            try
            {
              

                var requestUri = $"{_uri}api/TbSiparis/UpdateDiscount?lMiktar={lMiktar}&nSiparisID={nSiparisID}&nIskontoYuzdesi={nIskontoYuzdesi}";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sipariş güncellenirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }
        public async Task<string> DeleteOrderRow(int nSiparisID)
        {
            try
            {
                var requestUri = $"{_uri}api/TbSiparis/DeleteOrderRow?nSiparisID={nSiparisID}";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sipariş silinirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }
        public async Task<string> DeleteOrder(string lSiparisNo)
        {
            try
            {
                var requestUri = $"{_uri}api/TbSiparis/DeleteOrder?lSiparisNo={lSiparisNo}";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sipariş silinirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }

        public class SiparisModel
        {
            public string nFirmaId { get; set; }
            public string? sFiyatTipi { get; set; }
            public List<UrunSecimi> yeniSiparis { get; set; }
            public string? sSaticiRumuzu { get; set; }
            public string? PERSONELADI { get; set; }
            public string? sDepo { get; set; }
            public TbFirmaAdresi? adres { get; set; }
            public TbSiparisAciklamasi? siparisAciklamasi { get; set; }

        }
    }
}