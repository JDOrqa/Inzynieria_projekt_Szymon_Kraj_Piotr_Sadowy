using KomisSamochodowy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Controllers
{
    [Authorize]
    public class RezerwacjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RezerwacjeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Wyświetlanie moich rezerwacji (Tylko dla Klienta)
        [Authorize(Roles = "Klient")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var mojeRezerwacje = await _context.Rezerwacje
                .Include(r => r.Samochod)
                    .ThenInclude(s => s.Marka)
                .Include(r => r.Samochod)
                    .ThenInclude(s => s.Model)
                .Where(r => r.KlientId == userId && r.Status == "Aktywna")
                .ToListAsync();

            return View(mojeRezerwacje);
        }

        // 2. Logika anulowania rezerwacji (Tylko dla Klienta)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Klient")]
        public async Task<IActionResult> Anuluj(int id)
        {
            var userId = _userManager.GetUserId(User);

            var rezerwacja = await _context.Rezerwacje
                .Include(r => r.Samochod)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rezerwacja == null || rezerwacja.KlientId != userId || rezerwacja.Status != "Aktywna")
            {
                return NotFound();
            }

            rezerwacja.Status = "Anulowana";

            if (rezerwacja.Samochod != null)
            {
                rezerwacja.Samochod.Status = "Dostępny";
            }

            await _context.SaveChangesAsync();
            TempData["Sukces"] = "Rezerwacja została anulowana, a samochód wrócił do oferty.";

            return RedirectToAction(nameof(Index));
        }

        // 3. Widok wszystkich aktywnych rezerwacji (Obejście ról - wpuszcza zalogowanych)
        [Authorize]
        public async Task<IActionResult> WszystkieRezerwacje()
        {
            var aktywneRezerwacje = await _context.Rezerwacje
                .Include(r => r.Samochod)
                    .ThenInclude(s => s.Marka)
                .Include(r => r.Samochod)
                    .ThenInclude(s => s.Model)
                .Where(r => r.Status == "Aktywna")
                .ToListAsync();

            // Pobranie danych użytkowników (maili), żeby wyświetlić kto zarezerwował
            var userIds = aktywneRezerwacje.Select(r => r.KlientId).Distinct().ToList();
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Email);
            ViewBag.Users = users;

            return View(aktywneRezerwacje);
        }

        // 4. Anulowanie rezerwacji z poziomu Admina
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AnulujPrzezAdmina(int id)
        {
            var rezerwacja = await _context.Rezerwacje
                .Include(r => r.Samochod)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rezerwacja == null || rezerwacja.Status != "Aktywna")
            {
                return NotFound();
            }

            rezerwacja.Status = "Anulowana";

            if (rezerwacja.Samochod != null)
            {
                rezerwacja.Samochod.Status = "Dostępny";
            }

            await _context.SaveChangesAsync();
            TempData["Sukces"] = "Rezerwacja klienta została przymusowo anulowana.";

            return RedirectToAction(nameof(WszystkieRezerwacje));
        }
    }
}