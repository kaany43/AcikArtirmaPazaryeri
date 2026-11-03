using System.ComponentModel.DataAnnotations;
namespace AcikArtirmaPazaryeri.Models

{
    public enum MuzayedeDurumu
    {
        [Display(Name = "Taslak")]
        Taslak = 1,
        [Display(Name = "Onay Bekliyor")]
        OnayBekliyor = 2,
        [Display(Name = "Aktif")]
        Aktif = 3,
        [Display(Name = "Reddedildi")]
        Reddedildi = 4,
        [Display(Name = "Satıldı")]

        Satildi = 5,
        [Display(Name = "Satılmadı")]
        Satilmadi = 6
    }
    public enum UrunDurumu
    {
        [Display(Name = "Sıfır")]
        Sıfır = 1,
        [Display(Name = "Az Kullanılmış")]
        AzKullanilmis = 2,
        [Display(Name = "İkinci El")]
        IkinciEl = 3
    }
    public enum YakitTuru
    {
        [Display(Name = "Benzin")]
        Benzin = 1,
        [Display(Name = "Dizel")]
        Dizel = 2,
        [Display(Name = "Elektrik")]
        Elektrik = 3,
        [Display(Name = "Hibrit")]
        Hibrit = 4,
        [Display(Name = "LPG")]
        LPG = 5,
        [Display(Name = "CNG")]
        CNG = 6
    }
    public enum VitesTuru
    {
        [Display(Name = "Manuel")]
        Manuel = 1,
        [Display(Name = "Otomatik")]
        Otomatik = 2,
        [Display(Name = "Yarı Otomatik")]
        YariOtomatik = 3
    }
    public enum KasaTipi
    {
        [Display(Name = "Sedan")]
        Sedan = 1,
        [Display(Name = "Hatchback")]
        Hatchback = 2,
        [Display(Name = "Station Wagon")]
        StationWagon = 3,
        [Display(Name = "SUV")]
        SUV = 4,
        [Display(Name = "Cabrio")]
        Cabrio = 5,
        [Display(Name = "Coupe")]
        Coupe = 6,
        [Display(Name = "Minivan")]
        Minivan = 7,
        [Display(Name = "Pickup")]
        Pickup = 8
    }
    public enum ParcaDurumu
    {
        [Display(Name = "Orijinal")]
        Orijinal = 1,
        [Display(Name = "Lokal Boyalı")]
        LokalBoyali = 2,
        [Display(Name = "Boyalı")]
        Boyali = 3,

        [Display(Name = "Değişen")]
        Degisen = 4
    }
}
