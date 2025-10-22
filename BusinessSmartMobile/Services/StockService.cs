using BusinessSmartMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace BusinessSmartMobile.Services
{
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly string _uri;

        public StockService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _uri = httpClient.BaseAddress.AbsoluteUri;
        }

        public async Task<(List<TbFiyatTipi>, string)> GetPriceType()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/PriceType");
                if (response.IsSuccessStatusCode)
                {
                    var priceType = await response.Content.ReadFromJsonAsync<List<TbFiyatTipi>>();
                    return (priceType ?? new List<TbFiyatTipi>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbFiyatTipi>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbFiyatTipi>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<TbDepo>, string)> GetWarehouse()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/Warehouse");
                if (response.IsSuccessStatusCode)
                {
                    var warehouse = await response.Content.ReadFromJsonAsync<List<TbDepo>>();
                    return (warehouse ?? new List<TbDepo>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbDepo>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbDepo>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<TbStokBirimCinsi>, string)> GetUnits(string nStokID, string? sDepo = null, string? sFiyatTipi = null)
        {
            try
            {
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                sFiyatTipi = sFiyatTipi ?? _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/GetUnits?nStokID={nStokID}&sDepo={sDepo}&sFiyatTipi={sFiyatTipi}");
                if (response.IsSuccessStatusCode)
                {
                    var units = await response.Content.ReadFromJsonAsync<List<TbStokBirimCinsi>>();
                    return (units ?? new List<TbStokBirimCinsi>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbStokBirimCinsi>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbStokBirimCinsi>(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(Stock, string)> BarcodeRead(string sBarkod, string? sDepo = null, string? sFiyatTipi = null)
        {
            try
            {
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                sFiyatTipi = sFiyatTipi ?? _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/BarcodeRead?sBarkod={sBarkod}&sDepo={sDepo}&sFiyatTipi={sFiyatTipi}");
                if (response.IsSuccessStatusCode)
                {
                    var stock = await response.Content.ReadFromJsonAsync<Stock>();
                    if (stock != null)
                    {
                        Console.WriteLine($"Stok ID: {stock.nStokID}, Resim 1: {(string.IsNullOrEmpty(stock.ImageUrl) ? "Boş" : "Var")}");
                    }
                    return (stock ?? new Stock(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    errorMessage = errorMessage.Contains("no elements") ? "Girilen Barkod Bulunamadı" : errorMessage;
                    return (new Stock(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new Stock(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        public async Task<(List<Stock>, string)> GetStocks(string sKodu, string sAciklama, string? sFiyatTipi = null, string? sDepo = null, string sBarkod = null, string sRenk = null, string sBeden = null, bool getAll = false)
        {
            try
            {
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bilgisi bulunamadı.");
                sFiyatTipi = sFiyatTipi ?? _authService.Auth?.sAktifFiyatTipi ?? throw new Exception("Fiyat tipi bulunamadı.");
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/Stocks?sAciklama={sAciklama}&sFiyatTipi={sFiyatTipi}&sBarkod={sBarkod}&sRenk={sRenk}&sBeden={sBeden}&sKodu={sKodu}&sDepo={sDepo}&getAll={getAll}");
                if (response.IsSuccessStatusCode)
                {
                    var stocks = await response.Content.ReadFromJsonAsync<List<Stock>>();
                    if (stocks?.Any() == true)
                    {
                        foreach (var stock in stocks)
                        {
                            Console.WriteLine($"Stok ID: {stock.nStokID}, Resim 1: {(string.IsNullOrEmpty(stock.ImageUrl) ? "Boş" : "Var")}");
                        }
                    }
                    return (stocks ?? new List<Stock>(), string.Empty);
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

        public async Task<(List<Stock>, string)> GetCatalogItemsAsync(int currentPage, int pageSize, string nStokID, string searchTerm)
        {
            try
            {
                // HttpClient.BaseAddress tanımlıysa relative path kullanmak en sağlıklısı
                var url =
                    $"api/TbStok/CatalogStocks" +
                    $"?currentPage={currentPage}" +
                    $"&pageSize={pageSize}" +
                    $"&nStokID={Uri.EscapeDataString(nStokID ?? string.Empty)}" +
                    $"&searchTerm={Uri.EscapeDataString(searchTerm ?? string.Empty)}";

                var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = await _httpClient.GetFromJsonAsync<List<Stock>>(url, jsonOpts);

                if (response is { Count: > 0 })
                {
#if DEBUG
                    foreach (var stock in response)
                        System.Diagnostics.Debug.WriteLine($"[Catalog] {stock.nStokID} -> ImageUrl={(string.IsNullOrWhiteSpace(stock.ImageUrl) ? "YOK" : stock.ImageUrl)}");
#endif
                    return (response, "");
                }

                return (new List<Stock>(), "Veri bulunamadı.");
            }
            catch (Exception ex)
            {
                return (new List<Stock>(), $"Hata: {ex.Message}");
            }
        }

        public async Task<(Stock, string)> IsColorOrSize()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/TbStok/IsColorOrSize");
                if (response.IsSuccessStatusCode)
                {
                    var stock = await response.Content.ReadFromJsonAsync<Stock>();
                    return (stock ?? new Stock(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new Stock(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new Stock(), $"Veri çekme hatası: {ex.Message}");
            }
        }
    }
}