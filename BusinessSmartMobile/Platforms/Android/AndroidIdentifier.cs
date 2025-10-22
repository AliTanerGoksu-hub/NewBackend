using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Provider;
using BusinessSmartMobile.Services;


namespace BusinessSmartMobile.Platforms.Android
{
    public class AndroidIdentifier:IDeviceService
    {
        public string GetIdentifier()
        {
            var context = Platform.CurrentActivity?.ApplicationContext;
            return Settings.Secure.GetString(context?.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}
