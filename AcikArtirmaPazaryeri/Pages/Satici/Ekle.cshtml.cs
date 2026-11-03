using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Satici
{
    [Authorize(Roles = "Satici")]
    public class EkleModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public EkleModel(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        public static readonly string[] Parcalar = { "Kaput", "Tavan", "Bagaj", "Ön tampon", "Arka tampon", "Sol ön çamurluk", "Sağ ön çamurluk", "Sol ön kapı", "Sağ ön kapı", "Sol arka kapı", "Sağ arka kapı", "Sol arka çamurluk", "Sağ arka çamurluk" };

        [BindProperty]
        public IlanGirdi Girdi { get; set; } = new();
        public List<SelectListItem> Kategoriler { get; set; } = new();
        public List<SelectListItem> Markalar { get; set; } = new();
        public List<int> AracKategoriIdleri { get; set; } = new();
        public class IlanGirdi
        {
            // Ortak alanlar
            public string Baslik { get; set; } = string.Empty;
            public string Aciklama { get; set; } = string.Empty;
            public int KategoriId { get; set; }
            public decimal BaslangicFiyati { get; set; }
            public decimal MinimumArtis { get; set; }
            public decimal? RezervFiyati { get; set; }
            public DateTime BaslangicZamani { get; set; }
            public DateTime BitisZamani { get; set; }
            public string Sehir { get; set; } = string.Empty;
            public UrunDurumu UrunDurumu { get; set; }

            // Araç alanları
            public string? SasiNo { get; set; }
            public string? Plaka { get; set; }
            public int? AracModeliId { get; set; }
            public int? Yil { get; set; }
            public int? Km { get; set; }
            public YakitTuru? YakitTuru { get; set; }
            public VitesTuru? VitesTuru { get; set; }
            public KasaTipi? KasaTipi { get; set; }
            public string? Renk { get; set; }
            public string? Paket { get; set; }

            // Ekspertiz
            public List<ParcaDurumu> Ekspertiz { get; set; } = new();
        }

        public async Task OnGetAsync()
        {
            await ListeleriDoldurAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await ListeleriDoldurAsync();
                return Page();
            }
            var mevcutKategori = await _context.Kategoriler.FindAsync(Girdi.KategoriId);
            if (mevcutKategori == null)
            {
                ModelState.AddModelError("Girdi.KategoriId", "Geçerli bir kategori seçin.");
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
                // 6. adımda doğrulanacak
                muzayede.AracDetaylari = new AracDetaylari
                {
                    SasiNo = Girdi.SasiNo ?? string.Empty,
                    Plaka = Girdi.Plaka,
                    AracModeliId = Girdi.AracModeliId ?? 0,
                    Yil = Girdi.Yil ?? 0,
                    Km = Girdi.Km ?? 0,
                    YakitTuru = Girdi.YakitTuru ?? YakitTuru.Benzin,
                    VitesTuru = Girdi.VitesTuru ?? VitesTuru.Manuel,
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
            TempData["Mesaj"] = "İlan taslak olarak kaydedildi.";
            return RedirectToPage("/Satici/Index");
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
            AracKategoriIdleri = await _context.Kategoriler
                .Where(k => k.AracKategorisiMi)
                .Select(k => k.KategoriId)
                .ToListAsync();
        }
    }
}
