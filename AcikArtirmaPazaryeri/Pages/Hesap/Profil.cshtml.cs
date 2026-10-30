using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity;

namespace AcikArtirmaPazaryeri.Pages.Hesap
{
    [Authorize]
    public class ProfilModel : PageModel
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;
        public ProfilModel(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Kullanici AktifKullanici { get; set; } = null!;
        public bool SaticiMi { get; set; } = false;
        [BindProperty] public string? FirmaAdi { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var kullanici = await _userManager.GetUserAsync(User);
            if (kullanici == null)
            {
                return Challenge();
            }
            AktifKullanici = kullanici;
            SaticiMi = await _userManager.IsInRoleAsync(AktifKullanici, "Satici");
            return Page();
        }

        public async Task<IActionResult> OnPostSaticiOlAsync()
        {
            var kullanici = await _userManager.GetUserAsync(User);
            if (kullanici == null)
            {
                return Challenge();
            }
            if (!await _userManager.IsInRoleAsync(kullanici, "Satici"))
            {
                if (!string.IsNullOrWhiteSpace(FirmaAdi))
                {
                    kullanici.FirmaAdi = FirmaAdi.Trim();
                    await _userManager.UpdateAsync(kullanici);
                }
                await _userManager.AddToRoleAsync(kullanici, "Satici");
                await _signInManager.RefreshSignInAsync(kullanici);
            }
            return RedirectToPage();
        }
    }
}
