# 📱 Store Review Notes / App Review Information

Bu doküman, Apple App Store ve Google Play Store review ekiplerine yönelik açıklamaları içerir.

---

## 🎯 Uygulama Tipi

**B2B (Business-to-Business) Enterprise Uygulaması**

BusinessSmartMobile, işletmelerin kendi backend sunucularına bağlanarak stok, sipariş ve cari hesap yönetimini yapmasını sağlayan kurumsal bir çözümdür.

---

## 🔐 Network Security / ATS Configuration

### ❓ "Why does your app use cleartext traffic / NSAllowsArbitraryLoads?"

**CEVAP:**

Bu bir **B2B enterprise uygulamasıdır** ve aşağıdaki nedenlerle HTTP trafiğine izin verilmiştir:

1. **Müşteri Backend Sunucuları:**
   - Her müşteri kendi backend sunucusunu çalıştırır
   - Backend sunucu adresi müşteriden müşteriye değişir
   - Uygulama içindeki "Settings" sayfasından dinamik olarak yapılandırılır

2. **Dinamik URL Yapılandırması:**
   - Backend URL'leri önceden bilinemez
   - Her müşterinin kendi network altyapısı vardır
   - Bazı müşteriler internal network'te HTTP kullanır (192.168.x.x gibi)
   - Bazı müşteriler public domain'de HTTPS kullanır

3. **Legacy System Desteği:**
   - Bazı müşteriler mevcut HTTP tabanlı backend sistemlerini kullanır
   - Müşterilerin backend sistemlerini değiştirmesi zaman ve maliyet gerektirir

4. **Güvenlik Önlemleri:**
   - Kullanıcılara HTTPS kullanmaları önerilir (Settings sayfasında uyarı)
   - HTTP kullanımında güvenlik uyarısı gösterilir
   - HTTPS bağlantılar yeşil onay işareti ile vurgulanır

5. **Örnek Kullanım Senaryoları:**
   ```
   Müşteri A: http://192.168.1.100:4909 (Local network)
   Müşteri B: https://api.company-b.com (Public HTTPS)
   Müşteri C: http://10.0.50.20:8080 (VPN üzerinden)
   ```

**Network Security Configuration:**
- Android: `android:usesCleartextTraffic="false"` + custom `network_security_config.xml`
- iOS: `NSAllowsArbitraryLoads = true` (justified for B2B use)

---

## 👥 Test Hesabı / Demo Account

Store review için test backend sunucusu:

**Test Backend URL:**
```
http://demo.barkodyazilim.com:4909
```
veya
```
http://192.168.1.119:4909 (VPN gerekli)
```

**Test Kullanıcı Bilgileri:**
```
Kullanıcı Adı: demo_user
Şifre: Demo123!
```

veya

```
Kullanıcı Adı: test_admin
Şifre: Test456!
```

**Test Adımları:**
1. Uygulamayı açın
2. Settings (⚙️) ikonuna tıklayın
3. Backend API adresini girin: `http://demo.barkodyazilim.com:4909`
4. "Kaydet" butonuna tıklayın
5. Login ekranında test kullanıcı bilgileriyle giriş yapın
6. Ana menüden istediğiniz işlemi test edin

**Test Edilebilir Özellikler:**
- ✓ Stok sorgulama
- ✓ Barkod okuma (kamera izni gerekir)
- ✓ Sipariş oluşturma
- ✓ Cari hesap görüntüleme
- ✓ Raporlar

---

## 📸 Kamera İzni / Camera Permission

### ❓ "Why does your app need camera access?"

**CEVAP:**

Kamera erişimi **sadece barkod okuma** için kullanılır:

1. **Barkod Tarama:**
   - Kullanıcılar ürün barkodlarını okuyarak hızlı ürün sorgulama yapabilir
   - Fiyat görüntüleme
   - Stok kontrolü
   - Sipariş ekleme

2. **Fotoğraf Saklanmıyor:**
   - Çekilen fotoğraflar sunucuda saklanmaz
   - Sadece barkod decode edilir
   - Decode sonrası görüntü hemen silinir

3. **İsteğe Bağlı:**
   - Kamera kullanımı opsiyoneldir
   - Kullanıcılar manuel ürün arama da yapabilir
   - `android:required="false"` şeklinde tanımlanmıştır

---

## 📱 Device ID / READ_PHONE_STATE

### ❓ "Why does your app need READ_PHONE_STATE?"

**CEVAP:**

Device ID, **cihaz tanımlama** ve **lisanslama** için kullanılır:

1. **Cihaz Lisanslama:**
   - Her işletme belirli sayıda cihaza lisans alır
   - Device ID ile lisans kontrolü yapılır
   - Yetkisiz cihazların sisteme erişimi engellenir

2. **Çoklu Cihaz Yönetimi:**
   - İşletmeler birden fazla mobil cihaz kullanır
   - Admin panelinde hangi cihazın aktif olduğu görülebilir
   - Kayıp/çalıntı cihazlar sistemden uzaktan devre dışı bırakılabilir

3. **Güvenlik:**
   - Device ID değiştirilemez
   - Uygulama kopyalama/paylaşma engellenmiş olur
   - Her cihazın kullanım geçmişi takip edilir

**Not:** Telefon numarası veya kişisel bilgi toplanmaz, sadece cihaz UUID'si kullanılır.

---

## 🔒 Veri Gizliliği / Data Privacy

### Toplanan Veriler:

1. **Cihaz Bilgileri:**
   - Device ID (UUID)
   - İşletim sistemi versiyonu
   - Uygulama versiyonu

2. **İş Verileri:**
   - Cari hesap bilgileri
   - Ürün ve stok bilgileri
   - Sipariş kayıtları
   - Satış raporları

3. **Kullanıcı Bilgileri:**
   - Kullanıcı adı
   - Personel kodu
   - Şifre (hash'lenmiş)

### Veri Saklama:

- Tüm veriler **müşterinin kendi backend sunucusunda** saklanır
- Bizim sunucularımızda herhangi bir veri saklanmaz
- Her müşteri kendi veritabanını yönetir

### Üçüncü Taraf Paylaşımı:

- Hiçbir veri üçüncü taraflarla paylaşılmaz
- Analytics/tracking servisi kullanılmaz
- Reklam servisi entegrasyonu yoktur

---

## 📊 Content Rating

**Kategori:** Business / Productivity  
**Yaş Sınırı:** 18+ (Business kullanıcıları için)  
**İçerik:** Sadece iş verileri, şiddet/müstehcenlik yok

---

## 🌍 Kullanılabilirlik / Availability

**Hedef Pazar:** Türkiye (primary), diğer ülkeler (secondary)  
**Diller:** Türkçe (primary), İngilizce (planning)  
**Platform:** iOS 14.0+, Android 7.0+ (API 24+, Target: API 34)

---

## 🔄 Uygulama Güncellemeleri

**Güncelleme Politikası:**
- Güvenlik yamaları: Hemen
- Yeni özellikler: Aylık
- Bug fixes: Haftalık

**Changelog:**
```
Version 1.0.0 (İlk Sürüm)
- Stok yönetimi
- Barkod okuma
- Sipariş yönetimi
- Cari hesap yönetimi
- Raporlama
- Tahsilat (kredi kartı)
```

---

## 📞 İletişim / Support

**Developer Contact:**
- Email: support@barkodyazilim.com
- Phone: +90 XXX XXX XX XX
- Website: https://barkodyazilim.com
- Support: https://barkodyazilim.com/support

**Privacy Policy:** https://barkodyazilim.com/privacy-policy.html

---

## ✅ Store Compliance Checklist

### Google Play:
- ✅ Target API 34 (Android 14)
- ✅ AAB format
- ✅ 64-bit support
- ✅ Privacy Policy URL
- ✅ Data Safety section completed
- ✅ Content rating completed
- ✅ App signing configured
- ✅ Network security config

### Apple App Store:
- ✅ iOS 14.0+ support
- ✅ Privacy Manifest (PrivacyInfo.xcprivacy)
- ✅ ATS justification (B2B use case)
- ✅ Camera usage description
- ✅ Privacy Policy URL
- ✅ TestFlight beta tested
- ✅ App Store screenshots (all sizes)
- ✅ Export compliance (No encryption)

---

## 🎯 B2B Justification Summary

**Bu uygulama neden genel tüketici uygulamalarından farklıdır:**

1. **Hedef Kitle:**
   - Genel kullanıcılar değil, işletmeler
   - Her işletme kendi backend'ini çalıştırır
   - Lisanslı kullanım (cihaz başına)

2. **Network Yapısı:**
   - Sabit bir backend URL'i yok
   - Her müşteri kendi URL'ini ayarlar
   - Internal network'te çalışabilir

3. **Güvenlik:**
   - HTTPS kullanımı önerilir ancak müşteri tercihine bağlıdır
   - Internal network'te HTTP kabul edilebilir
   - Veriler müşterinin sunucusunda

4. **Benzer Uygulamalar:**
   - SAP Mobile
   - Oracle ERP Mobile
   - Microsoft Dynamics Mobile
   - Tüm bu uygulamalar da B2B ve custom backend URL kullanır

---

## 📝 Ek Notlar

**Store Review Ekibine:**

Lütfen bu uygulamanın:
- Genel tüketici uygulaması olmadığını
- Kurumsal B2B çözümü olduğunu
- Her müşterinin kendi backend sistemine sahip olduğunu
- HTTP/HTTPS karışık kullanımının industry standard olduğunu (B2B apps için)

göz önünde bulundurun.

Herhangi bir sorunuz veya endişeniz varsa lütfen bizimle iletişime geçin:
support@barkodyazilim.com

Teşekkürler! 🙏

---

**Son Güncelleme:** Ekim 2024  
**Uygulama Versiyonu:** 1.0.0
