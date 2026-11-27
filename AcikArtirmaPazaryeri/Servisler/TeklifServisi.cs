using AcikArtirmaPazaryeri.Data;
using AcikArtirmaPazaryeri.Models;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Servisler
{
    public record TeklifSonucu(bool Basarili, string Mesaj);

    public class TeklifServisi
    {
        private readonly AcikArtirmaPazaryeriDbContext _context;

        public TeklifServisi(AcikArtirmaPazaryeriDbContext context)
        {
            _context = context;
        }

        // İlk teklif başlangıç fiyatından, sonrakiler güncel fiyat + minimum artıştan az olamaz.
        public static decimal EnAzTeklif(Muzayede muzayede)
        {
            if (muzayede.GuncelFiyat == null)
            {
                return muzayede.BaslangicFiyati;
            }
            return muzayede.GuncelFiyat.Value + muzayede.MinimumArtis;
        }

        // Kurallara uymayan teklifte hata mesajı döner, uyuyorsa null.
        public static string? KuralKontrol(Muzayede muzayede, string? kullaniciId, decimal tutar, DateTime simdi)
        {
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return "Teklif vermek için giriş yapmalısınız.";
            }
            if (muzayede.SaticiId == kullaniciId)
            {
                return "Kendi ilanınıza teklif veremezsiniz.";
            }
            if (muzayede.Durum != MuzayedeDurumu.Aktif)
            {
                return "Bu ilan teklife açık değil.";
            }
            if (simdi < muzayede.BaslangicZamani)
            {
                return "Müzayede henüz başlamadı.";
            }
            if (simdi >= muzayede.BitisZamani)
            {
                return "Müzayede sona erdi.";
            }
            var enAz = EnAzTeklif(muzayede);
            if (tutar < enAz)
            {
                return $"Teklifiniz en az {enAz:N0} ₺ olmalı.";
            }
            return null;
        }

        public async Task<TeklifSonucu> TeklifVerAsync(int muzayedeId, string? kullaniciId, decimal tutar)
        {
            var muzayede = await _context.Muzayedeler.FindAsync(muzayedeId);
            if (muzayede == null)
            {
                return new TeklifSonucu(false, "İlan bulunamadı.");
            }

            var hata = KuralKontrol(muzayede, kullaniciId, tutar, DateTime.UtcNow);
            if (hata != null)
            {
                return new TeklifSonucu(false, hata);
            }

            muzayede.GuncelFiyat = tutar;
            _context.Teklifler.Add(new Teklif
            {
                MuzayedeId = muzayede.MuzayedeId,
                KullaniciId = kullaniciId!,
                Tutar = tutar
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Biz okuduktan sonra başka biri teklif verdi, Surum değişti. Bu teklif kaydedilmez.
                return new TeklifSonucu(false, "Aynı anda başka bir teklif geldi ve fiyat değişti. Güncel fiyata bakıp tekrar deneyin.");
            }

            return new TeklifSonucu(true, $"{tutar:N0} ₺ teklifiniz alındı.");
        }
    }
}
