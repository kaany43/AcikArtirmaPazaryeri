using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class EkspertizKalemi
    {
        public int EkspertizKalemiId { get; set; }

        public int AracDetaylariId { get; set; }

        public AracDetaylari AracDetaylari { get; set; } = null!;

        [Required, StringLength(100)]
        public string Parca { get; set; } = string.Empty;

        public ParcaDurumu Durum { get; set; }

    }
}
