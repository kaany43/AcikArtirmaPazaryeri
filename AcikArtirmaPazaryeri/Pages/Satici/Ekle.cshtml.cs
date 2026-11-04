using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using AcikArtirmaPazaryeri.Servisler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Pages.Satici
{
    [Authorize(Roles = "Satici")]
    [RequestSizeLimit(60_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 60_000_000)]
    public class EkleModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        private readonly UserManager<Kullanici> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly VinServisi _vinServisi;

        public EkleModel(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager, IWebHostEnvironment env, VinServisi vinServisi)
        {

            _context = context;
            _userManager = userManager;
            _env = env;
            _vinServisi = vinServisi;
        }

        public static readonly string[] Parcalar = { "Kaput", "Tavan", "Bagaj", "Ön tampon", "Arka tampon", "Sol ön çamurluk", "Sağ ön çamurluk", "Sol ön kapı", "Sağ ön kapı", "Sol arka kapı", "Sağ arka kapı", "Sol arka çamurluk", "Sağ arka çamurluk" };

        [BindProperty]
        public IlanGirdi Girdi { get; set; } = new();
        public List<SelectListItem> Kategoriler { get; set; } = new();
        public List<SelectListItem> Markalar { get; set; } = new();
        public List<SelectListItem> Modeller { get; set; } = new();
        public List<int> AracKategoriIdleri { get; set; } = new();
        [BindProperty]
        public List<IFormFile> Fotograflar { get; set; } = new();
        public class IlanGirdi : IValidatableObject
        {
            // Ortak alanlar
            [Display(Name = "Başlık")]
            [Required(ErrorMessage = "Başlık zorunludur.")]
            [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
            public string Baslik { get; set; } = string.Empty;

            [Display(Name = "Açıklama")]
            [Required(ErrorMessage = "Açıklama zorunludur.")]
            [StringLength(5000, ErrorMessage = "Açıklama en fazla 5000 karakter olabilir.")]
            public string Aciklama { get; set; } = string.Empty;

            [Display(Name = "Kategori")]
            [Required(ErrorMessage = "Kategori seçin.")]
            [Range(1, int.MaxValue, ErrorMessage = "Kategori seçin.")]
            public int KategoriId { get; set; }

            [Display(Name = "Başlangıç fiyatı")]
            [Required(ErrorMessage = "Başlangıç fiyatı zorunludur.")]
            [Range(typeof(decimal), "1", "100000000", ErrorMessage = "Başlangıç fiyatı 0'dan büyük olmalı.")]
            public decimal BaslangicFiyati { get; set; }

            [Display(Name = "Minimum artış")]
            [Required(ErrorMessage = "Minimum artış zorunludur.")]
            [Range(typeof(decimal), "1", "100000000", ErrorMessage = "Minimum artış 0'dan büyük olmalı.")]
            public decimal MinimumArtis { get; set; }

            [Display(Name = "Rezerv fiyatı")]
            [Range(typeof(decimal), "1", "100000000", ErrorMessage = "Rezerv fiyatı 0'dan büyük olmalı.")]
            public decimal? RezervFiyati { get; set; }

            [Display(Name = "Başlangıç zamanı")]
            [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
            public DateTime BaslangicZamani { get; set; }

            [Display(Name = "Bitiş zamanı")]
            [Required(ErrorMessage = "Bitiş zamanı zorunludur.")]
            public DateTime BitisZamani { get; set; }

            [Display(Name = "Şehir")]
            [Required(ErrorMessage = "Şehir zorunludur.")]
            [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir.")]
            public string Sehir { get; set; } = string.Empty;

            [Display(Name = "Ürün durumu")]
            public UrunDurumu UrunDurumu { get; set; }

            // Araç alanları
            [Display(Name = "Şasi numarası")]
            [StringLength(17, MinimumLength = 17, ErrorMessage = "Şasi numarası 17 karakter olmalı.")]
            [RegularExpression("^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$", ErrorMessage = "Şasi numarası 17 karakter olmalı ve I, O, Q içermemeli.")]
            public string? SasiNo { get; set; }
            public string? Plaka { get; set; }

            // Sadece formda kullanılır, kaydedilmez.
            [Display(Name = "Marka")]
            public int? MarkaId { get; set; }

            [Display(Name = "Model")]
            public int? AracModeliId { get; set; }
            [Display(Name = "Yıl")]
            public int? Yil { get; set; }
            [Display(Name = "Kilometre")]
            public int? Km { get; set; }
            [Display(Name = "Yakıt türü")]
            public YakitTuru? YakitTuru { get; set; }
            [Display(Name = "Vites türü")]
            public VitesTuru? VitesTuru { get; set; }
            public KasaTipi? KasaTipi { get; set; }
            public string? Renk { get; set; }
            public string? Paket { get; set; }

            // Ekspertiz
            public List<ParcaDurumu> Ekspertiz { get; set; } = new();

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (BitisZamani <= BaslangicZamani)
                {
                    yield return new ValidationResult("Bitiş, başlangıçtan sonra olmalı.", new[] { nameof(BitisZamani) });
                }
                if (BaslangicZamani < DateTime.Now.AddMinutes(-5))
                {
                    yield return new ValidationResult("Başlangıç geçmişte olamaz.", new[] { nameof(BaslangicZamani) });
                }
                if (RezervFiyati.HasValue && RezervFiyati < BaslangicFiyati)
                {
                    yield return new ValidationResult("Rezerv fiyatı başlangıç fiyatından küçük olamaz.", new[] { nameof(RezervFiyati) });
                }
            }
        }

        public async Task OnGetAsync()
        {
            await ListeleriDoldurAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Fotograflar.Count == 0)
            {
                ModelState.AddModelError("Fotograflar", "En az bir fotoğraf yükleyin.");
            }
            if (Fotograflar.Count > 10)
            {
                ModelState.AddModelError("Fotograflar", "En fazla 10 fotoğraf yükleyebilirsiniz.");
            }
            List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".webp" };
            foreach (var foto in Fotograflar)
            {
                if (foto.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("Fotograflar", $"{foto.FileName} 5 MB'tan büyük.");
                }
                var ext = Path.GetExtension(foto.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Fotograflar", $"{foto.FileName}: sadece JPG, PNG veya WEBP.");
                }
            }
            var mevcutKategori = await _context.Kategoriler.FindAsync(Girdi.KategoriId);
            if (mevcutKategori == null && Girdi.KategoriId > 0)
            {
                ModelState.AddModelError("Girdi.KategoriId", "Geçerli bir kategori seçin.");
            }
            if (mevcutKategori != null && mevcutKategori.AracKategorisiMi)
            {
                await AracAlanlariniDogrulaAsync();
            }
            if (!ModelState.IsValid || mevcutKategori == null)
            {
                await ListeleriDoldurAsync();
                return Page();
            }
            var muzayede = new Muzayede
            {
                Baslik = Girdi.Baslik,
                Aciklama = Girdi.Aciklama,
                KategoriId = Girdi.KategoriId,
                SaticiId = _userManager.GetUserId(User)!,
                BaslangicFiyati = Girdi.BaslangicFiyati,
                MinimumArtis = Girdi.MinimumArtis,
                RezervFiyati = Girdi.RezervFiyati,
                BaslangicZamani = Girdi.BaslangicZamani.ToUniversalTime(),
                BitisZamani = Girdi.BitisZamani.ToUniversalTime(),
                Sehir = Girdi.Sehir,
                UrunDurumu = Girdi.UrunDurumu,
                Durum = MuzayedeDurumu.Taslak
            };
            if (mevcutKategori.AracKategorisiMi)
            {
                muzayede.AracDetaylari = new AracDetaylari
                {
                    SasiNo = Girdi.SasiNo!,
                    Plaka = Girdi.Plaka,
                    AracModeliId = Girdi.AracModeliId!.Value,
                    Yil = Girdi.Yil!.Value,
                    Km = Girdi.Km!.Value,
                    YakitTuru = Girdi.YakitTuru!.Value,
                    VitesTuru = Girdi.VitesTuru!.Value,
                    KasaTipi = Girdi.KasaTipi ?? KasaTipi.Sedan,
                    Renk = Girdi.Renk ?? string.Empty,
                    Paket = Girdi.Paket,
                    EkspertizKalemleri = Parcalar.Select((parca, i) => new EkspertizKalemi
                    {
                        Parca = parca,
                        Durum = i < Girdi.Ekspertiz.Count ? Girdi.Ekspertiz[i] : ParcaDurumu.Orijinal
                    }).ToList()
                };
            }
            _context.Muzayedeler.Add(muzayede);
            await _context.SaveChangesAsync();
            var klasor = Path.Combine(_env.WebRootPath, "uploads", "muzayede", muzayede.MuzayedeId.ToString());
            Directory.CreateDirectory(klasor);
            for(int i = 0; i < Fotograflar.Count; i++)
            {
                var foto = Fotograflar[i];
                var uzanti = Path.GetExtension(foto.FileName).ToLowerInvariant();
                var ad = Guid.NewGuid() + uzanti;
                using (var akis = new FileStream(Path.Combine(klasor, ad), FileMode.Create))
                {
                    await foto.CopyToAsync(akis);
                }
                _context.MuzayedeResimleri.Add(new MuzayedeResmi
                {
                    MuzayedeId = muzayede.MuzayedeId,
                    DosyaYolu = $"/uploads/muzayede/{muzayede.MuzayedeId}/{ad}",
                    Sira = i,
                    KapakMi = i == 0
                });
            }
            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "İlan taslak olarak kaydedildi.";
            return RedirectToPage("/Satici/Index");
        }

        private async Task AracAlanlariniDogrulaAsync()
        {
            if (string.IsNullOrWhiteSpace(Girdi.SasiNo))
            {
                ModelState.AddModelError("Girdi.SasiNo", "Şasi numarası zorunludur.");
            }
            if (Girdi.AracModeliId == null || !await _context.AracModelleri.AnyAsync(m => m.AracModeliId == Girdi.AracModeliId))
            {
                ModelState.AddModelError("Girdi.AracModeliId", "Geçerli bir model seçin.");
            }
            if (Girdi.Yil == null || Girdi.Yil < 1950 || Girdi.Yil > DateTime.Now.Year + 1)
            {
                ModelState.AddModelError("Girdi.Yil", $"Yıl 1950 ile {DateTime.Now.Year + 1} arasında olmalı.");
            }
            if (Girdi.Km == null || Girdi.Km < 0 || Girdi.Km > 2_000_000)
            {
                ModelState.AddModelError("Girdi.Km", "Kilometre 0 ile 2.000.000 arasında olmalı.");
            }
            if (Girdi.YakitTuru == null)
            {
                ModelState.AddModelError("Girdi.YakitTuru", "Yakıt türü seçin.");
            }
            if (Girdi.VitesTuru == null)
            {
                ModelState.AddModelError("Girdi.VitesTuru", "Vites türü seçin.");
            }
            if (Girdi.UrunDurumu == UrunDurumu.AzKullanilmis)
            {
                ModelState.AddModelError("Girdi.UrunDurumu", "Araçlar için \"Az kullanılmış\" seçilemez.");
            }
        }

        public async Task<JsonResult> OnGetModellerAsync(int markaId)
        {
            var modeller = await _context.AracModelleri
                .Where(m => m.MarkaId == markaId)
                .OrderBy(m => m.ModelAdi)
                .Select(m => new { id = m.AracModeliId, ad = m.ModelAdi })
                .ToListAsync();
            return new JsonResult(modeller);
        }

        public async Task<JsonResult> OnGetVinCozAsync(string? sasiNo)
        {
            sasiNo = sasiNo?.Trim();
            if (sasiNo == null || sasiNo.Length != 17)
            {
                return new JsonResult(new { basarili = false, mesaj = "Şasi numarası 17 karakter olmalı." });
            }
            var vin = await _vinServisi.CozAsync(sasiNo);
            if (vin?.Marka == null)
            {
                return new JsonResult(new { basarili = false, mesaj = "Şasi numarası çözülemedi, bilgileri elle girin." });
            }

            // Listeler küçük; karşılaştırma bellekte yapılır çünkü ToLower Türkçe ortamda I harfini ı yapar (MITSUBISHI).
            var markalar = await _context.Markalar.ToListAsync();
            var marka = markalar.FirstOrDefault(m => m.MarkaAdi.Equals(vin.Marka, StringComparison.OrdinalIgnoreCase));
            AracModeli? model = null;
            if (marka != null && vin.Model != null)
            {
                var modeller = await _context.AracModelleri.Where(m => m.MarkaId == marka.MarkaId).ToListAsync();
                model = modeller.FirstOrDefault(m => m.ModelAdi.Equals(vin.Model, StringComparison.OrdinalIgnoreCase))
                    ?? modeller.FirstOrDefault(m => m.ModelAdi.Contains(vin.Model, StringComparison.OrdinalIgnoreCase)
                        || vin.Model.Contains(m.ModelAdi, StringComparison.OrdinalIgnoreCase));
            }

            return new JsonResult(new
            {
                basarili = true,
                marka = marka?.MarkaAdi ?? vin.Marka,
                model = model?.ModelAdi ?? vin.Model,
                markaId = marka?.MarkaId,
                modelId = model?.AracModeliId,
                yil = vin.Yil,
                yakit = YakitEslestir(vin.Yakit)?.ToString(),
                vites = VitesEslestir(vin.Vites)?.ToString()
            });
        }

        private static YakitTuru? YakitEslestir(string? yakit)
        {
            if (yakit == null) return null;
            if (yakit.Contains("Hybrid", StringComparison.OrdinalIgnoreCase)) return YakitTuru.Hibrit;
            if (yakit.Contains("Gasoline", StringComparison.OrdinalIgnoreCase)) return YakitTuru.Benzin;
            if (yakit.Contains("Diesel", StringComparison.OrdinalIgnoreCase)) return YakitTuru.Dizel;
            if (yakit.Contains("Electric", StringComparison.OrdinalIgnoreCase)) return YakitTuru.Elektrik;
            return null;
        }

        private static VitesTuru? VitesEslestir(string? vites)
        {
            if (vites == null) return null;
            if (vites.Contains("Automatic", StringComparison.OrdinalIgnoreCase)) return VitesTuru.Otomatik;
            if (vites.Contains("Manual", StringComparison.OrdinalIgnoreCase)) return VitesTuru.Manuel;
            return null;
        }

        private async Task ListeleriDoldurAsync()
        {
            Kategoriler = await _context.Kategoriler
                .Where(k => k.UstKategoriId != null)
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new SelectListItem(k.KategoriAdi, k.KategoriId.ToString()))
                .ToListAsync();
            Markalar = await _context.Markalar
                .OrderBy(m => m.MarkaAdi)
                .Select(m => new SelectListItem(m.MarkaAdi, m.MarkaId.ToString()))
                .ToListAsync();
            if (Girdi.MarkaId != null)
            {
                Modeller = await _context.AracModelleri
                    .Where(m => m.MarkaId == Girdi.MarkaId)
                    .OrderBy(m => m.ModelAdi)
                    .Select(m => new SelectListItem(m.ModelAdi, m.AracModeliId.ToString()))
                    .ToListAsync();
            }
            AracKategoriIdleri = await _context.Kategoriler
                .Where(k => k.AracKategorisiMi)
                .Select(k => k.KategoriId)
                .ToListAsync();
        }
    }
}
