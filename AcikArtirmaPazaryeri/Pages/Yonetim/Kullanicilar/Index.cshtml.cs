using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Pages.Yonetim.Kullanicilar
{
    [Authorize(Roles = "Yonetici")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<Kullanici> _userManager;
        public IndexModel(UserManager<Kullanici> userManager)
        {
            _userManager = userManager;
        }

        public class KullaniciSatiri
        {
            public Kullanici Kullanici { get; set; } = null!;
            public IList<string> Roller { get; set; } = new List<string>();
        }

        public List<KullaniciSatiri> Satirlar { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var kullanicilar = await _userManager.Users.OrderBy(k => k.Ad).ThenBy(k => k.Soyad).ToListAsync();

            foreach (var kullanici in kullanicilar)
            {
                Satirlar.Add(new KullaniciSatiri { Kullanici = kullanici, Roller = await _userManager.GetRolesAsync(kullanici) });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGuvenilirAsync(string id)
        {
            var kullanici = await _userManager.FindByIdAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(kullanici, "Satici"))
            {
                TempData["Mesaj"] = "Sadece satıcılar güvenilir olarak işaretlenebilir.";
                return RedirectToPage();
            }

            kullanici.GuvenilirSaticiMi = !kullanici.GuvenilirSaticiMi;
            await _userManager.UpdateAsync(kullanici);

            TempData["Mesaj"] = kullanici.GuvenilirSaticiMi
                ? $"{kullanici.Ad} {kullanici.Soyad} artık güvenilir satıcı."
                : $"{kullanici.Ad} {kullanici.Soyad} güvenilir satıcı listesinden çıkarıldı.";
            return RedirectToPage();
        }
    }
}
