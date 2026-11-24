"""Örnek ilan fotoğraflarını Wikimedia Commons'tan indirir (sadece serbest lisanslı görseller).

Kullanım (çözüm klasöründen):
  python araclar/ornek_foto_indir.py                 # eksik olan bütün fotoğrafları indirir
  python araclar/ornek_foto_indir.py --dosya x.jpg   # tek bir fotoğrafı yeniden indirir
       [--arama "başka arama"] [--atla 2]            # farklı arama ya da sonraki aday
  python araclar/ornek_foto_indir.py --yenile        # var olanların da üzerine yazar

Her fotoğrafın kaynağı, yazarı ve lisansı wwwroot/ornek-ilanlar/KAYNAKLAR.md tablosuna yazılır.
Sadece Python standart kütüphanesi kullanılır.
"""
import argparse, html, json, os, re, sys, time, urllib.parse, urllib.request

KOK = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
KLASOR = os.path.join(KOK, "AcikArtirmaPazaryeri", "wwwroot", "ornek-ilanlar")
KAYNAKLAR = os.path.join(KLASOR, "KAYNAKLAR.md")
LISTE = os.path.join(KOK, "araclar", "ornek_fotolar.json")
API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "AcikArtirmaPazaryeri-demo/1.0 (student project; ornek veri)"}
IZINLI = ("cc by", "cc-by", "cc0", "public domain", "kamu malı", "pd")


def istek(url):
    for deneme in range(4):
        try:
            with urllib.request.urlopen(urllib.request.Request(url, headers=UA), timeout=30) as y:
                return y.read()
        except Exception as h:
            if deneme == 3:
                raise
            time.sleep(2 + deneme * 3)


def adaylar(arama):
    q = {"action": "query", "format": "json", "generator": "search", "gsrnamespace": "6",
         "gsrsearch": arama + " filetype:bitmap", "gsrlimit": "20", "prop": "imageinfo",
         "iiprop": "url|size|mime|extmetadata", "iiurlwidth": "1200"}
    veri = json.loads(istek(API + "?" + urllib.parse.urlencode(q)))
    sayfalar = sorted(veri.get("query", {}).get("pages", {}).values(), key=lambda p: p.get("index", 99))
    sonuc = []
    for p in sayfalar:
        ii = (p.get("imageinfo") or [{}])[0]
        meta = ii.get("extmetadata", {})
        lisans = meta.get("LicenseShortName", {}).get("value", "")
        if ii.get("mime") != "image/jpeg" or ii.get("width", 0) < 800 or ii.get("width", 0) < ii.get("height", 0) * 0.9:
            continue  # dikey ve küçük fotoğraflar kartta kötü duruyor
        if not any(l in lisans.lower() for l in IZINLI) or "nc" in lisans.lower().split("-"):
            continue
        yazar = re.sub("<[^>]+>", "", html.unescape(meta.get("Artist", {}).get("value", "bilinmiyor"))).strip()
        sonuc.append({"url": ii.get("thumburl") or ii["url"], "sayfa": ii["descriptionurl"],
                      "yazar": re.sub(r"\s+", " ", yazar)[:80], "lisans": lisans})
    return sonuc


def kaynak_yaz(dosya, a):
    satirlar = open(KAYNAKLAR, encoding="utf-8").read().splitlines() if os.path.exists(KAYNAKLAR) else [
        "# Örnek ilan fotoğrafları — kaynaklar", "",
        "Fotoğrafların hepsi Wikimedia Commons'tan alındı. Genişlikleri en fazla 1200 px.", "",
        "| dosya | kaynak sayfası URL | yazar | lisans |", "|---|---|---|---|"]
    yeni = f"| {dosya} | {a['sayfa']} | {a['yazar'].replace('|', '/')} | {a['lisans']} |"
    satirlar = [s for s in satirlar if not s.startswith(f"| {dosya} |")] + [yeni]
    open(KAYNAKLAR, "w", encoding="utf-8").write("\n".join(satirlar) + "\n")


def indir(dosya, arama, atla=0):
    liste = adaylar(arama)
    if len(liste) <= atla:
        print(f"  BULUNAMADI: {dosya} ({arama})")
        return False
    a = liste[atla]
    open(os.path.join(KLASOR, dosya), "wb").write(istek(a["url"]))
    kaynak_yaz(dosya, a)
    print(f"  {dosya} <- {a['sayfa']} ({a['lisans']})")
    time.sleep(0.5)
    return True


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--dosya")
    ap.add_argument("--arama")
    ap.add_argument("--atla", type=int, default=0)
    ap.add_argument("--yenile", action="store_true")
    g = ap.parse_args()
    liste = json.load(open(LISTE, encoding="utf-8"))
    if g.dosya:
        oge = next((x for x in liste if x["dosya"] == g.dosya), None)
        arama = g.arama or (oge and oge["arama"])
        sys.exit(0 if arama and indir(g.dosya, arama, g.atla) else 1)
    eksik = 0
    for x in liste:
        yol = os.path.join(KLASOR, x["dosya"])
        if os.path.exists(yol) and not g.yenile:
            continue
        if not indir(x["dosya"], x["arama"]):
            eksik += 1
    print(f"Bitti. Bulunamayan: {eksik}")


if __name__ == "__main__":
    main()
