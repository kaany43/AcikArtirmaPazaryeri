using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;

        // DI, Program.cs'de kaydettigimiz DbContext'i buraya veriyor
        public IndexModel(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        public List<Kategori> UstKategoriler { get; set; } = new();

        public void OnGet()
        {
            UstKategoriler = _context.Kategoriler
                .Where(k => k.UstKategoriId == null)   // sadece ana kategoriler
                .Include(k => k.AltKategoriler)        // alt kategorileri de yukle
                .OrderBy(k => k.KategoriId)
                .ToList();                             // sorgu burada calisir
        }
    }
}
