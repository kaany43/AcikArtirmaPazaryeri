using System.Text.Json;
using System.Text.Json.Serialization;
using AcikArtirmaPazaryeri.Models;
using AcikArtirmaPazaryeri.Pages.Satici;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Data
{
    // Sunum için örnek satıcılar ve ilanlar.
    // Veriler Data/OrnekIlanlar.json dosyasında, fotoğraflar wwwroot/ornek-ilanlar klasöründe.
    // Uygulama her açıldığında:
    //  1) Örnek satıcı hesapları yoksa oluşturulur (şifresiz olduğu için bu hesaplarla giriş yapılamaz).
    //  2) JSON'daki ilan listesi veritabanındakinden farklıysa örnek ilanlar silinip baştan eklenir.
    //  3) Süresi dolmuş örnek ilanların süresi yeniden başlatılır, böylece site hep canlı görünür.
    public static class OrnekIlanSeed
    {
        private static readonly JsonSerializerOptions JsonAyarlari = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static async Task HazirlaAsync(AcikArtirmaPazaryeriDbContext context, UserManager<Kullanici> userManager,
            string icerikKlasoru, string wwwrootKlasoru)
        {
            var dosya = Path.Combine(icerikKlasoru, "Data", "OrnekIlanlar.json");
            if (!File.Exists(dosya))
            {
                return;
            }
            var veri = JsonSerializer.Deserialize<OrnekVeri>(await File.ReadAllTextAsync(dosya), JsonAyarlari)!;

            // 1) Örnek satıcılar
            var saticiIdleri = new Dictionary<string, string>();
            foreach (var s in veri.Saticilar)
            {
                var kullanici = await userManager.FindByEmailAsync(s.Eposta);
                if (kullanici == null)
                {
                    kullanici = new Kullanici { UserName = s.Eposta, Email = s.Eposta, EmailConfirmed = true, Ad = s.Ad, Soyad = s.Soyad };
                    var sonuc = await userManager.CreateAsync(kullanici);
                    if (!sonuc.Succeeded)
                    {
                        throw new Exception($"Örnek satıcı oluşturulamadı: {string.Join(", ", sonuc.Errors.Select(e => e.Description))}");
                    }
                }
                // Profil bilgileri JSON'dakiyle aynı tutulur.
                kullanici.Ad = s.Ad;
                kullanici.Soyad = s.Soyad;
                kullanici.FirmaAdi = s.FirmaAdi;
                kullanici.GuvenilirSaticiMi = s.GuvenilirMi;
                await userManager.UpdateAsync(kullanici);
                foreach (var rol in new[] { "Alici", "Satici" })
                {
                    if (!await userManager.IsInRoleAsync(kullanici, rol))
                    {
                        await userManager.AddToRoleAsync(kullanici, rol);
                    }
                }
                saticiIdleri[s.Anahtar] = kullanici.Id;
            }

            // Fotoğrafı klasörde olmayan ilan eklenmez; fotoğraf sonradan gelirse bir sonraki açılışta eklenir.
            var fotoKlasoru = Path.Combine(wwwrootKlasoru, "ornek-ilanlar");
            var hazirIlanlar = veri.Ilanlar
                .Where(i => i.Fotograflar.Count > 0 && i.Fotograflar.All(f => File.Exists(Path.Combine(fotoKlasoru, f))))
                .ToList();

            var idler = saticiIdleri.Values.ToList();
            var mevcutlar = await context.Muzayedeler.Where(m => idler.Contains(m.SaticiId)).ToListAsync();
            var simdi = DateTime.UtcNow;

            // 2) Liste değiştiyse baştan kur
            var ayniListe = mevcutlar.Count == hazirIlanlar.Count
                && mevcutlar.Select(m => m.Baslik).ToHashSet().SetEquals(hazirIlanlar.Select(i => i.Baslik));
            if (!ayniListe)
            {
                // Resimler, araç detayları, ekspertiz ve onay kayıtları veritabanında cascade ile silinir.
                context.Muzayedeler.RemoveRange(mevcutlar);
                await context.SaveChangesAsync();

                var kategoriler = await context.Kategoriler.ToListAsync();
                var modeller = await context.AracModelleri.Include(m => m.Marka).ToListAsync();
                for (var sira = 0; sira < hazirIlanlar.Count; sira++)
                {
                    var i = hazirIlanlar[sira];
                    var muzayede = new Muzayede
                    {
                        Baslik = i.Baslik,
                        Aciklama = i.Aciklama,
                        KategoriId = kategoriler.First(k => k.KategoriAdi == i.Kategori).KategoriId,
                        SaticiId = saticiIdleri[i.Satici],
                        Sehir = i.Sehir,
                        UrunDurumu = i.UrunDurumu,
                        BaslangicFiyati = i.BaslangicFiyati,
                        MinimumArtis = i.MinimumArtis,
                        RezervFiyati = i.RezervFiyati,
                        BaslangicZamani = simdi.AddHours(-i.BaslayaliSaat),
                        BitisZamani = simdi.AddHours(i.KalanSaat),
                        // İlan, açık artırma başlamadan birkaç saat ile birkaç gün önce oluşturulmuş görünsün.
                        OlusturmaZamani = simdi.AddHours(-Math.Max(i.BaslayaliSaat, 0) - 3 - sira % 40),
                        Durum = MuzayedeDurumu.Aktif,
                        MuzayedeResimleri = i.Fotograflar.Select((f, j) => new MuzayedeResmi
                        {
                            DosyaYolu = $"/ornek-ilanlar/{f}",
                            Sira = j,
                            KapakMi = j == 0
                        }).ToList()
                    };
                    if (i.Arac != null)
                    {
                        var a = i.Arac;
                        muzayede.AracDetaylari = new AracDetaylari
                        {
                            AracModeliId = modeller.First(m => m.Marka.MarkaAdi == a.Marka && m.ModelAdi == a.Model).AracModeliId,
                            Paket = a.Paket,
                            Yil = a.Yil,
                            Km = a.Km,
                            YakitTuru = a.Yakit,
                            VitesTuru = a.Vites,
                            KasaTipi = a.Kasa,
                            Renk = a.Renk,
                            SasiNo = a.SasiNo,
                            // Motosiklet, ATV ve UTV'de kaporta ekspertizi yok. Diğerlerinde listede olmayan parçalar orijinal.
                            EkspertizKalemleri = a.EkspertizYok
                                ? new List<EkspertizKalemi>()
                                : EkleModel.Parcalar.Select(p => new EkspertizKalemi
                                {
                                    Parca = p,
                                    Durum = a.Ekspertiz.TryGetValue(p, out var d) ? d : ParcaDurumu.Orijinal
                                }).ToList()
                        };
                    }
                    context.Muzayedeler.Add(muzayede);
                }
                await context.SaveChangesAsync();
                return;
            }

            // 3) Süresi dolanları yeniden başlat
            var tanimlar = hazirIlanlar.ToDictionary(i => i.Baslik);
            foreach (var m in mevcutlar.Where(m => m.BitisZamani <= simdi))
            {
                var t = tanimlar[m.Baslik];
                m.BaslangicZamani = simdi.AddHours(-t.BaslayaliSaat);
                m.BitisZamani = simdi.AddHours(t.KalanSaat);
            }
            await context.SaveChangesAsync();
        }

        // JSON dosyasının yapısı
        private record OrnekVeri(List<OrnekSatici> Saticilar, List<OrnekIlan> Ilanlar);

        private record OrnekSatici(string Anahtar, string Eposta, string Ad, string Soyad, string? FirmaAdi, bool GuvenilirMi);

        private record OrnekIlan(
            string Satici, string Kategori, string Sehir, UrunDurumu UrunDurumu, string Baslik, string Aciklama,
            decimal BaslangicFiyati, decimal MinimumArtis, decimal? RezervFiyati,
            double KalanSaat,     // açık artırmanın bitmesine kalan süre
            double BaslayaliSaat, // kaç saat önce başladı (eksi değer: henüz başlamadı)
            List<string> Fotograflar, OrnekArac? Arac);

        private record OrnekArac(
            string Marka, string Model, string? Paket, int Yil, int Km, YakitTuru Yakit, VitesTuru Vites, KasaTipi Kasa,
            string Renk, string SasiNo, Dictionary<string, ParcaDurumu> Ekspertiz, bool EkspertizYok);
    }
}
