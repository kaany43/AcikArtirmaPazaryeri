using System.Text.Json;

namespace AcikArtirmaPazaryeri.Servisler
{
    public record VinSonucu(string? Marka, string? Model, int? Yil, string? Yakit, string? Vites);

    // Şasi numarasını NHTSA vPIC servisinden çözer.
    public class VinServisi
    {
        private readonly HttpClient _http;

        public VinServisi(HttpClient http)
        {
            _http = http;
            _http.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<VinSonucu?> CozAsync(string sasiNo)
        {
            VpicYanit? yanit;
            try
            {
                var adres = $"https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVinValues/{Uri.EscapeDataString(sasiNo)}?format=json";
                yanit = await _http.GetFromJsonAsync<VpicYanit>(adres);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                return null;
            }

            var sonuc = yanit?.Results?.FirstOrDefault();
            if (sonuc == null)
            {
                return null;
            }
            // Hibrit araçlarda FuelTypePrimary "Gasoline" döner; hibrit bilgisi ElectrificationLevel'dadır.
            var yakit = sonuc.ElectrificationLevel?.Contains("Hybrid", StringComparison.OrdinalIgnoreCase) == true
                ? sonuc.ElectrificationLevel
                : sonuc.FuelTypePrimary;
            return new VinSonucu(
                BosIseNull(sonuc.Make),
                BosIseNull(sonuc.Model),
                int.TryParse(sonuc.ModelYear, out var yil) ? yil : null,
                BosIseNull(yakit),
                BosIseNull(sonuc.TransmissionStyle));
        }

        private static string? BosIseNull(string? deger) => string.IsNullOrWhiteSpace(deger) ? null : deger;

        private class VpicYanit
        {
            public List<VpicSonuc>? Results { get; set; }
        }

        private class VpicSonuc
        {
            public string? Make { get; set; }
            public string? Model { get; set; }
            public string? ModelYear { get; set; }
            public string? FuelTypePrimary { get; set; }
            public string? ElectrificationLevel { get; set; }
            public string? TransmissionStyle { get; set; }
        }
    }
}
