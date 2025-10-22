using BusinessSmartMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace BusinessSmartMobile.Platforms.iOS
{
    public class IosDeviceIdentifier:IDeviceService
    {
        public string GetIdentifier()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
    }
}
