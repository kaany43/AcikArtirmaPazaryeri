using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Yonetim.Kategoriler
{
    [Authorize(Roles = "Yonetici")]
    public class IndexModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        public IndexModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }
        public List<Kategori> Kategoriler { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var kategoriler = _context.Kategoriler.Include(k => k.UstKategori)
                .OrderBy(k => k.UstKategoriId ?? k.KategoriId).ThenBy(k => k.UstKategoriId.HasValue).ThenBy(k => k.KategoriAdi);

            Kategoriler = await kategoriler.ToListAsync();
            return Page();
        }
    }
}
