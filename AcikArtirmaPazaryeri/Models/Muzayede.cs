using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class Muzayede
    {
        public int MuzayedeId { get; set; }

        [Required, StringLength(100)]
        public string Baslik { get; set; } = string.Empty;

        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; } = null!;

        [Required]
        public string SaticiId { get; set; } = string.Empty;
        public Kullanici Satici { get; set; } = null!;

        public decimal BaslangicFiyati { get; set; }
        public decimal? RezervFiyati { get; set; }

        public MuzayedeDurumu Durum { get; set; } = MuzayedeDurumu.Taslak;

        [Required]
        public string Aciklama { get; set; } = string.Empty;

        public decimal MinimumArtis { get; set; }

        public DateTime BaslangicZamani { get; set; }

        public DateTime BitisZamani { get; set; }

        public DateTime OlusturmaZamani { get; set; } = DateTime.UtcNow;

        public UrunDurumu UrunDurumu { get; set; }

        [Required, StringLength(100)]
        public string Sehir { get; set; } = string.Empty;

        public AracDetaylari? AracDetaylari { get; set; }

        public ICollection<MuzayedeResmi> MuzayedeResimleri { get; set; } = new List<MuzayedeResmi>();
    }
}
