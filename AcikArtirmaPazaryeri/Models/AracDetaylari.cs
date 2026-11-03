using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class AracDetaylari
    {
        public int AracDetaylariId { get; set; }
        public int MuzayedeId { get; set; }
        public Muzayede Muzayede { get; set; } = null!;
        [Required, StringLength(17, MinimumLength = 17)]
        public string SasiNo { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Plaka { get; set; }
        public int AracModeliId { get; set; }
        public AracModeli AracModeli { get; set; } = null!;
        public int Yil { get; set; }
        public int Km { get; set; }
        [Required, StringLength(50)]
        public string Renk { get; set; } = string.Empty;
        public YakitTuru YakitTuru { get; set; }
        public VitesTuru VitesTuru { get; set; }
        public KasaTipi KasaTipi { get; set; }

        public string? Paket { get; set; }

        public ICollection<EkspertizKalemi> EkspertizKalemleri { get; set; } = new List<EkspertizKalemi>();
    }
}
