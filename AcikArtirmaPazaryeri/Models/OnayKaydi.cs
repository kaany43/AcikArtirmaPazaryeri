using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class OnayKaydi
    {
        public int OnayKaydiId { get; set; }

        public int MuzayedeId { get; set; }
        public Muzayede Muzayede { get; set; } = null!;

        public OnaySonucu Sonuc { get; set; }

        [StringLength(500)]
        public string? Gerekce { get; set; }

        public bool OtomatikMi { get; set; }

        // Manuel onayda işlemi yapan yönetici
        public string? YoneticiId { get; set; }
        public Kullanici? Yonetici { get; set; }

        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }
}
