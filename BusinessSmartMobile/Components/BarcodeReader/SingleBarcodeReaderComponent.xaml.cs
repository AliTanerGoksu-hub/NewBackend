using System.ComponentModel;

namespace BusinessSmartMobile.Components.BarcodeReader;

public partial class SingleBarcodeReaderComponent : ContentPage
{
    private bool _isTorchOn = false;

    private readonly TaskCompletionSource<string> _barcodeResultCompletionSource = new();
    public Task<string> BarcodeResult => _barcodeResultCompletionSource.Task;

    public event EventHandler<string> OnBarcodeDetected;

    public event PropertyChangedEventHandler PropertyChanged; // ?? Ekle

    private string _infoMessage; // ?? Ekranda geçici mesaj göstermek için
    private bool _barcodeHandled = false; // ?? Barkod bir kere işlensin diye

    public SingleBarcodeReaderComponent()
    {
        InitializeComponent();
        BindingContext = this; // ?? BindingContext set et
    }
    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            if (barcodeScanner == null)
            {
                await DisplayAlert("Hata", "barcodeScanner bileşeni bulunamadı.", "Tamam");
                await Navigation.PopModalAsync();
                return;
            }
            Console.WriteLine("iOS: Kamera izni kontrol ediliyor...");
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("iOS: Kamera izni talep ediliyor...");
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Hata", "Kamera izni verilmedi.", "Tamam");
                    await Navigation.PopModalAsync();
                    return;
                }
            }
            Console.WriteLine("iOS: Kamera etkinleştirme deneniyor...");
            barcodeScanner.CameraEnabled = true;
            if (!barcodeScanner.CameraEnabled)
            {
                await DisplayAlert("Hata", "Kamera etkinleştirilemedi.", "Tamam");
                await Navigation.PopModalAsync();
            }
            else
            {
                Console.WriteLine("iOS: Kamera başarıyla etkinleştirildi.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"iOS Kamera hatası: {ex.Message}");
            await DisplayAlert("Hata", $"Kamera başlatılamadı: {ex.Message}", "Tamam");
            await Navigation.PopModalAsync();
        }
    }

    private async void CameraView_OnDetectionFinished(object sender, BarcodeScanning.OnDetectionFinishedEventArg e)
    {
        if (_barcodeHandled)
            return;

        var detectedCode = e.BarcodeResults?.FirstOrDefault()?.DisplayValue;

        if (!string.IsNullOrWhiteSpace(detectedCode))
        {
            _barcodeHandled = true;

            var temizBarkod = detectedCode.Replace("\r", "").Replace("\n", "").Trim();

            // ?? Event tetikle
            OnBarcodeDetected?.Invoke(this, temizBarkod);

            // ?? Task tamamla
            if (!_barcodeResultCompletionSource.Task.IsCompleted)
                _barcodeResultCompletionSource.TrySetResult(temizBarkod);

            // ?? Kamerayı durdur ve ekranı kapat
            barcodeScanner.CameraEnabled = false;
            await Navigation.PopModalAsync();
        }
    }

    private void TorchButton_Clicked(object sender, EventArgs e)
    {
        string openLightIcon = "\uD83D\uDD26";
        string dimLightIcon = "\uD83D\uDEAB";

        _isTorchOn = !_isTorchOn;
        barcodeScanner.TorchOn = _isTorchOn;
        torchButton.Text = _isTorchOn ? openLightIcon : dimLightIcon;
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        barcodeScanner.CameraEnabled = false;
        await Navigation.PopModalAsync();
    }

    // ?? Geçici mesaj gösterimi
    public void ShowMessage(string message)
    {
        InfoMessage = message;

        Task.Run(async () =>
        {
            await Task.Delay(2000);
            InfoMessage = string.Empty;
        });
    }

    public string InfoMessage
    {
        get => _infoMessage;
        set
        {
            if (_infoMessage != value)
            {
                _infoMessage = value;
                OnPropertyChanged(nameof(InfoMessage));
            }
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}