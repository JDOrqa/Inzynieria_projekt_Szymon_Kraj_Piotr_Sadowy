using KomisSamochodowy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// RequireConfirmedAccount = false – łatwiejsze testy studenckie
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Polityki autoryzacji
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Administrator"));
    options.AddPolicy("SprzedawcaOrAdmin", p => p.RequireRole("Sprzedawca", "Administrator"));
    options.AddPolicy("KlientOnly", p => p.RequireRole("Klient"));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// === SEED RÓL I KONT STARTOWYCH ===
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = sp.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await SeedData.InitializeAsync(userManager, roleManager, dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // <-- MUSI być przed UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();