using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity;

namespace AcikArtirmaPazaryeri.Pages.Hesap
{
    public class GirisModel : PageModel
    {
        private readonly SignInManager<Kullanici> _signInManager;
        public GirisModel(SignInManager<Kullanici> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public class GirdiModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "E-posta")]
            public string Eposta { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Sifre { get; set; } = string.Empty;
            [Display(Name = "Beni Hatırla")]
            public bool BeniHatirla { get; set; } = false;
        }
        [BindProperty] public GirdiModel Girdi { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var sonuc = await _signInManager.PasswordSignInAsync(Girdi.Eposta, Girdi.Sifre, Girdi.BeniHatirla, lockoutOnFailure: false);
            if (sonuc.Succeeded)
            {
                return LocalRedirect(ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                return Page();
            }
        }
    }
}
