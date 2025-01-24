# Blog Sitesi Projesi

Bu proje, **ASP.NET Core 5 MVC** kullanılarak geliştirilmiş profesyonel bir blog sitesi uygulamasıdır. Aşağıda proje hakkında genel bilgiler ve temel kurulum adımları yer almaktadır.

---

## Özellikler

### Kullanıcı Sistemi
- Kullanıcı kayıt ve giriş işlemleri.
- Şifre sıfırlama desteği.
- Rollere göre özelleştirilmiş yetkilendirme.

### Admin Paneli
Admin paneli, birçok içeriğin yönetilmesine olanak tanır:
- **Hakkımızda**: Hakkımızda bölümü metinlerini düzenleme.
- **Sözleşmeler**: Kullanım koşulları, gizlilik politikaları gibi sözleşmelerin yönetimi.
- **Blog Kategorileri**: Blog kategorileri ekleme, düzenleme ve silme.
- **Bloglar**: Blog içeriklerini yönetme, yayın durumunu değiştirme.
- **Ürünler**: Ürün listesi yönetimi, fiyat ve stok bilgisi güncelleme.
- **Ekibimiz**: Ekip üyelerini tanıtan alanı düzenleme.
- **Slider**: Ana sayfa slider yönetimi.
- **Sosyal Medya İkonları**: Sosyal medya bağlantılarını düzenleme.

---

## Teknolojiler
- **Framework**: ASP.NET Core 5
- **Veritabanı**: MS SQL Server
- **Frontend**: HTML, CSS, Bootstrap, JavaScript, jQuery
- **ORM**: Entity Framework Core
- **Kimlik Doğrulama**: ASP.NET Core Identity

---

## Kurulum

### 1. Gerekli Araçları Yükleyin
- [Visual Studio](https://visualstudio.microsoft.com/) (ASP.NET Core geliştirme araçları yüklenmiş olmalıdır).
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) veya Azure SQL.

### 2. Projeyi Klonlayın
```bash
git clone https://github.com/kullaniciadi/blog-sitesi.git
cd blog-sitesi
```

### 3. Veritabanı Bağlantısını Yapılandırın
`appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi veritabanı bilgilerinize göre güncelleyin:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SUNUCUNUZ;Database=BlogDb;Trusted_Connection=True;"
}
```

### 4. Veritabanını Oluşturun
Aşağıdaki komutlarla veritabanı migrasyonlarını uygulayın:
```bash
dotnet ef database update
```

### 5. Projeyi Çalıştırın
```bash
dotnet run
```

### Youtube Link:
``` bash
https://youtu.be/KtFgPTF6OBI?si=1zseml4pzXn758Ek

---


