using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Muzayedeler
{
    public class DetayModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public DetayModel(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Muzayede Ilan { get; set; } = null!;
        public List<MuzayedeResmi> Resimler { get; set; } = new();
        public List<Teklif> Teklifler { get; set; } = new();
        public string? KullaniciId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ilan = await _context.Muzayedeler
                .Include(m => m.Kategori)
                    .ThenInclude(k => k.UstKategori)
                .Include(m => m.Satici)
                .Include(m => m.MuzayedeResimleri)
                .Include(m => m.AracDetaylari)
                    .ThenInclude(a => a!.AracModeli)
                        .ThenInclude(am => am.Marka)
                .Include(m => m.AracDetaylari)
                    .ThenInclude(a => a!.EkspertizKalemleri)
                .Include(m => m.Teklifler)
                    .ThenInclude(t => t.Kullanici)
                .FirstOrDefaultAsync(m => m.MuzayedeId == id);
            if (ilan == null)
            {
                return NotFound();
            }

            KullaniciId = _userManager.GetUserId(User);

            // Yayında olmayan ilanı sadece sahibi ve yönetici görebilir (onaydan önce kontrol için).
            if (ilan.Durum != MuzayedeDurumu.Aktif
                && ilan.SaticiId != KullaniciId
                && !User.IsInRole("Yonetici"))
            {
                return NotFound();
            }

            Ilan = ilan;
            Resimler = ilan.MuzayedeResimleri.OrderBy(r => r.Sira).ToList();
            Teklifler = ilan.Teklifler.OrderByDescending(t => t.Tutar).ToList();
            return Page();
        }
    }
}
