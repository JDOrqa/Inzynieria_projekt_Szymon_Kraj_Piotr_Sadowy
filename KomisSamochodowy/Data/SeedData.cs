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
        var existingCars = await context.Samochody.ToListAsync();
        foreach (var existingCar in existingCars)
        {
            if (string.IsNullOrWhiteSpace(existingCar.Paliwo))
                existingCar.Paliwo = "Benzyna";
            if (string.IsNullOrWhiteSpace(existingCar.Status))
                existingCar.Status = "Dostępny";
            if (string.IsNullOrWhiteSpace(existingCar.ZdjecieUrl))
                existingCar.ZdjecieUrl = "https://www.motorsworld.co.uk/assets/no-image.jpg";
        }

        var cars = new[]
        {
            new Samochod
            {
                Marka = "BMW",
                Model = "Seria 3 320i",
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
                Marka = "Audi",
                Model = "A4 Avant",
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
                Marka = "Toyota",
                Model = "Corolla Hybrid",
                RokProdukcji = 2022,
                Cena = 109900,
                Przebieg = 29100,
                Paliwo = "Hybryda",
                ZdjecieUrl = "https://images.unsplash.com/photo-1550355291-byee04a92027?auto=format&fit=crop&w=900&q=85",
                Opis = "Ekonomiczna Toyota Corolla Hybrid z niskim przebiegiem i pełną historią serwisową.",
                GaleriaZdjec = "https://images.unsplash.com/photo-1550355291-byee04a92027?auto=format&fit=crop&w=900&q=85"
            }
        };

        foreach (var car in cars)
        {
            var alreadyExists = await context.Samochody
                .AnyAsync(x => x.Marka == car.Marka && x.Model == car.Model);

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
                return; // w razie problemów – pomiń
        }

        if (!await userManager.IsInRoleAsync(user, rola))
            await userManager.AddToRoleAsync(user, rola);
    }
}