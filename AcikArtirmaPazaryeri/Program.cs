using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AcikArtirmaPazaryeriDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// Identity: kullanıcılar Kullanici sınıfıyla, roller IdentityRole ile tutulur; ikisi de bizim DbContext'imize kaydedilir.
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AcikArtirmaPazaryeriDbContext>()
    .AddDefaultTokenProviders();

// Giriş gerektiren bir sayfaya girişsiz gelen kullanıcı buraya yönlendirilir.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Hesap/Giris";
    options.AccessDeniedPath = "/Hesap/ErisimEngellendi";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AcikArtirmaPazaryeriDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<Kullanici>>();
    var yoneticiEposta = app.Configuration["Yonetici:Eposta"]
        ?? throw new InvalidOperationException("Yönetici e-postası bulunamadı: User Secrets'a Yonetici:Eposta ekleyin.");
    var yoneticiSifre = app.Configuration["Yonetici:Sifre"]
        ?? throw new InvalidOperationException("Yönetici şifresi bulunamadı: User Secrets'a Yonetici:Sifre ekleyin.");
    context.Database.Migrate();

    // Kategori ağacı: (ana kategori, araç kategorisi mi, alt kategoriler).
    // Eksik olan eklenir, var olana dokunulmaz; bu yüzden uygulama her açıldığında güvenle çalışır.
    var kategoriAgaci = new (string Ad, bool Arac, string[] Altlar)[]
    {
        ("Vasıta", true, new[] { "Otomobil", "Arazi, SUV & Pickup", "Elektrikli Araçlar", "Motosiklet",
            "Minivan & Panelvan", "Ticari Araçlar", "Deniz Araçları", "Hasarlı Araçlar", "Karavan",
            "Klasik Araçlar", "Hava Araçları", "ATV", "UTV", "Engelli Plakalı Araçlar" }),
        ("Elektronik", false, new[] { "Bilgisayar", "Cep Telefonu & Aksesuar", "Fotoğraf & Kamera",
            "Ev Elektroniği", "Elektrikli Ev Aletleri", "Oyunculara Özel", "Teknik Elektronik" }),
        ("Saat", false, Array.Empty<string>()),
        ("Koleksiyon", false, new[] { "Antika" }),
        ("Ev & Yaşam", false, new[] { "Ev Dekorasyon", "Bahçe & Yapı Market", "Ofis & Kırtasiye", "Anne & Bebek" }),
        ("Moda & Aksesuar", false, new[] { "Giyim & Aksesuar", "Takı & Mücevher" }),
        ("Hobi & Eğlence", false, new[] { "Hobi & Oyuncak", "Kitap, Dergi & Film", "Müzik", "Spor" }),
        ("Diğer Her Şey", false, Array.Empty<string>())
    };

    // 1. haftanın seed'inden kalan adlar yeni adlara taşınır (eski dosya Windows-1254 kaydedildiği için
    // "Vasıta" ve "Hasarlı" veritabanına bozuk karakterle yazılmış olabilir; baş harflerinden yakalıyoruz).
    foreach (var k in context.Kategoriler.ToList())
    {
        if (k.UstKategoriId == null && k.KategoriAdi.StartsWith("Vas") && k.KategoriAdi != "Vasıta")
            k.KategoriAdi = "Vasıta";
        else if (k.KategoriAdi.StartsWith("Hasarl") && k.KategoriAdi != "Hasarlı Araçlar")
            k.KategoriAdi = "Hasarlı Araçlar";
        else if (k.KategoriAdi == "SUV/Arazi")
            k.KategoriAdi = "Arazi, SUV & Pickup";
        else if (k.KategoriAdi == "Telefon")
            k.KategoriAdi = "Cep Telefonu & Aksesuar";
    }
    context.SaveChanges();

    foreach (var (ad, arac, altlar) in kategoriAgaci)
    {
        var ust = context.Kategoriler.FirstOrDefault(k => k.KategoriAdi == ad && k.UstKategoriId == null);
        if (ust == null)
        {
            ust = new Kategori { KategoriAdi = ad, AracKategorisiMi = arac };
            context.Kategoriler.Add(ust);
            context.SaveChanges();   // KategoriId oluşsun, alt kategoriler buna bağlanacak
        }
        foreach (var altAd in altlar)
        {
            if (!context.Kategoriler.Any(k => k.KategoriAdi == altAd && k.UstKategoriId == ust.KategoriId))
            {
                context.Kategoriler.Add(new Kategori { KategoriAdi = altAd, AracKategorisiMi = arac, UstKategoriId = ust.KategoriId });
            }
        }
    }
    context.SaveChanges();

    var roller = new List<string> { "Yonetici", "Satici", "Alici" };
    foreach (var rol in roller)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(rol));
            if (!result.Succeeded)
            {
                throw new Exception($"Rol oluşturulamadı: {rol}");
            }
        }
    }
    var yonetici = await userManager.FindByEmailAsync(yoneticiEposta);
    if (yonetici == null)
    {
        yonetici = new Kullanici
        {
            UserName = yoneticiEposta,
            Email = yoneticiEposta,
            Ad = "Kaan",
            Soyad = "Yorgun",
            EmailConfirmed = true
        };
        var sonuc = await userManager.CreateAsync(yonetici, yoneticiSifre);
        if (sonuc.Succeeded)
        {
            await userManager.AddToRoleAsync(yonetici, "Yonetici");
        }
        else
        {
            throw new Exception($"Yönetici oluşturulamadı: {string.Join(", ", sonuc.Errors.Select(e => e.Description))}");
        }
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
