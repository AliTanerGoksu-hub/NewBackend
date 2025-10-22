using BusinessSmartMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BusinessSmartMobile.Services.OrderService;

namespace BusinessSmartMobile.Services
{
    class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _uri = httpClient.BaseAddress.AbsoluteUri;
        }

        public async Task<(List<TbFirma>, string)> GetAccounts()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Payment/GetAccounts");

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
        public async Task<(List<TbOdemeSekli>, string)> GetPaymentType()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/Payment/GetPaymentType");

                if (response.IsSuccessStatusCode)
                {
                    var odemeSekli = await response.Content.ReadFromJsonAsync<List<TbOdemeSekli>>();
                    return (odemeSekli ?? new List<TbOdemeSekli>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbOdemeSekli>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbOdemeSekli>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<string> AddPayment(string sKodu, string sOdemeSeki, double lTutar)
        {
            try
            {

                var request = new HttpRequestMessage(HttpMethod.Post, _uri + $"api/Payment/AddPayment?sKodu={sKodu}&sOdemeSekli={sOdemeSeki}&lTutar={lTutar}");
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                   
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Ödeme yapılırken bir hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }
    }
}
