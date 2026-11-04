using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using KomisSamochodowy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Policy = "SprzedawcaOrAdmin")]
    public class TransakcjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransakcjeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Widok Historii Sprzedaży
        public async Task<IActionResult> Index()
        {
            var transakcje = await _context.Transakcje
                .Include(t => t.Samochod)
                .OrderByDescending(t => t.DataTransakcji)
                .ToListAsync();

            // Pobieramy dane użytkowników do słowników, aby wyświetlić ich e-maile w widoku
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users.ToDictionary(u => u.Id, u => u.Email);

            return View(transakcje);
        }

        // 2. Formularz Sprzedaży (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UtworzTransakcjeViewModel();
            await PrzygotujListyRozwijane(viewModel);
            return View(viewModel);
        }

        // 3. Logika Biznesowa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtworzTransakcjeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var samochod = await _context.Samochody.FindAsync(model.SamochodId);
                if (samochod == null || samochod.Status == "Sprzedany")
                {
                    ModelState.AddModelError("", "Wybrany samochód jest niedostępny lub został już sprzedany.");
                    await PrzygotujListyRozwijane(model);
                    return View(model);
                }

                // Krok A: Tworzenie nowej transakcji
                var sprzedawcaId = _userManager.GetUserId(User); // ID aktualnie zalogowanego sprzedawcy

                var transakcja = new Transakcja
                {
                    DataTransakcji = model.DataTransakcji,
                    Kwota = model.Kwota,
                    SprzedawcaId = sprzedawcaId!,
                    KlientId = model.KlientId,
                    SamochodId = model.SamochodId
                };
                _context.Transakcje.Add(transakcja);

                // Krok B: Zmiana statusu samochodu na Sprzedany
                samochod.Status = "Sprzedany";

                // Krok C: Anulowanie ewentualnej aktywnej rezerwacji
                var aktywnaRezerwacja = await _context.Rezerwacje
                    .FirstOrDefaultAsync(r => r.SamochodId == model.SamochodId && r.Status == "Aktywna");

                if (aktywnaRezerwacja != null)
                {
                    aktywnaRezerwacja.Status = "Anulowana";
                }

                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Pomyślnie zarejestrowano sprzedaż pojazdu.";
                return RedirectToAction(nameof(Index));
            }

            await PrzygotujListyRozwijane(model);
            return View(model);
        }

        // Metoda pomocnicza do budowania list rozwijanych
        private async Task PrzygotujListyRozwijane(UtworzTransakcjeViewModel model)
        {
            // Pobieramy auta (Dostępne lub Zarezerwowane)
            model.DostepneSamochody = await _context.Samochody
                .Where(s => s.Status == "Dostępny" || s.Status == "Zarezerwowany")
                .OrderBy(s => s.Marka)
                .ToListAsync();

            // Pobieramy klientów
            var klienci = await _userManager.GetUsersInRoleAsync("Klient");
            model.Klienci = new SelectList(klienci, "Id", "Email", model.KlientId);
        }
    }
}