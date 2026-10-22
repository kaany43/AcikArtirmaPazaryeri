using System.ComponentModel.DataAnnotations;

namespace AcikArtirmaPazaryeri.Models
{
    public class Kategori
    {
        [Required]
        public int KategoriId { get; set; }

        [Required, StringLength(100)]
        public string KategoriAdi { get; set; } = string.Empty;

        public int? UstKategoriId { get; set; }

        public bool AracKategorisiMi { get; set; }

        public bool OtomatikOnayAcikMi { get; set; }

        public virtual Kategori? UstKategori { get; set; }

        public ICollection<Kategori> AltKategoriler { get; set; } = new List<Kategori>();



    }
}
