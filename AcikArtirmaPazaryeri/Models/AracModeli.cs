using System.ComponentModel.DataAnnotations;
namespace AcikArtirmaPazaryeri.Models
{
    public class AracModeli
    {
        public int AracModeliId { get; set; }
        [Required, StringLength(100)]
        public string ModelAdi { get; set; } = string.Empty;

        public int MarkaId { get; set; }

        public virtual Marka Marka { get; set; } = null!;
    }
}
