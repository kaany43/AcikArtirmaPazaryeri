using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.EntityFrameworkCore;


namespace AcikArtirmaPazaryeri.Pages.Satici
{
    [Authorize(Roles = "Satici")]
    public class IndexModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public IndexModel(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Muzayede> Ilanlar { get; set; } = new();
        public async Task OnGetAsync()
        {
            var saticiId = _userManager.GetUserId(User);
            Ilanlar = await _context.Muzayedeler
                .Where(m => m.SaticiId == saticiId)
                .Include(m => m.Kategori)
                .OrderByDescending(m => m.OlusturmaZamani)
                .ToListAsync();
        }
    }
}
