using System.ComponentModel.DataAnnotations;
namespace AcikArtirmaPazaryeri.Models
{
    public class Marka
    {
        public int MarkaId { get; set; }
        [Required, StringLength(100)]
        public string MarkaAdi { get; set; } = string.Empty;

        public ICollection<AracModeli> Modeller { get; set; } = new List<AracModeli>();
    }
}

