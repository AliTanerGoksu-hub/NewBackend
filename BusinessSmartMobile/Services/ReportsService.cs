using BusinessSmartMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Services
{
    public class ReportsService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly string _uri;

        public ReportsService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _uri = httpClient.BaseAddress.AbsoluteUri;
        }
        public async Task<(List<TbPayableCheque>, string)> GetPayableCheque(string startDate, string endDate)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/PayableCheque?startDate={startDate}&endDate={endDate}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbPayableCheque>>();
                    return (report ?? new List<TbPayableCheque>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbPayableCheque>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbPayableCheque>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSalesAnalysis>, string)> GetSalesAnalysis(string startDate, string endDate)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesAnalysis?startDate={startDate}&endDate={endDate}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbSalesAnalysis>>();
                    return (report ?? new List<TbSalesAnalysis>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSalesAnalysis>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSalesAnalysis>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<SalesAnalysisDetail>, string)> GetSalesAnalysisDetail(string startDate, string endDate, string nAlisVerisID)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesAnalysisDetail?startDate={startDate}&endDate={endDate}&nAlisVerisID={nAlisVerisID}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<SalesAnalysisDetail>>();
                    return (report ?? new List<SalesAnalysisDetail>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<SalesAnalysisDetail>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<SalesAnalysisDetail>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSalesTurnover>, string)> GetSalesTurnover(string startDate, string endDate, string sDepo = null)
        {
            try
            {
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bulunamadı.");

                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesTurnover?startDate={startDate}&endDate={endDate}&sDepo={sDepo}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbSalesTurnover>>();
                    return (report ?? new List<TbSalesTurnover>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSalesTurnover>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSalesTurnover>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSalesTurnoverVendors>, string)> GetSalesTurnoverVendor(string startDate, string endDate, string sSaticiRumuzu = null)
        {
            try
            {
                sSaticiRumuzu = sSaticiRumuzu ?? _authService.Auth?.sSaticiRumuzu ?? throw new Exception("Satıcı rumuzu bulunamadı.");
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesTurnoverVendors?startDate={startDate}&endDate={endDate}&sSaticiRumuzu={sSaticiRumuzu}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbSalesTurnoverVendors>>();
                    return (report ?? new List<TbSalesTurnoverVendors>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSalesTurnoverVendors>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSalesTurnoverVendors>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSalesTurnoverClassifications>, string)> GetSalesTurnoverClassifications(string startDate, string endDate, string sinif = "1")
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesTurnoverClassifications?startDate={startDate}&endDate={endDate}&sinif={sinif}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbSalesTurnoverClassifications>>();
                    return (report ?? new List<TbSalesTurnoverClassifications>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSalesTurnoverClassifications>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSalesTurnoverClassifications>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSalesRemaining>, string)> GetSalesRemainingReport(string startDate, string endDate, string magaza = "")
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/SalesRemainingReport?startDate={startDate}&endDate={endDate}&magaza={magaza}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbSalesRemaining>>();
                    return (report ?? new List<TbSalesRemaining>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSalesRemaining>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSalesRemaining>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbSatici>, string)> GetVendors()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Reports/GetVendors");
                if (response.IsSuccessStatusCode)
                {
                    var satici = await response.Content.ReadFromJsonAsync<List<TbSatici>>();
                    return (satici ?? new List<TbSatici>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbSatici>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbSatici>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<TbDeliveryReport>, string)> GetDeliveryReport(string startDate, string endDate, string sSaticiRumuzu = null, string sDepo = null, string type = "1")
        {
            try
            {
                sDepo = sDepo ?? _authService.Auth?.sDepo ?? throw new Exception("Depo bulunamadı.");

                var response = await _httpClient.GetAsync(_uri + $"api/Reports/DeliveryReport?startDate={startDate}&endDate={endDate}&sSaticiRumuzu={sSaticiRumuzu}&sDepo={sDepo}&type={type}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadFromJsonAsync<List<TbDeliveryReport>>();
                    return (report ?? new List<TbDeliveryReport>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbDeliveryReport>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbDeliveryReport>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
    }
}
