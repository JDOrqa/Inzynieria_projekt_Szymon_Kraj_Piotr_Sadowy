using KomisSamochodowy.Data;
using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

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

            samochod.Status = "Zarezerwowany";
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Samochód został zarezerwowany.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa");
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa");
            return View();
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MarkaId,ModelId,RokProdukcji,Cena,Przebieg,Paliwo,ZdjecieUrl,PlikZdjecia,Status,Opis,GaleriaZdjec")] Samochod samochod)
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
            ViewBag.Marki = new SelectList(_context.Marki, "Id", "Nazwa", samochod.MarkaId);
            ViewBag.Modele = new SelectList(_context.Modele, "Id", "Nazwa", samochod.ModelId);
            return View(samochod);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var samochod = await _context.Samochody.Include(s => s.Marka).Include(s => s.Model).FirstOrDefaultAsync(m => m.Id == id);
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