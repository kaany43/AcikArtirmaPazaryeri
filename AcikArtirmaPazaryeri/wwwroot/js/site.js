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
