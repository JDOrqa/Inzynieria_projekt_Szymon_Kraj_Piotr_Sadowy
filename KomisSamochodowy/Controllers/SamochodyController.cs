using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Policy = "SprzedawcaOrAdmin")]
    public class SamochodyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SamochodyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            if (id == null) return NotFound();

            var samochod = await _context.Samochody.FindAsync(id);
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
        public async Task<IActionResult> Create([Bind("Marka,Model,RokProdukcji,Cena,Przebieg,Paliwo,PlikZdjecia,Status,Opis,GaleriaZdjec")] Samochod samochod)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marka,Model,RokProdukcji,Cena,Przebieg,Paliwo,ZdjecieUrl,PlikZdjecia,Status,Opis,GaleriaZdjec")] Samochod samochod)
        {
            if (id != samochod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
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