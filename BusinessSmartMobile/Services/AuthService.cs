using BusinessSmartMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _uri;

        public Auth Auth { get; set; } = new Auth(); // 🔧 Eklenen satır

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _uri = httpClient.BaseAddress.AbsoluteUri;
        }

        public async Task<(Auth, string)> Login(string user, string password)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/User/Login?user={user}&password={password}");

                if (response.IsSuccessStatusCode)
                {
                    var loginUser = await response.Content.ReadFromJsonAsync<Auth>();
                    Auth = loginUser ?? new Auth(); // 🔧 Oturum bilgisi burada saklanıyor
                    return (Auth, string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new Auth(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new Auth(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<(List<Auth>, string)> GetMobileUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/User/GetMobileUsers");

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<Auth>>();
                    return (users ?? new List<Auth>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<Auth>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<Auth>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
       
        public async Task<(List<TbYetkiRapor>, string)> GetReportPerm(string personelKodu)
        {
            try
            {
                var response = await _httpClient.GetAsync(_uri + $"api/User/GetYetkiRapor?personelKodu={personelKodu}");

                if (response.IsSuccessStatusCode)
                {
                    var perm = await response.Content.ReadFromJsonAsync<List<TbYetkiRapor>>();
                    return (perm ?? new List<TbYetkiRapor>(), string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (new List<TbYetkiRapor>(), errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (new List<TbYetkiRapor>(), $"Veri çekme hatası: {ex.Message}");
            }
        }
        public async Task<string> UpdatePerm(int permissionID,bool yetki)
        {
            try
            {
                var requestUri = $"{_uri}api/User/UpdatePermission?permissionID={permissionID}&yetki={yetki}";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Yetki güncellenirken hata oluştu: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }
    }
}
