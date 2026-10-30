using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Yonetim.Kategoriler
{
    [Authorize(Roles = "Yonetici")]
    public class SilModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        public SilModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        public Kategori Kategori { get; set; } = null!;

        private async Task<Kategori?> GetirAsync(int id)
        {
            return await _context.Kategoriler
                .Include(k => k.UstKategori)
                .Include(k => k.AltKategoriler)
                .FirstOrDefaultAsync(k => k.KategoriId == id);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var kategori = await GetirAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }
            Kategori = kategori;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var kategori = await GetirAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }
            if (kategori.AltKategoriler.Any())
            {
                ModelState.AddModelError(string.Empty, "Önce bu kategorinin alt kategorilerini silmelisin.");
                Kategori = kategori;
                return Page();
            }
            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
