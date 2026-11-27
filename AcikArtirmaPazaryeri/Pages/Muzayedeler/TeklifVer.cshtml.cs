using AcikArtirmaPazaryeri.Models;
using AcikArtirmaPazaryeri.Servisler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace AcikArtirmaPazaryeri.Pages.Muzayedeler
{
    // Detay sayfasındaki teklif formu buraya gönderilir. Kendi sayfası yok, sonuç detay sayfasında gösterilir.
    [Authorize]
    [EnableRateLimiting("teklif")]
    public class TeklifVerModel : PageModel
    {
        private readonly TeklifServisi _teklifServisi;
        private readonly UserManager<Kullanici> _userManager;

        public TeklifVerModel(TeklifServisi teklifServisi, UserManager<Kullanici> userManager)
        {
            _teklifServisi = teklifServisi;
            _userManager = userManager;
        }

        public IActionResult OnGet(int id)
        {
            return RedirectToPage("/Muzayedeler/Detay", new { id });
        }

        public async Task<IActionResult> OnPostAsync(int id, int tutar)
        {
            var sonuc = await _teklifServisi.TeklifVerAsync(id, _userManager.GetUserId(User), tutar);
            TempData["TeklifMesaj"] = sonuc.Mesaj;
            TempData["TeklifBasarili"] = sonuc.Basarili;
            return RedirectToPage("/Muzayedeler/Detay", new { id });
        }
    }
}
