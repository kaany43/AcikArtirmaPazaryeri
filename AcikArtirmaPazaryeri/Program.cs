using AcikArtirmaPazaryeri.Data;
using Microsoft.EntityFrameworkCore;
using AcikArtirmaPazaryeri.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AcikArtirmaPazaryeriDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AcikArtirmaPazaryeriDbContext>();
    context.Database.Migrate();
    if (!context.Kategoriler.Any())
    {
        context.Kategoriler.AddRange(
            new Kategori
            {
                KategoriAdi = "Vasýta",
                AracKategorisiMi = true,
                AltKategoriler =
                {
                    new Kategori { KategoriAdi = "Otomobil", AracKategorisiMi = true },
                    new Kategori { KategoriAdi = "SUV/Arazi", AracKategorisiMi = true },
                    new Kategori { KategoriAdi = "Motosiklet", AracKategorisiMi = true },
                    new Kategori { KategoriAdi = "Hasarlý/Pert", AracKategorisiMi = true }
                }
            },
            new Kategori
            {
                KategoriAdi = "Elektronik",
                AltKategoriler =
                {
                    new Kategori { KategoriAdi = "Telefon" },
                    new Kategori { KategoriAdi = "Bilgisayar" }
                }
                },
                    new Kategori { KategoriAdi = "Saat" },
                    new Kategori { KategoriAdi = "Koleksiyon" }
        );
        context.SaveChanges();
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

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
