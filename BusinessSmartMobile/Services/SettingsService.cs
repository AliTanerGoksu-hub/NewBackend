using Microsoft.Maui.Storage;
using System;

namespace BusinessSmartMobile.Services
{
    public class SettingsService 
    {
        private const string ApiBaseUrlKey = "ApiBaseUrl";

        public string GetApiBaseUrl()
        {
            return Preferences.Get(ApiBaseUrlKey, "http://192.168.1.104:4909"); 
        }

        public void SetApiBaseUrl(string newBaseUrl)
        {
            Preferences.Set(ApiBaseUrlKey, newBaseUrl);
        }
    }
}
