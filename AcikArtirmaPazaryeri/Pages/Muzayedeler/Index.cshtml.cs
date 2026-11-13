using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Muzayedeler
{
    public class IndexModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;

        public IndexModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        public const int SayfaBoyutu = 12;

        // Filtreler adres çubuğundan (?KategoriId=3&Sayfa=2) gelir.
        [BindProperty(SupportsGet = true)] public string? Arama { get; set; }
        [BindProperty(SupportsGet = true)] public int? KategoriId { get; set; }
        [BindProperty(SupportsGet = true)] public int? MinFiyat { get; set; }
        [BindProperty(SupportsGet = true)] public int? MaxFiyat { get; set; }
        [BindProperty(SupportsGet = true)] public string? Sehir { get; set; }
        [BindProperty(SupportsGet = true)] public UrunDurumu? UrunDurumu { get; set; }

        // Araç filtreleri
        [BindProperty(SupportsGet = true)] public int? MarkaId { get; set; }
        [BindProperty(SupportsGet = true)] public int? AracModeliId { get; set; }
        [BindProperty(SupportsGet = true)] public int? MinYil { get; set; }
        [BindProperty(SupportsGet = true)] public int? MaxYil { get; set; }
        [BindProperty(SupportsGet = true)] public int? MinKm { get; set; }
        [BindProperty(SupportsGet = true)] public int? MaxKm { get; set; }
        [BindProperty(SupportsGet = true)] public YakitTuru? YakitTuru { get; set; }
        [BindProperty(SupportsGet = true)] public VitesTuru? VitesTuru { get; set; }

        [BindProperty(SupportsGet = true)] public int Sayfa { get; set; } = 1;

        public List<Muzayede> Ilanlar { get; set; } = new();
        public int ToplamIlan { get; set; }
        public int ToplamSayfa { get; set; }
        public Kategori? SeciliKategori { get; set; }
        public bool AracFiltreleriAcik { get; set; }

        public List<SelectListItem> Kategoriler { get; set; } = new();
        public List<SelectListItem> Sehirler { get; set; } = new();
        public List<SelectListItem> Markalar { get; set; } = new();
        public List<SelectListItem> Modeller { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Ziyaretçiler sadece yayındaki (Aktif) ilanları görür.
            var sorgu = _context.Muzayedeler
                .Where(m => m.Durum == MuzayedeDurumu.Aktif);

            if (KategoriId != null)
            {
                SeciliKategori = await _context.Kategoriler.FindAsync(KategoriId);
                // Ana kategori seçilince alt kategorilerin ilanları da gelir.
                sorgu = sorgu.Where(m => m.KategoriId == KategoriId || m.Kategori.UstKategoriId == KategoriId);
            }
            AracFiltreleriAcik = SeciliKategori?.AracKategorisiMi == true;

            if (!string.IsNullOrWhiteSpace(Arama))
            {
                var aranan = $"%{Arama.Trim()}%";
                sorgu = sorgu.Where(m => EF.Functions.ILike(m.Baslik, aranan) || EF.Functions.ILike(m.Aciklama, aranan));
            }
            // 5. haftada teklifler gelince fiyat filtresi güncel fiyata göre yapılacak.
            if (MinFiyat != null)
            {
                sorgu = sorgu.Where(m => m.BaslangicFiyati >= MinFiyat);
            }
            if (MaxFiyat != null)
            {
                sorgu = sorgu.Where(m => m.BaslangicFiyati <= MaxFiyat);
            }
            if (!string.IsNullOrWhiteSpace(Sehir))
            {
                sorgu = sorgu.Where(m => m.Sehir == Sehir);
            }
            if (UrunDurumu != null)
            {
                sorgu = sorgu.Where(m => m.UrunDurumu == UrunDurumu);
            }

            if (AracFiltreleriAcik)
            {
                if (MarkaId != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.AracModeli.MarkaId == MarkaId);
                }
                if (AracModeliId != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.AracModeliId == AracModeliId);
                }
                if (MinYil != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.Yil >= MinYil);
                }
                if (MaxYil != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.Yil <= MaxYil);
                }
                if (MinKm != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.Km >= MinKm);
                }
                if (MaxKm != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.Km <= MaxKm);
                }
                if (YakitTuru != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.YakitTuru == YakitTuru);
                }
                if (VitesTuru != null)
                {
                    sorgu = sorgu.Where(m => m.AracDetaylari!.VitesTuru == VitesTuru);
                }
            }

            ToplamIlan = await sorgu.CountAsync();
            ToplamSayfa = Math.Max(1, (int)Math.Ceiling(ToplamIlan / (double)SayfaBoyutu));
            Sayfa = Math.Clamp(Sayfa, 1, ToplamSayfa);

            Ilanlar = await sorgu
                .Include(m => m.Kategori)
                    .ThenInclude(k => k.UstKategori)
                .Include(m => m.MuzayedeResimleri)
                .Include(m => m.AracDetaylari)
                    .ThenInclude(a => a!.AracModeli)
                        .ThenInclude(am => am.Marka)
                .OrderBy(m => m.BitisZamani) // bitişi en yakın olan önce
                .Skip((Sayfa - 1) * SayfaBoyutu)
                .Take(SayfaBoyutu)
                .ToListAsync();

            await ListeleriDoldurAsync();
        }

        private async Task ListeleriDoldurAsync()
        {
            var kategoriler = await _context.Kategoriler
                .Include(k => k.AltKategoriler)
                .Where(k => k.UstKategoriId == null)
                .OrderBy(k => k.KategoriId)
                .ToListAsync();
            foreach (var ust in kategoriler)
            {
                Kategoriler.Add(new SelectListItem(ust.KategoriAdi, ust.KategoriId.ToString()));
                foreach (var alt in ust.AltKategoriler.OrderBy(a => a.KategoriId))
                {
                    Kategoriler.Add(new SelectListItem($"{ust.KategoriAdi} › {alt.KategoriAdi}", alt.KategoriId.ToString()));
                }
            }

            Sehirler = await _context.Muzayedeler
                .Where(m => m.Durum == MuzayedeDurumu.Aktif)
                .Select(m => m.Sehir)
                .Distinct()
                .OrderBy(s => s)
                .Select(s => new SelectListItem(s, s))
                .ToListAsync();

            if (AracFiltreleriAcik)
            {
                Markalar = await _context.Markalar
                    .OrderBy(m => m.MarkaAdi)
                    .Select(m => new SelectListItem(m.MarkaAdi, m.MarkaId.ToString()))
                    .ToListAsync();
                if (MarkaId != null)
                {
                    Modeller = await _context.AracModelleri
                        .Where(m => m.MarkaId == MarkaId)
                        .OrderBy(m => m.ModelAdi)
                        .Select(m => new SelectListItem(m.ModelAdi, m.AracModeliId.ToString()))
                        .ToListAsync();
                }
            }
        }
    }
}
