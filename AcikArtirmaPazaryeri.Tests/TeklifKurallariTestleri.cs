using AcikArtirmaPazaryeri.Models;
using AcikArtirmaPazaryeri.Servisler;

namespace AcikArtirmaPazaryeri.Tests
{
    public class TeklifKurallariTestleri
    {
        private static readonly DateTime Simdi = new DateTime(2026, 11, 27, 12, 0, 0, DateTimeKind.Utc);

        // Başlangıç 10.000 ₺, artış 500 ₺, şu an devam eden bir müzayede
        private static Muzayede OrnekIlan(decimal? guncelFiyat = null)
        {
            return new Muzayede
            {
                MuzayedeId = 1,
                SaticiId = "satici",
                Durum = MuzayedeDurumu.Aktif,
                BaslangicFiyati = 10000,
                MinimumArtis = 500,
                GuncelFiyat = guncelFiyat,
                BaslangicZamani = Simdi.AddDays(-1),
                BitisZamani = Simdi.AddDays(1)
            };
        }

        [Fact]
        public void GirisYapmayan_TeklifVeremez()
        {
            var hata = TeklifServisi.KuralKontrol(OrnekIlan(), null, 10000, Simdi);
            Assert.Equal("Teklif vermek için giriş yapmalısınız.", hata);
        }

        [Fact]
        public void Satici_KendiIlaninaTeklifVeremez()
        {
            var hata = TeklifServisi.KuralKontrol(OrnekIlan(), "satici", 10000, Simdi);
            Assert.Equal("Kendi ilanınıza teklif veremezsiniz.", hata);
        }

        [Theory]
        [InlineData(MuzayedeDurumu.Taslak)]
        [InlineData(MuzayedeDurumu.OnayBekliyor)]
        [InlineData(MuzayedeDurumu.Reddedildi)]
        [InlineData(MuzayedeDurumu.Satildi)]
        public void AktifOlmayanIlana_TeklifVerilemez(MuzayedeDurumu durum)
        {
            var ilan = OrnekIlan();
            ilan.Durum = durum;
            var hata = TeklifServisi.KuralKontrol(ilan, "alici", 10000, Simdi);
            Assert.Equal("Bu ilan teklife açık değil.", hata);
        }

        [Fact]
        public void BaslamadanOnce_TeklifVerilemez()
        {
            var ilan = OrnekIlan();
            ilan.BaslangicZamani = Simdi.AddMinutes(5);
            var hata = TeklifServisi.KuralKontrol(ilan, "alici", 10000, Simdi);
            Assert.Equal("Müzayede henüz başlamadı.", hata);
        }

        [Fact]
        public void BitistenSonra_TeklifVerilemez()
        {
            var ilan = OrnekIlan();
            ilan.BitisZamani = Simdi;
            var hata = TeklifServisi.KuralKontrol(ilan, "alici", 10000, Simdi);
            Assert.Equal("Müzayede sona erdi.", hata);
        }

        [Fact]
        public void IlkTeklif_BaslangicFiyatindanAzOlamaz()
        {
            var hata = TeklifServisi.KuralKontrol(OrnekIlan(), "alici", 9999, Simdi);
            Assert.NotNull(hata);
        }

        [Fact]
        public void IlkTeklif_BaslangicFiyatinaEsitOlabilir()
        {
            var hata = TeklifServisi.KuralKontrol(OrnekIlan(), "alici", 10000, Simdi);
            Assert.Null(hata);
        }

        [Theory]
        [InlineData(12000, false)] // güncel fiyatla aynı
        [InlineData(12499, false)] // artış yetmiyor
        [InlineData(12500, true)]  // tam güncel + minimum artış
        [InlineData(20000, true)]
        public void SonrakiTeklif_GuncelFiyatArtiMinimumArtisOlmali(int tutar, bool gecerli)
        {
            var hata = TeklifServisi.KuralKontrol(OrnekIlan(guncelFiyat: 12000), "alici", tutar, Simdi);
            Assert.Equal(gecerli, hata == null);
        }

        [Fact]
        public void EnAzTeklif_TeklifYokkenBaslangicFiyati()
        {
            Assert.Equal(10000, TeklifServisi.EnAzTeklif(OrnekIlan()));
        }

        [Fact]
        public void EnAzTeklif_TeklifVarkenGuncelArtiArtis()
        {
            Assert.Equal(12500, TeklifServisi.EnAzTeklif(OrnekIlan(guncelFiyat: 12000)));
        }
    }
}
