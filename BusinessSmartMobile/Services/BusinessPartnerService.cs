using BusinessSmartMobile.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Services
{
    public class BusinessPartnerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;

        public BusinessPartnerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _uri = httpClient.BaseAddress.AbsoluteUri;
        }

        public async Task<List<TbFirma>> GetBpList(string sAciklama, string sKodu, string sAdres1, string sSaticiRumuzu)
        {
            try
            {
                var businessPartners = await _httpClient.GetFromJsonAsync<List<TbFirma>>(_uri+$"api/TbFirma/GetBP?sAciklama={sAciklama}&sKodu={sKodu}&sAdres1={sAdres1}&sSaticiRumuzu={sSaticiRumuzu}");

                return businessPartners ?? new List<TbFirma>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri çekme hatası: {ex.Message}");
                return new List<TbFirma>();
            }
        }
        
        public async Task<List<Balance>> Balance(string sKodu)
        {
            try
            {
                var balance = await _httpClient.GetFromJsonAsync<List<Balance>>(_uri+ $"api/TbFirma/Balance?sKodu={sKodu}");

                return balance ?? new List<Balance>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri çekme hatası: {ex.Message}");
                return new List<Balance>();
            }
        }
        public async Task<List<ReceiptDetail>> GetReceiptDetail(string lFisNo)
        {
            try
            {
                var balance = await _httpClient.GetFromJsonAsync<List<ReceiptDetail>>(_uri + $"api/TbFirma/ReceiptDetail?lFisNo={lFisNo}");

                return balance ?? new List<ReceiptDetail>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri çekme hatası: {ex.Message}");
                return new List<ReceiptDetail>();
            }
        }

        public async Task<string> APersonelFiyatTipi(string userName)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/TbFirma/APersonelFiyatTipi?PERSONELKODU={userName}");

                if (response.IsSuccessStatusCode)
                {
                    var fiyatTipi = await response.Content.ReadAsStringAsync();
                    return fiyatTipi;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
            catch (Exception ex)
            {
                return $"Veri çekme hatası: {ex.Message}";
            }
        }

        public async Task<(TbFirmaAdresi, string)> GetTbFirmaAdresi(string nFirmaID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}api/TbFirma/GetTbFirmaAdresi?nFirmaId={nFirmaID}");
                if (response.IsSuccessStatusCode)
                {
                    var address = await response.Content.ReadFromJsonAsync<TbFirmaAdresi>();
                    return (address ?? new TbFirmaAdresi(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new TbFirmaAdresi(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new TbFirmaAdresi(), $"Veri çekme hatası: {ex.Message}");
            }
        }

        //public async Task<IEnumerable<TbFirma>> GetBpList()
        //{
        //    try
        //    {
        //        var products = await _httpClient.GetFromJsonAsync<IEnumerable<TbFirma>>(_uri + "api/TbFirma/GetProducts");
        //        return products ?? Enumerable.Empty<TbFirma>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Veri çekme hatası: {ex.Message}");
        //        return Enumerable.Empty<TbFirma>();
        //    }
        //}

    }
}
