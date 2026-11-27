using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class Teklif
    {
        public int TeklifId { get; set; }

        public int MuzayedeId { get; set; }
        public Muzayede Muzayede { get; set; } = null!;

        [Required]
        public string KullaniciId { get; set; } = string.Empty;
        public Kullanici Kullanici { get; set; } = null!;

        public decimal Tutar { get; set; }

        public DateTime Zaman { get; set; } = DateTime.UtcNow;
    }
}
