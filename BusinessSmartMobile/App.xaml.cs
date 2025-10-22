using BusinessSmartMobile.Models;
using BusinessSmartMobile.Services;

namespace BusinessSmartMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        protected override async void OnStart()
        {
            var stockService = MauiProgram.CurrentApp.Services.GetService<StockService>();

            if (stockService != null)
            {
                var (stock, error) = await stockService.IsColorOrSize();

                CommonParameters.colorOpen = stock.sRenkAdi;
                CommonParameters.sizeOpen = stock.sBeden;
            }
        }
    }
}
