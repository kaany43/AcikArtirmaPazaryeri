using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AcikArtirmaPazaryeri.Models
{
    public class Kullanici : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Soyad { get; set; } = string.Empty;

        public string ? FirmaAdi { get; set; }

        public bool GuvenilirSaticiMi { get; set; }

    }
}
