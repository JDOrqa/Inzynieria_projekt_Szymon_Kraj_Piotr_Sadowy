using Microsoft.AspNetCore.Identity;

namespace KomisSamochodowy.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
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