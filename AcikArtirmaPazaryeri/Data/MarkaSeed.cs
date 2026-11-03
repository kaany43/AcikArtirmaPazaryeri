using AcikArtirmaPazaryeri.Models;

namespace AcikArtirmaPazaryeri.Data
{
    // Müzayede portalı için Marka / AracModeli seed verisi
    // Türkiye ikinci el pazarında yaygın marka ve modeller (alfabetik)
    public static class MarkaSeed
    {
        public static List<Marka> Getir() => new List<Marka>
        {
            new Marka { MarkaAdi = "Audi", Modeller =
            {
                new AracModeli { ModelAdi = "A3" },
                new AracModeli { ModelAdi = "A4" },
                new AracModeli { ModelAdi = "A5" },
                new AracModeli { ModelAdi = "A6" },
                new AracModeli { ModelAdi = "Q2" },
                new AracModeli { ModelAdi = "Q3" },
                new AracModeli { ModelAdi = "Q5" }
            }},

            new Marka { MarkaAdi = "BMW", Modeller =
            {
                new AracModeli { ModelAdi = "1 Serisi" },
                new AracModeli { ModelAdi = "3 Serisi" },
                new AracModeli { ModelAdi = "4 Serisi" },
                new AracModeli { ModelAdi = "5 Serisi" },
                new AracModeli { ModelAdi = "X1" },
                new AracModeli { ModelAdi = "X3" },
                new AracModeli { ModelAdi = "X5" }
            }},

            new Marka { MarkaAdi = "BYD", Modeller =
            {
                new AracModeli { ModelAdi = "Atto 3" },
                new AracModeli { ModelAdi = "Dolphin" },
                new AracModeli { ModelAdi = "Seal" },
                new AracModeli { ModelAdi = "Seal U" }
            }},

            new Marka { MarkaAdi = "Chery", Modeller =
            {
                new AracModeli { ModelAdi = "Omoda 5" },
                new AracModeli { ModelAdi = "Tiggo 7 Pro" },
                new AracModeli { ModelAdi = "Tiggo 8 Pro" }
            }},

            new Marka { MarkaAdi = "Citroën", Modeller =
            {
                new AracModeli { ModelAdi = "Berlingo" },
                new AracModeli { ModelAdi = "C-Elysée" },
                new AracModeli { ModelAdi = "C3" },
                new AracModeli { ModelAdi = "C4" },
                new AracModeli { ModelAdi = "C4 X" },
                new AracModeli { ModelAdi = "C5 Aircross" }
            }},

            new Marka { MarkaAdi = "Dacia", Modeller =
            {
                new AracModeli { ModelAdi = "Duster" },
                new AracModeli { ModelAdi = "Jogger" },
                new AracModeli { ModelAdi = "Lodgy" },
                new AracModeli { ModelAdi = "Logan" },
                new AracModeli { ModelAdi = "Sandero" },
                new AracModeli { ModelAdi = "Sandero Stepway" }
            }},

            new Marka { MarkaAdi = "Fiat", Modeller =
            {
                new AracModeli { ModelAdi = "500" },
                new AracModeli { ModelAdi = "Albea" },
                new AracModeli { ModelAdi = "Doblo" },
                new AracModeli { ModelAdi = "Egea" },
                new AracModeli { ModelAdi = "Egea Cross" },
                new AracModeli { ModelAdi = "Fiorino" },
                new AracModeli { ModelAdi = "Linea" },
                new AracModeli { ModelAdi = "Punto" }
            }},

            new Marka { MarkaAdi = "Ford", Modeller =
            {
                new AracModeli { ModelAdi = "Fiesta" },
                new AracModeli { ModelAdi = "Focus" },
                new AracModeli { ModelAdi = "Kuga" },
                new AracModeli { ModelAdi = "Mondeo" },
                new AracModeli { ModelAdi = "Puma" },
                new AracModeli { ModelAdi = "Ranger" },
                new AracModeli { ModelAdi = "Transit" },
                new AracModeli { ModelAdi = "Transit Courier" }
            }},

            new Marka { MarkaAdi = "Honda", Modeller =
            {
                new AracModeli { ModelAdi = "Accord" },
                new AracModeli { ModelAdi = "City" },
                new AracModeli { ModelAdi = "Civic" },
                new AracModeli { ModelAdi = "CR-V" },
                new AracModeli { ModelAdi = "HR-V" },
                new AracModeli { ModelAdi = "Jazz" }
            }},

            new Marka { MarkaAdi = "Hyundai", Modeller =
            {
                new AracModeli { ModelAdi = "Accent Blue" },
                new AracModeli { ModelAdi = "Bayon" },
                new AracModeli { ModelAdi = "Elantra" },
                new AracModeli { ModelAdi = "i10" },
                new AracModeli { ModelAdi = "i20" },
                new AracModeli { ModelAdi = "Kona" },
                new AracModeli { ModelAdi = "Tucson" }
            }},

            new Marka { MarkaAdi = "Kia", Modeller =
            {
                new AracModeli { ModelAdi = "Ceed" },
                new AracModeli { ModelAdi = "Niro" },
                new AracModeli { ModelAdi = "Picanto" },
                new AracModeli { ModelAdi = "Rio" },
                new AracModeli { ModelAdi = "Sportage" },
                new AracModeli { ModelAdi = "Stonic" }
            }},

            new Marka { MarkaAdi = "Mercedes-Benz", Modeller =
            {
                new AracModeli { ModelAdi = "A Serisi" },
                new AracModeli { ModelAdi = "C Serisi" },
                new AracModeli { ModelAdi = "CLA" },
                new AracModeli { ModelAdi = "E Serisi" },
                new AracModeli { ModelAdi = "GLA" },
                new AracModeli { ModelAdi = "GLC" },
                new AracModeli { ModelAdi = "S Serisi" },
                new AracModeli { ModelAdi = "Vito" }
            }},

            new Marka { MarkaAdi = "Nissan", Modeller =
            {
                new AracModeli { ModelAdi = "Juke" },
                new AracModeli { ModelAdi = "Micra" },
                new AracModeli { ModelAdi = "Navara" },
                new AracModeli { ModelAdi = "Qashqai" },
                new AracModeli { ModelAdi = "X-Trail" }
            }},

            new Marka { MarkaAdi = "Opel", Modeller =
            {
                new AracModeli { ModelAdi = "Astra" },
                new AracModeli { ModelAdi = "Corsa" },
                new AracModeli { ModelAdi = "Crossland" },
                new AracModeli { ModelAdi = "Grandland" },
                new AracModeli { ModelAdi = "Insignia" },
                new AracModeli { ModelAdi = "Mokka" },
                new AracModeli { ModelAdi = "Vectra" }
            }},

            new Marka { MarkaAdi = "Peugeot", Modeller =
            {
                new AracModeli { ModelAdi = "208" },
                new AracModeli { ModelAdi = "2008" },
                new AracModeli { ModelAdi = "301" },
                new AracModeli { ModelAdi = "308" },
                new AracModeli { ModelAdi = "3008" },
                new AracModeli { ModelAdi = "5008" },
                new AracModeli { ModelAdi = "Partner" },
                new AracModeli { ModelAdi = "Rifter" }
            }},

            new Marka { MarkaAdi = "Renault", Modeller =
            {
                new AracModeli { ModelAdi = "Austral" },
                new AracModeli { ModelAdi = "Captur" },
                new AracModeli { ModelAdi = "Clio" },
                new AracModeli { ModelAdi = "Fluence" },
                new AracModeli { ModelAdi = "Kadjar" },
                new AracModeli { ModelAdi = "Megane" },
                new AracModeli { ModelAdi = "Symbol" },
                new AracModeli { ModelAdi = "Taliant" }
            }},

            new Marka { MarkaAdi = "Seat", Modeller =
            {
                new AracModeli { ModelAdi = "Arona" },
                new AracModeli { ModelAdi = "Ateca" },
                new AracModeli { ModelAdi = "Ibiza" },
                new AracModeli { ModelAdi = "Leon" }
            }},

            new Marka { MarkaAdi = "Skoda", Modeller =
            {
                new AracModeli { ModelAdi = "Fabia" },
                new AracModeli { ModelAdi = "Kamiq" },
                new AracModeli { ModelAdi = "Karoq" },
                new AracModeli { ModelAdi = "Kodiaq" },
                new AracModeli { ModelAdi = "Octavia" },
                new AracModeli { ModelAdi = "Scala" },
                new AracModeli { ModelAdi = "Superb" }
            }},

            new Marka { MarkaAdi = "Tesla", Modeller =
            {
                new AracModeli { ModelAdi = "Model 3" },
                new AracModeli { ModelAdi = "Model S" },
                new AracModeli { ModelAdi = "Model X" },
                new AracModeli { ModelAdi = "Model Y" }
            }},

            new Marka { MarkaAdi = "Togg", Modeller =
            {
                new AracModeli { ModelAdi = "T10F" },
                new AracModeli { ModelAdi = "T10X" }
            }},

            new Marka { MarkaAdi = "Toyota", Modeller =
            {
                new AracModeli { ModelAdi = "Auris" },
                new AracModeli { ModelAdi = "C-HR" },
                new AracModeli { ModelAdi = "Camry" },
                new AracModeli { ModelAdi = "Corolla" },
                new AracModeli { ModelAdi = "Hilux" },
                new AracModeli { ModelAdi = "RAV4" },
                new AracModeli { ModelAdi = "Yaris" },
                new AracModeli { ModelAdi = "Yaris Cross" }
            }},

            new Marka { MarkaAdi = "Volkswagen", Modeller =
            {
                new AracModeli { ModelAdi = "Caddy" },
                new AracModeli { ModelAdi = "Golf" },
                new AracModeli { ModelAdi = "Jetta" },
                new AracModeli { ModelAdi = "Passat" },
                new AracModeli { ModelAdi = "Polo" },
                new AracModeli { ModelAdi = "T-Roc" },
                new AracModeli { ModelAdi = "Tiguan" },
                new AracModeli { ModelAdi = "Transporter" }
            }},

            new Marka { MarkaAdi = "Volvo", Modeller =
            {
                new AracModeli { ModelAdi = "S60" },
                new AracModeli { ModelAdi = "S90" },
                new AracModeli { ModelAdi = "V40" },
                new AracModeli { ModelAdi = "XC40" },
                new AracModeli { ModelAdi = "XC60" },
                new AracModeli { ModelAdi = "XC90" }
            }}
        };
    }
}
