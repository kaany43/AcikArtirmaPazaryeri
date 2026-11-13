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
                    .ThenInclude(k => k.UstKategori) // tasarımda "Vasıta › Otomobil" gibi gösteriliyor
                .Include(m => m.OnayKayitlari) // reddedilen ilanın gerekçesi için
                .OrderByDescending(m => m.OlusturmaZamani)
                .ToListAsync();
        }

        // Taslak → OnayBekliyor: ilan yöneticinin onay kuyruğuna düşer.
        public async Task<IActionResult> OnPostOnayaGonderAsync(int id)
        {
            var saticiId = _userManager.GetUserId(User);
            var muzayede = await _context.Muzayedeler.FirstOrDefaultAsync(m => m.MuzayedeId == id && m.SaticiId == saticiId);
            if (muzayede == null)
            {
                return NotFound();
            }
            if (muzayede.Durum != MuzayedeDurumu.Taslak)
            {
                TempData["Mesaj"] = "Sadece taslak ilanlar onaya gönderilebilir.";
                return RedirectToPage();
            }

            muzayede.Durum = MuzayedeDurumu.OnayBekliyor;
            _context.OnayKayitlari.Add(new OnayKaydi
            {
                MuzayedeId = muzayede.MuzayedeId,
                Sonuc = OnaySonucu.KuyrugaAlindi,
                Gerekce = "Satıcı ilanı onaya gönderdi.",
                OtomatikMi = false
            });
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "İlan onaya gönderildi.";
            return RedirectToPage();
        }
    }
}
