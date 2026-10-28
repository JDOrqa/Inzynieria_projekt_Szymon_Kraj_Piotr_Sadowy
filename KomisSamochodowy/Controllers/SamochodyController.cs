using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Policy = "SprzedawcaOrAdmin")]
    public class SamochodyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SamochodyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Samochody.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Oferty()
        {
            var oferty = await _context.Samochody
                .Where(s => s.Status == "Dostępny" || s.Status == "Zarezerwowany")
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return View(oferty);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var samochod = await _context.Samochody.FindAsync(id);

            if (samochod == null)
                return NotFound();

            var canViewSoldCar = User.IsInRole("Administrator") || User.IsInRole("Sprzedawca");
            if (samochod.Status == "Sprzedany" && !canViewSoldCar)
                return NotFound();

            return View(samochod);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int id)
        {
            var samochod = await _context.Samochody.FindAsync(id);

            if (samochod == null || samochod.Status != "Dostępny")
                return NotFound();

            samochod.Status = "Zarezerwowany";
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Samochód został zarezerwowany.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Marka,Model,RokProdukcji,Cena,Przebieg,Paliwo,ZdjecieUrl,Status,Opis,GaleriaZdjec")] Samochod samochod)
        {
            if (ModelState.IsValid)
            {
                _context.Samochody.Add(samochod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(samochod);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var samochod = await _context.Samochody.FindAsync(id);
            if (samochod == null) return NotFound();

            return View(samochod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marka,Model,RokProdukcji,Cena,Przebieg,Paliwo,ZdjecieUrl,Status,Opis,GaleriaZdjec")] Samochod samochod)
        {
            if (id != samochod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(samochod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(samochod);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var samochod = await _context.Samochody.FirstOrDefaultAsync(m => m.Id == id);
            if (samochod == null) return NotFound();

            return View(samochod);
        }

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