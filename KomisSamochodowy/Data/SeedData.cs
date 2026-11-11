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

        if (bmw.Id == 0) context.Marki.Add(bmw);
        if (audi.Id == 0) context.Marki.Add(audi);
        if (toyota.Id == 0) context.Marki.Add(toyota);
        await context.SaveChangesAsync();

        // Dodanie słowników Modeli
        var bmwModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Seria 3 320i") ?? new Model { Nazwa = "Seria 3 320i", MarkaId = bmw.Id };
        var audiModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "A4 Avant") ?? new Model { Nazwa = "A4 Avant", MarkaId = audi.Id };
        var toyotaModel = await context.Modele.FirstOrDefaultAsync(m => m.Nazwa == "Corolla Hybrid") ?? new Model { Nazwa = "Corolla Hybrid", MarkaId = toyota.Id };

        if (bmwModel.Id == 0) context.Modele.Add(bmwModel);
        if (audiModel.Id == 0) context.Modele.Add(audiModel);
        if (toyotaModel.Id == 0) context.Modele.Add(toyotaModel);
        await context.SaveChangesAsync();

        var existingCars = await context.Samochody.ToListAsync();
        foreach (var existingCar in existingCars)
        {
            if (string.IsNullOrWhiteSpace(existingCar.Paliwo)) existingCar.Paliwo = "Benzyna";
            if (string.IsNullOrWhiteSpace(existingCar.Status)) existingCar.Status = "Dostępny";
            if (string.IsNullOrWhiteSpace(existingCar.ZdjecieUrl)) existingCar.ZdjecieUrl = "https://www.motorsworld.co.uk/assets/no-image.jpg";
        }

        var cars = new[]
        {
            new Samochod
            {
                MarkaId = bmw.Id,
                ModelId = bmwModel.Id,
                RokProdukcji = 2021,
                Cena = 139900,
                Przebieg = 48500,
                Paliwo = "Benzyna",
                ZdjecieUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=900&q=85",
                Opis = "Zadbane BMW z bogatym wyposażeniem i automatyczną skrzynią biegów.",
                GaleriaZdjec = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=900&q=85"
            },
            new Samochod
            {
                MarkaId = audi.Id,
                ModelId = audiModel.Id,
                RokProdukcji = 2020,
                Cena = 124500,
                Przebieg = 67200,
                Paliwo = "Diesel",
                ZdjecieUrl = "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?auto=format&fit=crop&w=900&q=85",
                Opis = "Przestronne Audi A4 Avant, idealne do codziennej jazdy i dłuższych tras.",
                GaleriaZdjec = "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?auto=format&fit=crop&w=900&q=85"
            },
            new Samochod
            {
                MarkaId = toyota.Id,
                ModelId = toyotaModel.Id,
                RokProdukcji = 2022,
                Cena = 109900,
                Przebieg = 29100,
                Paliwo = "Hybryda",
                ZdjecieUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLi3ihRmHpdVEezYYdMsxsfigno-w0VbQVbFmSrNB4-w&s=10",
                Opis = "Ekonomiczna Toyota Corolla Hybrid z niskim przebiegiem i pełną historią serwisową.",
                GaleriaZdjec = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLi3ihRmHpdVEezYYdMsxsfigno-w0VbQVbFmSrNB4-w&s=10"
            }
        };

        foreach (var car in cars)
        {
            var alreadyExists = await context.Samochody
                .AnyAsync(x => x.MarkaId == car.MarkaId && x.ModelId == car.ModelId);

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