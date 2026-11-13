using AcikArtirmaPazaryeri.Models;
using AcikArtirmaPazaryeri.Pages.Satici;

namespace AcikArtirmaPazaryeri.Data
{
    // Örnek ilan verisi: liste, filtre ve detay sayfaları boş görünmesin diye.
    // Fotoğraflar wwwroot/ornek-ilanlar klasöründe (kaynaklar KAYNAKLAR.md'de).
    public static class OrnekIlanSeed
    {
        public static List<Muzayede> Getir(string saticiId, AcikArtirmaPazaryeriDbContext context)
        {
            var kategoriler = context.Kategoriler.ToList();
            var modeller = context.AracModelleri.ToList();
            var markalar = context.Markalar.ToList();
            var simdi = DateTime.UtcNow;

            int Kategori(string ad) => kategoriler.First(k => k.KategoriAdi == ad).KategoriId;
            int Model(string marka, string model)
            {
                var markaId = markalar.First(m => m.MarkaAdi == marka).MarkaId;
                return modeller.First(m => m.MarkaId == markaId && m.ModelAdi == model).AracModeliId;
            }

            Muzayede Ilan(string baslik, string aciklama, string kategori, string sehir, UrunDurumu urunDurumu,
                decimal baslangic, decimal artis, decimal? rezerv, TimeSpan kalanSure, params string[] resimler)
            {
                return new Muzayede
                {
                    Baslik = baslik,
                    Aciklama = aciklama,
                    KategoriId = Kategori(kategori),
                    SaticiId = saticiId,
                    Sehir = sehir,
                    UrunDurumu = urunDurumu,
                    BaslangicFiyati = baslangic,
                    MinimumArtis = artis,
                    RezervFiyati = rezerv,
                    BaslangicZamani = simdi.AddHours(-2),
                    BitisZamani = simdi.Add(kalanSure),
                    Durum = MuzayedeDurumu.Aktif,
                    MuzayedeResimleri = resimler.Select((r, i) => new MuzayedeResmi
                    {
                        DosyaYolu = $"/ornek-ilanlar/{r}",
                        Sira = i,
                        KapakMi = i == 0
                    }).ToList()
                };
            }

            // degisenler: ekspertizde orijinal olmayan parçalar, diğerleri orijinal.
            AracDetaylari Arac(string marka, string model, string paket, int yil, int km, YakitTuru yakit, VitesTuru vites,
                KasaTipi kasa, string renk, string sasiNo, Dictionary<string, ParcaDurumu>? degisenler = null)
            {
                return new AracDetaylari
                {
                    AracModeliId = Model(marka, model),
                    Paket = paket,
                    Yil = yil,
                    Km = km,
                    YakitTuru = yakit,
                    VitesTuru = vites,
                    KasaTipi = kasa,
                    Renk = renk,
                    SasiNo = sasiNo,
                    EkspertizKalemleri = EkleModel.Parcalar.Select(p => new EkspertizKalemi
                    {
                        Parca = p,
                        Durum = degisenler != null && degisenler.TryGetValue(p, out var d) ? d : ParcaDurumu.Orijinal
                    }).ToList()
                };
            }

            var ilanlar = new List<Muzayede>();

            var egea = Ilan("2019 Fiat Egea 1.4 Fire Urban", "İlk sahibinden, bakımları yetkili serviste yapıldı. Sağ arka kapıda lokal boya var, başka işlem yok. Kışlık lastikler hediye.",
                "Otomobil", "Ankara", UrunDurumu.IkinciEl, 485000, 5000, 540000, TimeSpan.FromDays(3), "egea-1.jpg", "egea-2.jpg");
            egea.AracDetaylari = Arac("Fiat", "Egea", "1.4 Fire Urban", 2019, 98500, YakitTuru.Benzin, VitesTuru.Manuel, KasaTipi.Sedan, "Bej", "ZFA35600006K21457",
                new() { ["Sağ arka kapı"] = ParcaDurumu.LokalBoyali });
            ilanlar.Add(egea);

            var clio = Ilan("2017 Renault Clio 1.5 dCi Touch", "Şehir içi kullanıldı, yakıt tüketimi çok düşük. Ön tampon boyalı, tramer kaydı 4.200 ₺. Triger seti 90.000 km'de değişti.",
                "Otomobil", "İzmir", UrunDurumu.IkinciEl, 520000, 5000, null, TimeSpan.FromDays(5), "clio-1.jpg", "clio-2.jpg");
            clio.AracDetaylari = Arac("Renault", "Clio", "1.5 dCi Touch", 2017, 142000, YakitTuru.Dizel, VitesTuru.Manuel, KasaTipi.Hatchback, "Kırmızı", "VF15R0J0H57834612",
                new() { ["Ön tampon"] = ParcaDurumu.Boyali, ["Kaput"] = ParcaDurumu.LokalBoyali });
            ilanlar.Add(clio);

            var golf = Ilan("2020 Volkswagen Golf 1.5 TSI Style", "Hatasız, boyasız. Dijital gösterge, adaptif hız sabitleyici, geri görüş kamerası. Garantisi yeni bitti.",
                "Otomobil", "İstanbul", UrunDurumu.IkinciEl, 1150000, 10000, 1300000, TimeSpan.FromDays(7), "golf-1.jpg", "golf-2.jpg");
            golf.AracDetaylari = Arac("Volkswagen", "Golf", "1.5 TSI Style", 2020, 61000, YakitTuru.Benzin, VitesTuru.YariOtomatik, KasaTipi.Hatchback, "Gri", "WVWZZZCDZLW048213");
            ilanlar.Add(golf);

            var corolla = Ilan("2021 Toyota Corolla 1.5 Vision", "Aile aracı, uzun yolda kullanıldı. Bagaj kapağı değişti (park kazası), şasi ve airbag sağlam. Ekspertiz raporu ilanda.",
                "Otomobil", "Bursa", UrunDurumu.IkinciEl, 890000, 7500, 980000, TimeSpan.FromHours(5), "corolla-1.jpg", "corolla-2.jpg");
            corolla.AracDetaylari = Arac("Toyota", "Corolla", "1.5 Vision", 2021, 74000, YakitTuru.Benzin, VitesTuru.Otomatik, KasaTipi.Sedan, "Siyah", "NMTBA3BE30R118745",
                new() { ["Bagaj"] = ParcaDurumu.Degisen, ["Arka tampon"] = ParcaDurumu.Boyali });
            ilanlar.Add(corolla);

            var tucson = Ilan("2018 Hyundai Tucson 1.6 CRDi Elite", "4x2, panoramik cam tavan, deri koltuk. Sol taraf kapılarda boya var. Lastikler yeni.",
                "Arazi, SUV & Pickup", "Antalya", UrunDurumu.IkinciEl, 1050000, 10000, null, TimeSpan.FromDays(4), "tucson-1.jpg", "tucson-2.jpg");
            tucson.AracDetaylari = Arac("Hyundai", "Tucson", "1.6 CRDi Elite", 2018, 118000, YakitTuru.Dizel, VitesTuru.Otomatik, KasaTipi.SUV, "Mavi", "NLHJ381AAJZ420973",
                new() { ["Sol ön kapı"] = ParcaDurumu.Boyali, ["Sol arka kapı"] = ParcaDurumu.Boyali });
            ilanlar.Add(tucson);

            var tesla = Ilan("2022 Tesla Model 3 Long Range", "Çift motor, 4x4. Ev şarj ünitesi ile birlikte. Yazılım güncel, batarya sağlığı %94.",
                "Elektrikli Araçlar", "İstanbul", UrunDurumu.IkinciEl, 1750000, 15000, 1950000, TimeSpan.FromDays(9), "tesla-1.jpg", "tesla-2.jpg");
            tesla.AracDetaylari = Arac("Tesla", "Model 3", "Long Range AWD", 2022, 38000, YakitTuru.Elektrik, VitesTuru.Otomatik, KasaTipi.Sedan, "Beyaz", "5YJ3E7EB4NF184562");
            ilanlar.Add(tesla);

            var focus = Ilan("2015 Ford Focus 1.6 TDCi - Hasarlı", "Önden çarpma: kaput, ön tampon ve sağ ön çamurluk değişecek. Motor ve şanzıman sağlam, araç çalışır durumda. Çekici ile teslim.",
                "Hasarlı Araçlar", "Kocaeli", UrunDurumu.IkinciEl, 210000, 2500, null, TimeSpan.FromDays(2), "focus-1.jpg", "focus-2.jpg");
            focus.AracDetaylari = Arac("Ford", "Focus", "1.6 TDCi Trend X", 2015, 186000, YakitTuru.Dizel, VitesTuru.Manuel, KasaTipi.Hatchback, "Gümüş", "WF05XXGCC5FU71836",
                new() { ["Kaput"] = ParcaDurumu.Degisen, ["Ön tampon"] = ParcaDurumu.Degisen, ["Sağ ön çamurluk"] = ParcaDurumu.Degisen, ["Sol ön çamurluk"] = ParcaDurumu.Boyali });
            ilanlar.Add(focus);

            ilanlar.Add(Ilan("MacBook Air M2 13\" 8 GB / 256 GB", "2023'te alındı, pil döngüsü 112. Kutusu ve şarj aleti duruyor. Ekranda çizik yok.",
                "Bilgisayar", "İstanbul", UrunDurumu.AzKullanilmis, 28000, 500, 32000, TimeSpan.FromDays(6), "macbook-1.jpg"));
            ilanlar.Add(Ilan("iPhone 13 128 GB", "Pil sağlığı %88. Hep kılıf ve ekran koruyucuyla kullanıldı. Faturası var.",
                "Cep Telefonu & Aksesuar", "Ankara", UrunDurumu.IkinciEl, 17500, 250, null, TimeSpan.FromHours(20), "iphone-1.jpg"));
            ilanlar.Add(Ilan("Canon EOS 250D + 18-55 mm Kit", "Deklanşör sayısı 6.000 civarı. Yedek batarya ve 64 GB kart ile birlikte.",
                "Fotoğraf & Kamera", "İzmir", UrunDurumu.AzKullanilmis, 14000, 250, 16000, TimeSpan.FromDays(8), "canon-1.jpg"));
            ilanlar.Add(Ilan("Seiko Presage Otomatik Kol Saati", "Kutulu, belgeli. Kasa çapı 40,5 mm. Kayış orijinal, deri.",
                "Saat", "İstanbul", UrunDurumu.AzKullanilmis, 9000, 200, null, TimeSpan.FromDays(10), "saat-1.jpg"));
            ilanlar.Add(Ilan("PlayStation 5 Diskli + Kol", "Az oynandı, fan sesi yok. DualSense kol ile birlikte.",
                "Oyunculara Özel", "Eskişehir", UrunDurumu.IkinciEl, 16000, 250, 18500, TimeSpan.FromDays(1), "ps5-1.jpg"));
            ilanlar.Add(Ilan("Antika Bakır Cezve ve Tepsi Takımı", "Dededen kalma, el işçiliği. Kalaylı, kullanıma uygun. Yaklaşık 60 yıllık.",
                "Antika", "Gaziantep", UrunDurumu.IkinciEl, 2500, 100, null, TimeSpan.FromDays(12), "antika-1.jpg"));
            ilanlar.Add(Ilan("Yamaha F310 Akustik Gitar", "Yeni tel takıldı, kılıfı ile birlikte. Başlangıç için ideal.",
                "Müzik", "Ankara", UrunDurumu.IkinciEl, 3500, 100, null, TimeSpan.FromDays(5), "gitar-1.jpg"));

            return ilanlar;
        }
    }
}
