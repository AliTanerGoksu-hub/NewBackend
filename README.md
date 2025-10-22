# 🖥️ BarkodBackend API

BusinessSmartMobile uygulamasının backend API servisi.

## 🎯 Özellikler

- ✅ **Cari Hesap Yönetimi** - Müşteri/tedarikçi API'leri
- ✅ **Stok Yönetimi** - Ürün ve stok API'leri
- ✅ **Sipariş Yönetimi** - Sipariş oluşturma ve takibi
- ✅ **Kullanıcı Yönetimi** - Authentication ve authorization
- ✅ **Raporlama** - Satış analizi, ciro, stok raporları
- ✅ **Ödeme Entegrasyonu** - Tahsilat API'leri

## 🚀 Teknolojiler

- **.NET 8.0** - Web API framework
- **Entity Framework Core** - ORM
- **Dapper** - Micro-ORM (performans için)
- **SQL Server** - Veritabanı
- **Swagger/OpenAPI** - API dokümantasyonu

## 🛠️ Kurulum ve Çalıştırma

### Gereksinimler

- .NET 8.0 SDK
- SQL Server 2014 veya üstü
- Visual Studio 2022 / VS Code / Rider

### 1. Repository'yi Klonlama

```bash
git clone https://github.com/AliTanerGoksu-hub/Backend.git
cd Backend/BarkodBackend
```

### 2. Environment Variables Ayarlama

```bash
# .env.example dosyasını kopyalayın
cp .env.example .env

# .env dosyasını düzenleyin
nano .env
```

**Zorunlu Değişkenler:**
```env
DB_CONNECTION_STRING=Server=YOUR_SERVER;Database=YOUR_DB;User Id=sa;Password=YOUR_PASS;
```

### 3. Database Migration (İlk Kurulum)

```bash
# Database'i oluşturun
dotnet ef database update

# Veya mevcut database'i kullanıyorsanız connection string'i güncelleyin
```

### 4. Bağımlılıkları Yükleme

```bash
dotnet restore
```

### 5. Çalıştırma

#### Development:
```bash
dotnet run --environment Development
```

#### Production:
```bash
dotnet run --environment Production
```

**API Endpoints:**
- HTTP: `http://localhost:4909`
- HTTPS: `https://localhost:4910`
- Swagger UI: `http://localhost:4909/swagger`

## 📝 API Dokümantasyonu

### Swagger UI

Development modunda çalıştırdığınızda Swagger UI otomatik olarak aktif olur:

```
http://localhost:4909/swagger
```

### Ana Endpoint'ler

#### 👤 User Controller (`/api/User`)
- `GET /api/User/Login` - Kullanıcı girişi
- `GET /api/User/GetMobileUsers` - Mobil kullanıcıları listele
- `GET /api/User/GetYetkiRapor` - Kullanıcı yetkileri
- `POST /api/User/UpdatePermission` - Yetki güncelle

#### 🏢 Firma Controller (`/api/TbFirma`)
- Cari hesap işlemleri

#### 📦 Stok Controller (`/api/TbStok`)
- Ürün ve stok işlemleri

#### 🛒 Sipariş Controller (`/api/TbSiparis`)
- Sipariş yönetimi

#### 📊 Reports Controller (`/api/Reports`)
- Raporlama API'leri

#### 💳 Payment Controller (`/api/Payment`)
- Ödeme işlemleri

## 🔐 Güvenlik

### Yapılan İyileştirmeler

1. ✅ **SQL Injection Koruması**
   - Tüm sorgular parametreli hale getirildi
   - Input validation eklendi

2. ✅ **CORS Yapılandırması**
   - Mobil uygulama erişimi için CORS eklendi

3. ✅ **HTTPS Desteği**
   - HTTPS endpoint yapılandırması
   - Production'da HTTPS zorunlu

4. ✅ **Error Handling**
   - Generic error messages (detay gizleme)
   - Proper HTTP status codes

5. ✅ **Environment Variables**
   - Connection strings externalized
   - Certificate configuration

### Yapılması Gerekenler

⚠️ **Hala eksik olanlar (gelecekte eklenecek):**

1. **Authentication & Authorization**
   - JWT Token implementation
   - Role-based access control
   - Refresh token mechanism

2. **Password Hashing**
   - Şu anda plain text (ASAP düzeltilmeli!)
   - bcrypt veya Argon2 eklenecek

3. **Rate Limiting**
   - API rate limiting middleware
   - DDoS koruması

4. **Logging & Monitoring**
   - Serilog entegrasyonu
   - Application Insights

5. **Input Validation**
   - FluentValidation
   - DTO validation

## 🔧 Yapılandırma

### appsettings.json

```json
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:4909"
      },
      "Https": {
        "Url": "https://0.0.0.0:4910"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### HTTPS Sertifikası

**Development için self-signed certificate:**

```bash
dotnet dev-certs https --trust
```

**Production için:**
- Let's Encrypt (ücretsiz)
- Ticari SSL sertifikası
- Azure App Service (otomatik)

## 🌍 Deployment

### IIS (Windows Server)

1. Publish:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. IIS'de yeni site oluşturun
3. Application Pool: .NET CLR Version = No Managed Code
4. Binding'leri ayarlayın (HTTP:4909, HTTPS:4910)

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
EXPOSE 4909
EXPOSE 4910
ENTRYPOINT ["dotnet", "BarkodBackend.dll"]
```

### Azure App Service

```bash
az webapp up --name barkod-backend --resource-group myResourceGroup
```

## 🧪 Testing

### API Test (cURL)

```bash
# Health check
curl http://localhost:4909/api/User/GetMobileUsers

# Login
curl "http://localhost:4909/api/User/Login?user=demo_user&password=Demo123!"
```

### Postman Collection

Import edebileceğiniz Postman collection: `BarkodBackend.postman_collection.json` (eklenecek)

## 📊 Database Schema

### Temel Tablolar

- `APERSONEL` - Kullanıcılar
- `TbFirma` - Cari hesaplar
- `TbStok` - Ürünler ve stoklar
- `TbSiparis` - Siparişler
- `TbYetkiRapor` - Yetkilendirme
- `TbRaporlar` - Rapor tanımları

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Versiyonlama

Semantic Versioning kullanıyoruz: `MAJOR.MINOR.PATCH`

**Mevcut Versiyon:** 1.0.0

## 🆘 Sorun Giderme

### "Connection String hatası"

```bash
# .env dosyasını kontrol edin
cat .env

# Veya environment variable tanımlayın
export DB_CONNECTION_STRING="Server=...;Database=...;"
```

### "CORS hatası"

CORS policy'de mobil app origin'i eklenmiş olmalı. `Program.cs` kontrol edin.

### "SQL Server bağlanamıyor"

1. SQL Server servisinin çalıştığından emin olun
2. Firewall ayarlarını kontrol edin
3. Connection string'i doğrulayın

## 📞 İletişim

- **E-posta:** support@barkodyazilim.com
- **GitHub Issues:** https://github.com/AliTanerGoksu-hub/Backend/issues

## 📝 Changelog

### [1.0.1] - 2024-10 (Store Hazırlık Düzeltmeleri)

**Güvenlik:**
- ✅ SQL Injection açıkları düzeltildi (parametreli sorgular)
- ✅ CORS eklendi (mobil app erişimi)
- ✅ HTTPS endpoint yapılandırması
- ✅ Error handling iyileştirildi
- ✅ Environment variables support

**API:**
- ✅ Swagger UI aktif (development)
- ✅ Input validation eklendi
- ✅ Proper HTTP status codes

### [1.0.0] - 2024-XX (İlk Sürüm)

- Initial release
- Temel CRUD operasyonlar
- Mobil app backend desteği

---

**Geliştirici:** Barkod Yazılım  
**Lisans:** Proprietary  
**Last Updated:** Ekim 2024
