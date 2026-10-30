using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Yonetim.Kategoriler
{
    [Authorize(Roles = "Yonetici")]
    public class DuzenleModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        public DuzenleModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Kategori Kategori { get; set; } = new();
        public SelectList UstKategoriSecenekleri { get; set; } = null!;
        public string MevcutUstKategori { get; set; } = "—";
        public int AltKategoriSayisi { get; set; }

        private async Task SecenekleriDoldurAsync(int haricId)
        {
            var anaKategoriler = await _context.Kategoriler
                .Where(k => k.UstKategoriId == null && k.KategoriId != haricId)
                .OrderBy(k => k.KategoriAdi)
                .ToListAsync();
            UstKategoriSecenekleri = new SelectList(anaKategoriler, "KategoriId", "KategoriAdi");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var kategori = await _context.Kategoriler
                .Include(k => k.UstKategori)
                .Include(k => k.AltKategoriler)
                .FirstOrDefaultAsync(k => k.KategoriId == id);
            if (kategori == null)
            {
                return NotFound();
            }
            Kategori = kategori;
            MevcutUstKategori = kategori.UstKategori?.KategoriAdi ?? "—";
            AltKategoriSayisi = kategori.AltKategoriler.Count;
            await SecenekleriDoldurAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var mevcut = await _context.Kategoriler
                .Include(k => k.AltKategoriler)
                .FirstOrDefaultAsync(k => k.KategoriId == id);
            if (mevcut == null)
            {
                return NotFound();
            }
            Kategori.KategoriAdi = Kategori.KategoriAdi?.Trim() ?? string.Empty;
            if (Kategori.UstKategoriId == id)
            {
                ModelState.AddModelError("Kategori.UstKategoriId", "Bir kategori kendisinin üst kategorisi olamaz.");
            }
            else if (Kategori.UstKategoriId != null && mevcut.AltKategoriler.Any())
            {
                ModelState.AddModelError("Kategori.UstKategoriId", "Alt kategorisi olan bir kategori başka bir kategorinin altına taşınamaz.");
            }
            var ayniIsimVar = await _context.Kategoriler
                .AnyAsync(k => k.KategoriId != id && k.KategoriAdi == Kategori.KategoriAdi && k.UstKategoriId == Kategori.UstKategoriId);
            if (ayniIsimVar)
            {
                ModelState.AddModelError("Kategori.KategoriAdi", "Bu isimde bir kategori zaten var.");
            }
            if (!ModelState.IsValid)
            {
                Kategori.KategoriId = id;
                MevcutUstKategori = await _context.Kategoriler
                    .Where(k => k.KategoriId == mevcut.UstKategoriId)
                    .Select(k => k.KategoriAdi)
                    .FirstOrDefaultAsync() ?? "—";
                AltKategoriSayisi = mevcut.AltKategoriler.Count;
                await SecenekleriDoldurAsync(id);
                return Page();
            }
            mevcut.KategoriAdi = Kategori.KategoriAdi;
            mevcut.UstKategoriId = Kategori.UstKategoriId;
            mevcut.AracKategorisiMi = Kategori.AracKategorisiMi;
            mevcut.OtomatikOnayAcikMi = Kategori.OtomatikOnayAcikMi;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
