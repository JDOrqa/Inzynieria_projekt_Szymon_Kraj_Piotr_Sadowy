using KomisSamochodowy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Roles = "Klient")]
    public class RezerwacjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RezerwacjeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Wyświetlanie moich rezerwacji
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

        // 2. Logika anulowania rezerwacji
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anuluj(int id)
        {
            var userId = _userManager.GetUserId(User);

            var rezerwacja = await _context.Rezerwacje
                .Include(r => r.Samochod)
                .FirstOrDefaultAsync(r => r.Id == id);

            // Zabezpieczenie: sprawdzamy, czy rezerwacja istnieje, należy do tego klienta i jest aktywna
            if (rezerwacja == null || rezerwacja.KlientId != userId || rezerwacja.Status != "Aktywna")
            {
                return NotFound();
            }

            // Zmiana statusu rezerwacji
            rezerwacja.Status = "Anulowana";

            // Przywrócenie auta do oferty (status Dostępny)
            if (rezerwacja.Samochod != null)
            {
                rezerwacja.Samochod.Status = "Dostępny";
            }

            await _context.SaveChangesAsync();
            TempData["Sukces"] = "Rezerwacja została anulowana, a samochód wrócił do oferty.";

            return RedirectToAction(nameof(Index));
        }
    }
}