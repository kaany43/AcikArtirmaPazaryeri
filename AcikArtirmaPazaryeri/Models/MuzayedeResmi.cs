using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class MuzayedeResmi
    {
        public int MuzayedeResmiId { get; set; }
        public int MuzayedeId { get; set; }
        public Muzayede Muzayede { get; set; } = null!;
        [Required, StringLength(200)]
        public string DosyaYolu { get; set; } = string.Empty;
        public int Sira { get; set; }

        public bool KapakMi { get; set; }
    }
}
