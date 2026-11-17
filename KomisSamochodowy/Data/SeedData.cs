using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KomisSamochodowy.Models;

namespace KomisSamochodowy.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        // 1) Role
        string[] role = { "Administrator", "Sprzedawca", "Klient" };
        foreach (var r in role)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole(r));
        }

        // 2) Konto administratora
        await EnsureUserAsync(userManager,
            email: "admin@komis.pl",
            haslo: "Admin123!",
            rola: "Administrator");

        // 3) Konto sprzedawcy
        await EnsureUserAsync(userManager,
            email: "sprzedawca@komis.pl",
            haslo: "Sprzedawca123!",
            rola: "Sprzedawca");

        await EnsureCarsAsync(context);
    }

    private static async Task EnsureCarsAsync(ApplicationDbContext context)
    {
        // Dodanie słowników Marek
        var bmw = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "BMW") ?? new Marka { Nazwa = "BMW" };
        var audi = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Audi") ?? new Marka { Nazwa = "Audi" };
        var toyota = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Toyota") ?? new Marka { Nazwa = "Toyota" };
        var mercedes = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Mercedes-Benz") ?? new Marka { Nazwa = "Mercedes-Benz" };
        var volvo = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Volvo") ?? new Marka { Nazwa = "Volvo" };
        var ford = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Ford") ?? new Marka { Nazwa = "Ford" };
        var vw = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Volkswagen") ?? new Marka { Nazwa = "Volkswagen" };
        var skoda = await context.Marki.FirstOrDefaultAsync(m => m.Nazwa == "Skoda") ?? new Marka { Nazwa = "Skoda" };

        if (bmw.Id == 0) context.Marki.Add(bmw);
        if (audi.Id == 0) context.Marki.Add(audi);
        if (toyota.Id == 0) context.Marki.Add(toyota);
        if (mercedes.Id == 0) context.Marki.Add(mercedes);
        if (volvo.Id == 0) context.Marki.Add(volvo);
        if (ford.Id == 0) context.Marki.Add(ford);
        if (vw.Id == 0) context.Marki.Add(vw);
        if (skoda.Id == 0) context.Marki.Add(skoda);
        await context.SaveChangesAsync();

        // Dodanie słowników Modeli
        var bmwModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Seria 3 320i") ?? new Model { Nazwa = "Seria 3 320i", MarkaId = bmw.Id };
        var audiModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "A4 Avant") ?? new Model { Nazwa = "A4 Avant", MarkaId = audi.Id };
        var toyotaModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Corolla Hybrid") ?? new Model { Nazwa = "Corolla Hybrid", MarkaId = toyota.Id };
        var mercedesModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "C-Class") ?? new Model { Nazwa = "C-Class", MarkaId = mercedes.Id };
        var volvoModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "XC90") ?? new Model { Nazwa = "XC90", MarkaId = volvo.Id };
        var fordModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Focus") ?? new Model { Nazwa = "Focus", MarkaId = ford.Id };
        var vwModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Passat") ?? new Model { Nazwa = "Passat", MarkaId = vw.Id };
        var skodaModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Superb") ?? new Model { Nazwa = "Superb", MarkaId = skoda.Id };

        if (bmwModel.Id == 0) context.Modele.Add(bmwModel);
        if (audiModel.Id == 0) context.Modele.Add(audiModel);
        if (toyotaModel.Id == 0) context.Modele.Add(toyotaModel);
        if (mercedesModel.Id == 0) context.Modele.Add(mercedesModel);
        if (volvoModel.Id == 0) context.Modele.Add(volvoModel);
        if (fordModel.Id == 0) context.Modele.Add(fordModel);
        if (vwModel.Id == 0) context.Modele.Add(vwModel);
        if (skodaModel.Id == 0) context.Modele.Add(skodaModel);
        await context.SaveChangesAsync();

        // --- POPRAWKA: Aktualizacja starych, zepsutych linków w istniejących autach ---
        // --- POPRAWKA: Aktualizacja starych, zepsutych linków w istniejących autach ---
        var existingCars = await context.Samochody.ToListAsync();
        foreach (var existingCar in existingCars)
        {
            if (string.IsNullOrWhiteSpace(existingCar.Paliwo)) existingCar.Paliwo = "Benzyna";
            if (string.IsNullOrWhiteSpace(existingCar.Status)) existingCar.Status = "Dostępny";

            // Jeśli w bazie jest stary link Unsplash, podmieniamy go na bezpieczny link z Wikimedia
            if (existingCar.ZdjecieUrl != null && existingCar.ZdjecieUrl.Contains("images.unsplash.com"))
            {
                existingCar.ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/2015_Audi_A4_Avant.jpg";
                existingCar.GaleriaZdjec = existingCar.ZdjecieUrl;
            }
        }
        await context.SaveChangesAsync();
        await context.SaveChangesAsync();

        var cars = new[]
       {
            // BMW Seria 3 320i
            new Samochod
            {
                MarkaId = bmw.Id,
                ModelId = bmwModel.Id,
                RokProdukcji = 2021,
                Cena = 139900,
                Przebieg = 48500,
                Paliwo = "Benzyna",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/BMW_320i_Luxury_(F30)_front.JPG",
                Opis = "Zadbane BMW Seria 3 320i z bogatym wyposażeniem, automatyczną skrzynią biegów, systemem nawigacji i klimatyzacją. Pełna historia serwisowa. Idealny samochód do codziennej jazdy i dłuższych podróży.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/BMW_320i_Luxury_(F30)_front.JPG"
            },
            // Audi A4 Avant
            new Samochod
            {
                MarkaId = audi.Id,
                ModelId = audiModel.Id,
                RokProdukcji = 2020,
                Cena = 124500,
                Przebieg = 67200,
                Paliwo = "Diesel",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/2015_Audi_A4_Avant.jpg",
                Opis = "Przestronne Audi A4 Avant z silnikiem diesla, doskonałym połączeniem ekonomii paliwa i mocy. Bezwypadkowy, z wyposażeniem LED, systemem parkowania i asystentem pasa ruchu. Idealne do codziennej jazdy i transportu.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/2015_Audi_A4_Avant.jpg"
            },
            // Toyota Corolla Hybrid
            new Samochod
            {
                MarkaId = toyota.Id,
                ModelId = toyotaModel.Id,
                RokProdukcji = 2022,
                Cena = 109900,
                Przebieg = 29100,
                Paliwo = "Hybryda",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Toyota_Corolla_Hybrid_(E210)_IMG_4338.jpg",
                Opis = "Ekonomiczna Toyota Corolla Hybrid z niskim przebiegiem i pełną historią serwisowej. System hybrydowy gwarantuje niskie spalanie i przyjazne dla środowiska. Wyposażeni w system bezpieczeństwa Toyota Safety Sense.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/Toyota_Corolla_Hybrid_(E210)_IMG_4338.jpg"
            },
            // Mercedes-Benz C-Class
            new Samochod
            {
                MarkaId = mercedes.Id,
                ModelId = mercedesModel.Id,
                RokProdukcji = 2022,
                Cena = 189500,
                Przebieg = 35800,
                Paliwo = "Benzyna",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Mercedes_C_Class.jpg",
                Opis = "Luksusowy Mercedes-Benz C-Class z silnikiem 1.5L turbocharged. Wyposażenie premium: tapicerka skórzana, panoramiczny dach, kierownica podgrzewana. AIRMATIC - zawieszenie pneumatyczne dla wygody jazdy. Stan idealny.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/Mercedes_C_Class.jpg"
            },
            // Volvo XC90
            new Samochod
            {
                MarkaId = volvo.Id,
                ModelId = volvoModel.Id,
                RokProdukcji = 2021,
                Cena = 169900,
                Przebieg = 52300,
                Paliwo = "Diesel",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Volvo_XC90_front.JPG",
                Opis = "Bezpieczny i komfortowy Volvo XC90 z silnikiem diesla D5. 7 siedzeń, system Volvo On Call, automatyczna skrzynia biegów. Bogate wyposażenie bezpieczeństwa - system City Safety. Idealny dla rodzin.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/Volvo_XC90_front.JPG"
            },
            // Ford Focus
            new Samochod
            {
                MarkaId = ford.Id,
                ModelId = fordModel.Id,
                RokProdukcji = 2020,
                Cena = 79900,
                Przebieg = 89400,
                Paliwo = "Benzyna",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/2022_Ford_Focus.jpg",
                Opis = "Praktyczny Ford Focus 1.5L EcoBoost z manualną skrzynią biegów. Niskie spalanie, dynamiczna jazda. Wyposażony w system SYNC 3 z ekranem dotykowym. Doskonały wybór do codziennej jazdy.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/2022_Ford_Focus.jpg"
            },
            // Volkswagen Passat
            new Samochod
            {
                MarkaId = vw.Id,
                ModelId = vwModel.Id,
                RokProdukcji = 2021,
                Cena = 119900,
                Przebieg = 61200,
                Paliwo = "Diesel",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/VW_Passat_Variant_2.0_TDI_Elegance_(B9)_%E2%80%93_f_03062024.jpg",
                Opis = "Elegancki Volkswagen Passat z silnikiem TDI zapewniającym ekonomię paliwa. Duży bagażnik, komfortowe siedzenia. Wyposażony w system ACC (Adaptive Cruise Control) i asystenta parkowania. Bezwypadkowy.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/VW_Passat_Variant_2.0_TDI_Elegance_(B9)_%E2%80%93_f_03062024.jpg"
            },
            // Skoda Superb
            new Samochod
            {
                MarkaId = skoda.Id,
                ModelId = skodaModel.Id,
                RokProdukcji = 2023,
                Cena = 134900,
                Przebieg = 15600,
                Paliwo = "Benzyna",
                Status = "Dostępny",
                ZdjecieUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Skoda_Superb_162TSI,_2018_front.jpg",
                Opis = "Nowy Skoda Superb z silnikiem 1.4TSI i automatyczną skrzynią biegów. Nowoczesna technologia, spacjona kabina. Wyposażony w systemy ADAS: Lane Keeping i Traffic Sign Recognition. Gwarancja producenta.",
                GaleriaZdjec = "https://commons.wikimedia.org/wiki/Special:FilePath/Skoda_Superb_162TSI,_2018_front.jpg"
            }
        };

        foreach (var car in cars)
        {
            var alreadyExists = await context.Samochody
                .AnyAsync(x => x.MarkaId == car.MarkaId && x.ModelId == car.ModelId && x.RokProdukcji == car.RokProdukcji);

            if (!alreadyExists)
                context.Samochody.Add(car);
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureUserAsync(
        UserManager<IdentityUser> userManager,
        string email, string haslo, string rola)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var created = await userManager.CreateAsync(user, haslo);
            if (!created.Succeeded)
                return;
        }

        if (!await userManager.IsInRoleAsync(user, rola))
            await userManager.AddToRoleAsync(user, rola);
    }
}