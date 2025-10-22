# 📱 BusinessSmartMobile

.NET MAUI tabanlı, cross-platform işletme yönetim mobil uygulaması.

## 🎯 Özellikler

- ✅ **Stok Yönetimi** - Ürün ve stok takibi
- ✅ **Barkod Okuma** - Kamera ile hızlı ürün sorgulama
- ✅ **Sipariş Yönetimi** - Sipariş oluşturma ve takibi
- ✅ **Cari Hesaplar** - Müşteri/tedarikçi yönetimi
- ✅ **Raporlama** - Satış analizi, ciro, stok raporları
- ✅ **Tahsilat** - Kredi kartı entegrasyonu
- ✅ **PDF Oluşturma** - Raporları PDF olarak kaydetme

## 🚀 Teknolojiler

- **.NET 9.0** - En güncel .NET framework
- **MAUI** - Cross-platform UI framework
- **Blazor Hybrid** - Web teknolojileri ile native uygulama
- **MudBlazor** - Modern UI component kütüphanesi
- **BarcodeScanning** - Barkod okuma entegrasyonu
- **Syncfusion PDF** - PDF oluşturma

## 📱 Desteklenen Platformlar

- 🤖 **Android** 7.0+ (API 24+) - Target: Android 14 (API 34)
- 🍎 **iOS** 14.0+
- 🖥️ **Windows** 10.0.17763.0+
- 💻 **macOS** (MacCatalyst) 14.0+

## 🛠️ Kurulum ve Çalıştırma

### Gereksinimler

- Visual Studio 2022 (17.8+) veya Visual Studio Code
- .NET 9.0 SDK
- Android SDK (Android için)
- Xcode 15+ (iOS için)

### Projeyi Klonlama

```bash
git clone https://github.com/yourusername/BusinessSmartMobile.git
cd BusinessSmartMobile
```

### Bağımlılıkları Yükleme

```bash
dotnet restore
```

### Çalıştırma

#### Android:
```bash
dotnet build -t:Run -f net9.0-android
```

#### iOS:
```bash
dotnet build -t:Run -f net9.0-ios
```

#### Windows:
```bash
dotnet build -t:Run -f net9.0-windows
```

## 📦 Store'a Yayınlama

Store'a yayınlamadan önce mutlaka [**STORE_DEPLOYMENT_GUIDE.md**](STORE_DEPLOYMENT_GUIDE.md) dosyasını okuyun!

### Hızlı Başlangıç

1. **Environment Variables Ayarlayın:**
   ```bash
   cp .env.example .env
   # .env dosyasını düzenleyin
   export ANDROID_KEYSTORE_PASS="your_password"
   export ANDROID_KEY_PASS="your_password"
   ```

2. **Privacy Policy URL'ini Güncelleyin:**
   - `BusinessSmartMobile.csproj` içinde `PrivacyPolicyUrl`
   - `Platforms/iOS/Info.plist` içinde `NSPrivacyPolicyURL`

3. **Release Build:**
   ```bash
   # Android (AAB)
   dotnet build -c Release -f net9.0-android
   
   # iOS
   dotnet build -c Release -f net9.0-ios
   ```

Detaylı bilgi için: [STORE_DEPLOYMENT_GUIDE.md](STORE_DEPLOYMENT_GUIDE.md)

## 🔐 Güvenlik

### ⚠️ ÖNEMLİ NOTLAR

1. **Keystore Şifreleri:**
   - `123456` gibi basit şifreler sadece development için!
   - Production için MUTLAKA güçlü şifreler kullanın
   - Şifreleri environment variable olarak tanımlayın

2. **API Güvenliği:**
   - Backend API HTTPS kullanmalı
   - Geçici HTTP kullanımı için `network_security_config.xml` düzenleyin

3. **Privacy Policy:**
   - `docs/privacy-policy.html` dosyasını web sitenize yükleyin
   - URL'leri config dosyalarında güncelleyin

## 📂 Proje Yapısı

```
BusinessSmartMobile/
├── Components/
│   ├── BarcodeReader/      # Barkod okuma komponentleri
│   ├── Custom Components/  # Özel UI komponentleri
│   ├── Layout/             # Layout komponentleri
│   └── Pages/              # Sayfa komponentleri
├── Models/                 # Veri modelleri
├── Platforms/             
│   ├── Android/            # Android-specific kod
│   ├── iOS/                # iOS-specific kod
│   ├── MacCatalyst/        # macOS-specific kod
│   └── Windows/            # Windows-specific kod
├── Resources/              # Görseller, fontlar, vb.
├── Services/               # Business logic servisler
└── wwwroot/                # Web assets (Blazor)
```

## 🧪 Test

### Unit Tests
```bash
dotnet test
```

### Manual Testing
1. Development build ile test edin
2. TestFlight (iOS) veya Internal Testing (Android) kullanın
3. Crash-free rate'in %97+ olduğundan emin olun

## 📝 Versiyonlama

Semantic Versioning kullanıyoruz: `MAJOR.MINOR.PATCH`

- **MAJOR:** Breaking changes
- **MINOR:** Yeni özellikler (backward compatible)
- **PATCH:** Bug fixes

**Mevcut Versiyon:** 1.0.0

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje [Lisans Türünüz] altında lisanslanmıştır.

## 📞 İletişim

- **E-posta:** privacy@barkodyazilim.com
- **Web:** https://barkodyazilim.com
- **Destek:** https://barkodyazilim.com/support

## ✨ Yapılan İyileştirmeler (Store Hazırlık)

### Güvenlik
- ✅ Keystore şifreleri environment variable'a taşındı
- ✅ Network Security Config eklendi (Android)
- ✅ ATS ayarları düzenlendi (iOS)
- ✅ Cleartext traffic kapatıldı

### Store Gereksinimleri
- ✅ Android Target API 34'e yükseltildi
- ✅ AAB format'a geçildi (Release builds)
- ✅ Permission açıklamaları eklendi
- ✅ Privacy Policy oluşturuldu
- ✅ Privacy Manifest güncellendi (iOS)
- ✅ Store deployment guide oluşturuldu

### Dokümantasyon
- ✅ README.md
- ✅ STORE_DEPLOYMENT_GUIDE.md
- ✅ Privacy Policy (HTML)
- ✅ Environment variables template

---

**Son Güncelleme:** Ekim 2024  
**Store Hazır:** ✅ Evet

Sorularınız için: [STORE_DEPLOYMENT_GUIDE.md](STORE_DEPLOYMENT_GUIDE.md)
