# 🚀 BusinessSmartMobile - Store Yayınlama Rehberi

## 📋 İçindekiler
1. [Ön Hazırlık](#ön-hazırlık)
2. [Google Play Store](#google-play-store)
3. [Apple App Store](#apple-app-store)
4. [Güvenlik Notları](#güvenlik-notları)
5. [Test ve Kontrol Listesi](#test-ve-kontrol-listesi)

---

## 🎯 Ön Hazırlık

### ✅ Tamamlanan İyileştirmeler

Projenizde aşağıdaki store gereksinimleri karşılanmıştır:

- ✅ **Android Target API 34** (Google Play 2024 standardı)
- ✅ **Permission Açıklamaları** (Android & iOS)
- ✅ **Network Security Config** (Android)
- ✅ **ATS Security Settings** (iOS)
- ✅ **Privacy Manifest** (iOS)
- ✅ **Privacy Policy** (docs/privacy-policy.html)
- ✅ **Keystore Güvenliği** (Environment variables)
- ✅ **AAB Format** (Release build için)

### 📝 Eksik Kalan İşler

Aşağıdaki adımları sizin tamamlamanız gerekiyor:

1. **Privacy Policy URL'ini güncelleyin**
   - `BusinessSmartMobile.csproj` → `PrivacyPolicyUrl` değerini değiştirin
   - `Info.plist` → `NSPrivacyPolicyURL` değerini değiştirin
   - Privacy Policy HTML'i web sitenize yükleyin

2. **Backend API HTTPS'e geçirin** (Önerilen)
   - Veya `network_security_config.xml` içinde domain exception ekleyin

3. **Store Assets Hazırlayın**
   - Screenshots
   - Feature graphic
   - App descriptions

---

## 📱 Google Play Store

### 1. Developer Account Oluşturma

```
URL: https://play.google.com/console
Maliyet: $25 (tek seferlik)
Süre: ~48 saat (hesap onayı)
```

### 2. Release Build Oluşturma

#### Adım 1: Environment Variables Ayarlayın

```bash
# Production keystore şifrelerini ayarlayın
export ANDROID_KEYSTORE_PASS="your_secure_password"
export ANDROID_KEY_PASS="your_secure_password"
```

⚠️ **ÖNEMLİ:** `123456` şifresini değiştirin! Production keystore oluşturun:

```bash
keytool -genkey -v -keystore bsm-production.keystore \
  -alias bsm -keyalg RSA -keysize 2048 -validity 10000
```

#### Adım 2: Release Build

```bash
# .NET MAUI ile build
dotnet build BusinessSmartMobile.csproj \
  -c Release \
  -f net9.0-android \
  -p:AndroidPackageFormat=aab \
  -p:AndroidSigningKeyStore=bsm-release.keystore
```

veya Visual Studio'da:
```
Configuration: Release
Target: Android
Build → Archive for Publishing
```

#### Adım 3: AAB Dosyası

Build sonrası AAB dosyası burada olacak:
```
bin/Release/net9.0-android/com.barkodyazilim.bsm-Signed.aab
```

### 3. Google Play Console'a Yükleme

#### 3.1. App Oluşturma
1. Play Console → "Create app"
2. App detaylarını doldurun:
   - **App name:** BusinessSmartMobile
   - **Default language:** Turkish
   - **App/Game:** App
   - **Free/Paid:** Free (veya Paid)

#### 3.2. Store Listing
Gerekli bilgiler:

**App Details:**
```
Short description (80 karakter):
"İşletmeniz için eksiksiz stok, sipariş ve cari yönetim çözümü"

Full description (4000 karakter):
[Uygulamanızın detaylı açıklaması]
- Stok yönetimi
- Barkod okuma
- Sipariş takibi
- Cari hesaplar
- Raporlama
```

**Assets Gereksinimleri:**
- Icon: 512x512 PNG
- Feature Graphic: 1024x500 PNG
- Screenshots:
  - En az 2 adet
  - Phone: 16:9 veya 9:16 (1080x1920 önerilir)
  - 7" Tablet: 1024x600
  - 10" Tablet: 1920x1200

#### 3.3. Content Rating
1. Content Rating → Start Questionnaire
2. Category: Utility veya Business
3. Soruları cevaplayın (şiddet, uyuşturucu, vb. yok)

#### 3.4. Target Audience
1. Age groups: 18+ (Business app)

#### 3.5. Data Safety
**Toplanan Veriler:**
- ✅ Cihaz kimliği (Device ID)
- ✅ Kamera erişimi (Sadece barkod için)
- ❌ Konum (Kullanılmıyorsa işaretlemeyin)

**Privacy Policy URL:**
```
https://yourdomain.com/privacy-policy.html
```

#### 3.6. App Access
- "All functionality is available without special access" seçin
- Veya test hesabı bilgileri ekleyin

### 4. Release Yapma

#### 4.1. Production Track
1. Production → Create new release
2. AAB dosyasını yükleyin
3. Release notes ekleyin:

```
Version 1.0
• İlk sürüm
• Stok yönetimi
• Barkod okuma
• Sipariş takibi
• Cari hesaplar
```

#### 4.2. Review & Rollout
1. Review your release
2. "Start rollout to Production"
3. Confirm

⏱️ **Review Süresi:** 1-7 gün

---

## 🍎 Apple App Store

### 1. Apple Developer Account

```
URL: https://developer.apple.com
Maliyet: $99/yıl
Süre: ~24-48 saat (onay)
```

### 2. App Store Connect Hazırlık

#### 2.1. Bundle ID Oluşturma
1. Developer Portal → Certificates, IDs & Profiles
2. Identifiers → (+) New
3. App IDs → Continue
4. Bundle ID: `com.barkodyazilim.bsm`

#### 2.2. Provisioning Profile
1. Profiles → (+) New
2. Distribution → App Store
3. Bundle ID'nizi seçin
4. Certificate seçin (veya yeni oluşturun)

### 3. Archive Oluşturma

#### Visual Studio Mac/Windows:
```
1. Configuration: Release
2. Target: iOS Device (Generic)
3. Build → Archive for Publishing
4. Sign and Distribute
5. App Store
```

#### .NET CLI:
```bash
dotnet publish BusinessSmartMobile.csproj \
  -c Release \
  -f net9.0-ios \
  -p:ArchiveOnBuild=true \
  -p:RuntimeIdentifier=ios-arm64
```

### 4. App Store Connect

#### 4.1. App Oluşturma
1. App Store Connect → My Apps → (+)
2. New App:
   - **Platform:** iOS
   - **Name:** BusinessSmartMobile
   - **Primary Language:** Turkish
   - **Bundle ID:** com.barkodyazilim.bsm
   - **SKU:** bsm001

#### 4.2. App Information

**Category:**
```
Primary: Business
Secondary: Productivity
```

**Privacy Policy URL:**
```
https://yourdomain.com/privacy-policy.html
```

**Support URL:**
```
https://yourdomain.com/support
```

#### 4.3. Pricing and Availability
- **Price:** Free (veya fiyat belirleyin)
- **Availability:** Tüm ülkeler

#### 4.4. App Privacy

**Data Collection:**
1. Data Types → Add data types
   - **Device ID:** Yes (Uygulamayı tanımlamak için)
   - **Camera:** Yes (Barkod tarama için)
   - **Purchase History:** Yes (Siparişler için)

2. Data Usage:
   - App Functionality ✓
   - Analytics ✗
   - Third-Party Advertising ✗

#### 4.5. Version Information

**Screenshots Gereksinimleri:**
```
iPhone 6.7" (Pro Max):  1290 x 2796 px
iPhone 6.5" (Plus):     1242 x 2688 px
iPhone 5.5":            1242 x 2208 px
iPad Pro 12.9" (3rd):   2048 x 2732 px
iPad Pro 12.9" (2nd):   2048 x 2732 px
```

En az 3, maksimum 10 screenshot.

**App Preview Video (Opsiyonel):**
- 15-30 saniye
- 1920x1080 veya 1080x1920

**Description:**
```
BusinessSmartMobile ile işletme yönetiminizi kolaylaştırın!

ÖZELLİKLER:
✓ Stok Yönetimi - Ürün ve stok takibi
✓ Barkod Okuma - Hızlı ürün sorgulama
✓ Sipariş Yönetimi - Kolay sipariş oluşturma
✓ Cari Hesaplar - Müşteri/tedarikçi takibi
✓ Raporlama - Detaylı analiz ve raporlar
✓ Tahsilat - Kredi kartı ile ödeme

[Detaylı açıklama buraya...]
```

**Keywords (100 karakter):**
```
stok,barkod,sipariş,cari,envanter,iş,yönetim,raporlama
```

**What's New (4000 karakter):**
```
Version 1.0
• İlk sürüm yayınlandı
• Tüm temel özellikler aktif
```

### 5. TestFlight (Beta Testing)

Build'inizi önce TestFlight'a gönderin:

1. Xcode → Upload to App Store
2. App Store Connect → TestFlight
3. Internal Testing Group oluşturun
4. Test edin!

✅ **Crash-Free Rate:** Minimum %97 olmalı

### 6. Submit for Review

1. Version → Submit for Review
2. Export Compliance:
   - "No" (Şifreleme API kullanmıyorsanız)
   - "Yes" + explanation (HTTPS kullanıyorsanız)
3. Advertising Identifier: No

⏱️ **Review Süresi:** 24-48 saat (ortalama)

---

## 🔐 Güvenlik Notları

### 1. Keystore Şifreleri

**❌ YAPMAYIN:**
```xml
<AndroidSigningStorePass>123456</AndroidSigningStorePass>
```

**✅ YAPIN:**
```bash
# CI/CD ortamınızda
export ANDROID_KEYSTORE_PASS="SecurePassword123!"
export ANDROID_KEY_PASS="SecurePassword123!"
```

### 2. Production Keystore

**Yeni keystore oluşturun:**
```bash
keytool -genkey -v \
  -keystore bsm-production.keystore \
  -alias bsm \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000 \
  -storepass YOUR_SECURE_PASSWORD \
  -keypass YOUR_SECURE_PASSWORD
```

⚠️ **ÖNEMLİ:** Bu keystore'u GÜVENLİ bir yerde saklayın!
- Kaybederseniz, uygulama güncelleyemezsiniz!
- Yedeklerin, şifrelerin güvende olduğundan emin olun!

### 3. API Security

**Backend API HTTPS kullanmalı!**

Geçici olarak HTTP kullanıyorsanız:

**Android:**
`Resources/xml/network_security_config.xml` içinde:
```xml
<domain-config cleartextTrafficPermitted="true">
    <domain includeSubdomains="true">yourapi.com</domain>
</domain-config>
```

**iOS:**
`Info.plist` içinde:
```xml
<key>NSExceptionDomains</key>
<dict>
    <key>yourapi.com</key>
    <dict>
        <key>NSExceptionAllowsInsecureHTTPLoads</key>
        <true/>
    </dict>
</dict>
```

---

## ✅ Test ve Kontrol Listesi

### Pre-Launch Checklist

#### Teknik
- [ ] App çökmüyor (crash-free rate > 97%)
- [ ] Tüm özellikler çalışıyor
- [ ] Performans sorunları yok
- [ ] Memory leak yok
- [ ] Battery drain yok
- [ ] Network hataları düzgün handle ediliyor

#### Store Requirements
- [ ] Privacy Policy URL güncel
- [ ] Screenshots hazır (tüm boyutlar)
- [ ] App description yazıldı (TR + EN)
- [ ] Keywords optimize edildi
- [ ] Category doğru seçildi
- [ ] Content rating tamamlandı
- [ ] Data safety/privacy bilgileri doğru

#### Security
- [ ] Production keystore oluşturuldu
- [ ] Keystore şifreleri environment variable'da
- [ ] HTTPS kullanılıyor (veya exception tanımlı)
- [ ] API keys güvenli

#### Legal
- [ ] Privacy Policy onaylandı
- [ ] Terms of Service (opsiyonel)
- [ ] KVKK uyumluluğu kontrol edildi

### Test Cihazlar

**Android:**
- [ ] Android 14 (API 34)
- [ ] Android 12 (API 31)
- [ ] Android 10 (API 29)
- [ ] Farklı ekran boyutları

**iOS:**
- [ ] iOS 18
- [ ] iOS 17
- [ ] iOS 16
- [ ] iPhone & iPad

---

## 📞 Yardım ve Destek

### Faydalı Linkler

**Google Play:**
- [Developer Console](https://play.google.com/console)
- [Launch Checklist](https://developer.android.com/distribute/best-practices/launch/launch-checklist)
- [Policy Center](https://play.google.com/about/developer-content-policy/)

**Apple App Store:**
- [App Store Connect](https://appstoreconnect.apple.com)
- [Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)

### Sık Karşılaşılan Sorunlar

**1. Google Play: "Target SDK version must be 34 or higher"**
✅ Çözüldü: `.csproj` dosyasında `TargetSdkVersion` 34'e güncellendi

**2. iOS: "App Transport Security issue"**
✅ Çözüldü: `Info.plist` dosyasında ATS ayarları yapılandırıldı

**3. "Privacy Policy required"**
✅ Çözüldü: `docs/privacy-policy.html` oluşturuldu

**4. Keystore şifresi hardcoded**
✅ Çözüldü: Environment variable kullanımına geçildi

---

## 🎉 Başarılar!

Store'da yayınlanma süreciniz için başarılar dileriz! 

Sorularınız için: privacy@barkodyazilim.com

---

**Son Güncelleme:** Ekim 2024  
**Versiyon:** 1.0
