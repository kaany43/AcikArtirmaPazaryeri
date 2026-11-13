using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Yonetim.Onaylar
{
    [Authorize(Roles = "Yonetici")]
    public class IndexModel : PageModel
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public IndexModel(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Muzayede> Bekleyenler { get; set; } = new();

        public async Task OnGetAsync()
        {
            Bekleyenler = await _context.Muzayedeler
                .Where(m => m.Durum == MuzayedeDurumu.OnayBekliyor)
                .Include(m => m.Kategori)
                    .ThenInclude(k => k.UstKategori)
                .Include(m => m.Satici)
                .OrderBy(m => m.OlusturmaZamani) // ilk gelen ilk onaylanır
                .ToListAsync();
        }

        // OnayBekliyor → Aktif
        public async Task<IActionResult> OnPostOnaylaAsync(int id)
        {
            var muzayede = await _context.Muzayedeler.FindAsync(id);
            if (muzayede == null)
            {
                return NotFound();
            }
            if (muzayede.Durum != MuzayedeDurumu.OnayBekliyor)
            {
                TempData["Mesaj"] = "Bu ilan onay beklemiyor.";
                return RedirectToPage();
            }

            muzayede.Durum = MuzayedeDurumu.Aktif;
            _context.OnayKayitlari.Add(new OnayKaydi
            {
                MuzayedeId = muzayede.MuzayedeId,
                Sonuc = OnaySonucu.Onaylandi,
                OtomatikMi = false,
                YoneticiId = _userManager.GetUserId(User)
            });
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{muzayede.Baslik}\" onaylandı ve yayına alındı.";
            return RedirectToPage();
        }

        // OnayBekliyor → Reddedildi (gerekçe zorunlu)
        public async Task<IActionResult> OnPostReddetAsync(int id, string? gerekce)
        {
            var muzayede = await _context.Muzayedeler.FindAsync(id);
            if (muzayede == null)
            {
                return NotFound();
            }
            if (muzayede.Durum != MuzayedeDurumu.OnayBekliyor)
            {
                TempData["Mesaj"] = "Bu ilan onay beklemiyor.";
                return RedirectToPage();
            }
            gerekce = gerekce?.Trim();
            if (string.IsNullOrEmpty(gerekce))
            {
                TempData["Mesaj"] = "Reddetmek için gerekçe yazın.";
                return RedirectToPage();
            }
            if (gerekce.Length > 500)
            {
                gerekce = gerekce.Substring(0, 500);
            }

            muzayede.Durum = MuzayedeDurumu.Reddedildi;
            _context.OnayKayitlari.Add(new OnayKaydi
            {
                MuzayedeId = muzayede.MuzayedeId,
                Sonuc = OnaySonucu.Reddedildi,
                Gerekce = gerekce,
                OtomatikMi = false,
                YoneticiId = _userManager.GetUserId(User)
            });
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{muzayede.Baslik}\" reddedildi.";
            return RedirectToPage();
        }
    }
}
