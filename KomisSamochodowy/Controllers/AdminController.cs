using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; // Dodane do obsługi bazy danych dla słowników

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context) // Wstrzykiwanie kontekstu
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ==========================================
        // ZARZĄDZANIE UŻYTKOWNIKAMI I ROLAMI
        // ==========================================

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                model.Add(new UserWithRolesViewModel
                {
                    UserId = u.Id,
                    Email = u.Email ?? "(brak)",
                    Roles = roles.ToList(),
                    IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow 
                });
            }

            ViewBag.WszystkieRole = _roleManager.Roles.Select(r => r.Name!).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZmienRole(string userId, string nowaRola)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var obecne = await _userManager.GetRolesAsync(user);
            if (obecne.Any())
                await _userManager.RemoveFromRolesAsync(user, obecne);

            await _userManager.AddToRoleAsync(user, nowaRola);

            TempData["Sukces"] = $"Rola użytkownika {user.Email} → {nowaRola}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrzelaczBlokade(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Sprawdzamy, czy obecnie zablokowany
            var isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

            if (isLocked)
            {
                // Odblokowanie
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Sukces"] = $"Konto {user.Email} zostało odblokowane.";
            }
            else
            {
                // Zablokowanie na "zawsze"
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Sukces"] = $"Konto {user.Email} zostało zablokowane.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ZARZĄDZANIE SŁOWNIKAMI (SB5-05)
        // ==========================================

        public async Task<IActionResult> Slowniki()
        {
            ViewBag.Marki = await _context.Marki.ToListAsync();
            ViewBag.Modele = await _context.Modele.Include(m => m.Marka).ToListAsync();
            ViewBag.MarkiList = new SelectList(await _context.Marki.ToListAsync(), "Id", "Nazwa");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajMarke(string nazwaMarki)
        {
            if (!string.IsNullOrWhiteSpace(nazwaMarki))
            {
                _context.Marki.Add(new Marka { Nazwa = nazwaMarki });
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Dodano nową markę!";
            }
            return RedirectToAction(nameof(Slowniki));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajModel(string nazwaModelu, int markaId)
        {
            if (!string.IsNullOrWhiteSpace(nazwaModelu) && markaId > 0)
            {
                _context.Modele.Add(new Model { Nazwa = nazwaModelu, MarkaId = markaId });
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Dodano nowy model!";
            }
            return RedirectToAction(nameof(Slowniki));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunMarke(int id)
        {
            var marka = await _context.Marki.FindAsync(id);
            if (marka != null)
            {
                _context.Marki.Remove(marka);
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Marka została usunięta.";
            }
            return RedirectToAction(nameof(Slowniki));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunModel(int id)
        {
            var model = await _context.Modele.FindAsync(id);
            if (model != null)
            {
                _context.Modele.Remove(model);
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Model został usunięty.";
            }
            return RedirectToAction(nameof(Slowniki));
        }
    }

    public class UserWithRolesViewModel
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public bool IsLocked { get; set; } 
    }
}