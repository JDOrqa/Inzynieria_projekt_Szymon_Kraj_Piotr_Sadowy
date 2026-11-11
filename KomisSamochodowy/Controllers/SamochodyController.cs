using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity; // <-- DODANO
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace KomisSamochodowy.Controllers
{
    public class SamochodyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager; // <-- DODANO

        public SamochodyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager) // <-- DODANO userManager
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager; // <-- DODANO
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        public async Task<IActionResult> Index()
        {
            var samochody = _context.Samochody.Include(s => s.Marka).Include(s => s.Model);
            return View(await samochody.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Oferty(int? page)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var oferty = _context.Samochody
                .Include(s => s.Marka)
                .Include(s => s.Model)
                .Where(s => s.Status == "Dostępny" || s.Status == "Zarezerwowany")
                .OrderByDescending(s => s.Id);

            var listaOfert = await oferty.ToListAsync();
            return View(listaOfert.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var samochod = await _context.Samochody
                .Include(s => s.Marka)
                .Include(s => s.Model)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (samochod == null) return NotFound();

            var canViewSoldCar = User.IsInRole("Administrator") || User.IsInRole("Sprzedawca");
            if (samochod.Status == "Sprzedany" && !canViewSoldCar) return NotFound();

            return View(samochod);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int id)
        {
            var samochod = await _context.Samochody.FindAsync(id);
            if (samochod == null || samochod.Status != "Dostępny") return NotFound();

            // 1. Zmiana statusu auta
            samochod.Status = "Zarezerwowany";

            // 2. TWORZENIE WPISU W TABELI REZERWACJE (NOWY KOD)
            var userId = _userManager.GetUserId(User);

            var nowaRezerwacja = new Rezerwacja
            {
                SamochodId = id,
                KlientId = userId,
                DataRezerwacji = DateTime.Now,
                Status = "Aktywna"
            };

            _context.Rezerwacje.Add(nowaRezerwacja); // Zapisujemy rezerwację

            // 3. Zapis wszystkich zmian
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Samochód został zarezerwowany.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa");
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa");
            return View();
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarkaId,ModelId,RokProdukcji,Cena,Przebieg,Paliwo,PlikZdjecia,Status,Opis,GaleriaZdjec")] Samochod samochod)
        {
            if (ModelState.IsValid)
            {
                if (samochod.PlikZdjecia != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + samochod.PlikZdjecia.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await samochod.PlikZdjecia.CopyToAsync(fileStream);
                    }
                    samochod.ZdjecieUrl = "/uploads/" + uniqueFileName;
                }
                _context.Samochody.Add(samochod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa", samochod.MarkaId);
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa", samochod.ModelId);
            return View(samochod);
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var samochod = await _context.Samochody.FindAsync(id);
            if (samochod == null) return NotFound();

            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa", samochod.MarkaId);
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa", samochod.ModelId);
            return View(samochod);
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MarkaId,ModelId,RokProdukcji,Cena,Przebieg,Paliwo,ZdjecieUrl,PlikZdjecia,Status,Opis,GaleriaZdjec")] Samochod samochod)
        {
            if (id != samochod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Obsługa zdjęcia (to co już było)
                    if (samochod.PlikZdjecia != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + samochod.PlikZdjecia.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await samochod.PlikZdjecia.CopyToAsync(fileStream);
                        }
                        samochod.ZdjecieUrl = "/uploads/" + uniqueFileName;
                    }

                    // 2. NOWY KOD: Jeśli sprzedawca ustawia status na "Dostępny", anulujemy aktywne rezerwacje
                    if (samochod.Status == "Dostępny")
                    {
                        var aktywneRezerwacje = await _context.Rezerwacje
                            .Where(r => r.SamochodId == id && r.Status == "Aktywna")
                            .ToListAsync();

                        foreach (var rezerwacja in aktywneRezerwacje)
                        {
                            rezerwacja.Status = "Anulowana";
                        }
                    }

                    // 3. Zapis zmian w aucie i ewentualnych zmian w rezerwacjach
                    _context.Update(samochod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Samochody.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa", samochod.MarkaId);
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa", samochod.ModelId);
            return View(samochod);
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var samochod = await _context.Samochody.Include(s => s.Marka).Include(s => s.Model).FirstOrDefaultAsync(m => m.Id == id);
            if (samochod == null) return NotFound();
            return View(samochod);
        }

        [Authorize(Policy = "SprzedawcaOrAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var samochod = await _context.Samochody.FindAsync(id);
            if (samochod != null)
            {
                _context.Samochody.Remove(samochod);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}