using BarcodeScanning;
using BusinessSmartMobile.Components.BarcodeReader;
namespace BusinessSmartMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            OnAppearing();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Methods.AskForRequiredPermissionAsync();

        }
    }
}
