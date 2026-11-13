// Telefonda header menusunu ac / kapat
document.querySelectorAll('.menu-ac').forEach(function (dugme) {
    dugme.addEventListener('click', function () {
        var header = dugme.closest('.site-header');
        var acik = header.classList.toggle('acik');
        dugme.setAttribute('aria-expanded', acik ? 'true' : 'false');
        dugme.setAttribute('aria-label', acik ? 'Menüyü kapat' : 'Menüyü aç');
    });
});

// Sifre alanindaki "Goster / Gizle" dugmesi
document.querySelectorAll('[data-sifre-goster]').forEach(function (dugme) {
    var girdi = document.getElementById(dugme.getAttribute('data-sifre-goster'));
    if (!girdi) return;
    dugme.addEventListener('click', function () {
        var gizli = girdi.type === 'password';
        girdi.type = gizli ? 'text' : 'password';
        dugme.textContent = gizli ? 'Gizle' : 'Göster';
        dugme.setAttribute('aria-label', gizli ? 'Şifreyi gizle' : 'Şifreyi göster');
    });
});

// Kayit formunda sifre kurallarini yazarken isaretle
document.querySelectorAll('[data-sifre-kurallari]').forEach(function (girdi) {
    var liste = document.getElementById(girdi.getAttribute('data-sifre-kurallari'));
    if (!liste) return;
    var kontroller = {
        uzunluk: function (s) { return s.length >= 8; },
        rakam: function (s) { return /\d/.test(s); },
        kucuk: function (s) { return /[a-zçğıöşü]/.test(s); }
    };
    function guncelle() {
        liste.querySelectorAll('[data-kural]').forEach(function (madde) {
            var kontrol = kontroller[madde.getAttribute('data-kural')];
            madde.classList.toggle('tamam', !!kontrol && kontrol(girdi.value));
        });
    }
    girdi.addEventListener('input', guncelle);
    guncelle();
});

// Ana sayfadaki "Hoş geldin" bandini kapat
document.querySelectorAll('.karsilama-kapat').forEach(function (dugme) {
    dugme.addEventListener('click', function () {
        dugme.closest('.karsilama').hidden = true;
    });
});

// İlan kartı ve detay sayfasındaki geri sayım (data-geri-sayim: bitiş zamanı, UTC)
var geriSayimlar = document.querySelectorAll('[data-geri-sayim]');
function geriSayimGuncelle() {
    var simdi = Date.now();
    geriSayimlar.forEach(function (alan) {
        var bitis = new Date(alan.getAttribute('data-geri-sayim')).getTime();
        var baslangic = alan.hasAttribute('data-baslangic') ? new Date(alan.getAttribute('data-baslangic')).getTime() : 0;
        var hedef = simdi < baslangic ? baslangic : bitis;
        var kalan = Math.floor((hedef - simdi) / 1000);
        if (kalan <= 0) {
            alan.textContent = 'Süre doldu';
            alan.classList.add('bitti');
            return;
        }
        var gun = Math.floor(kalan / 86400);
        var saat = Math.floor(kalan % 86400 / 3600);
        var dakika = Math.floor(kalan % 3600 / 60);
        var saniye = kalan % 60;
        var yazi = (gun > 0 ? gun + 'g ' : '') + String(saat).padStart(2, '0') + ':' +
            String(dakika).padStart(2, '0') + ':' + String(saniye).padStart(2, '0');
        alan.textContent = (simdi < baslangic ? 'Başlamasına ' : '') + yazi;
        alan.classList.toggle('son-dakikalar', simdi >= baslangic && kalan <= 120);
    });
}
if (geriSayimlar.length > 0) {
    geriSayimGuncelle();
    setInterval(geriSayimGuncelle, 1000);
}
