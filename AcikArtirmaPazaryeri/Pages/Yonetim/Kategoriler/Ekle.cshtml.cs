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
    public class EkleModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        public EkleModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Kategori Kategori { get; set; } = new();
        public SelectList UstKategoriSecenekleri { get; set; } = null!;

        private async Task SecenekleriDoldurAsync()
        {
            var anaKategoriler = await _context.Kategoriler
                .Where(k => k.UstKategoriId == null)
                .OrderBy(k => k.KategoriAdi)
                .ToListAsync();
            UstKategoriSecenekleri = new SelectList(anaKategoriler, "KategoriId", "KategoriAdi");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await SecenekleriDoldurAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Kategori.KategoriAdi = Kategori.KategoriAdi?.Trim() ?? string.Empty;
            var ayniIsimVar = await _context.Kategoriler
                .AnyAsync(k => k.KategoriAdi == Kategori.KategoriAdi && k.UstKategoriId == Kategori.UstKategoriId);
            if (ayniIsimVar)
            {
                ModelState.AddModelError("Kategori.KategoriAdi", "Bu isimde bir kategori zaten var.");
            }
            if (!ModelState.IsValid)
            {
                await SecenekleriDoldurAsync();
                return Page();
            }
            _context.Kategoriler.Add(Kategori);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
