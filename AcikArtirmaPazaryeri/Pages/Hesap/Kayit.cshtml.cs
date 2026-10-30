using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Pages.Hesap
{
    public class KayitModel : PageModel
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;
        public KayitModel(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager)
        { _userManager = userManager; _signInManager = signInManager; }
        public class GirdiModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "E-posta")]
            public string Eposta { get; set; } = string.Empty;
            [Required]
            [StringLength(100, MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Sifre { get; set; } = string.Empty;
            [DataType(DataType.Password)]
            [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor.")]
            [Display(Name = "Şifre (Tekrar)")]
            public string SifreTekrar { get; set; } = string.Empty;
            [Required]
            [StringLength(100)]
            [Display(Name = "Ad")]
            public string Ad { get; set; } = string.Empty;
            [Required]
            [StringLength(100)]
            [Display(Name = "Soyad")]
            public string Soyad { get; set; } = string.Empty;
        }
        [BindProperty] public GirdiModel Girdi { get; set; } = new();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var kullanici = new Kullanici
            {
                UserName = Girdi.Eposta,
                Email = Girdi.Eposta,
                Ad = Girdi.Ad,
                Soyad = Girdi.Soyad
            };

            var sonuc = await _userManager.CreateAsync(kullanici, Girdi.Sifre);
            if (sonuc.Succeeded)
            {
                await _userManager.AddToRoleAsync(kullanici, "Alici");
                await _signInManager.SignInAsync(kullanici, isPersistent: false);
                return RedirectToPage("/Index");
            }

            foreach (var hata in sonuc.Errors)
            {
                ModelState.AddModelError(string.Empty, hata.Description);
            }
            return Page();
        }
    }
}
