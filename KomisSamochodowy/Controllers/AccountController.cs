using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KomisSamochodowy.Controllers
{
    [Authorize] // Tylko zalogowani mogą zmienić hasło
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult ZmianaHasla()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZmianaHasla(ZmianaHaslaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            // Zmiana hasła za pomocą wbudowanej metody Identity
            var result = await _userManager.ChangePasswordAsync(user, model.StareHaslo, model.NoweHaslo);

            if (result.Succeeded)
            {
                // Odświeżenie ciasteczka logowania, żeby sesja nie wygasła
                await _signInManager.RefreshSignInAsync(user);
                TempData["Sukces"] = "Hasło zostało pomyślnie zmienione.";
                return RedirectToAction("Index", "Home");
            }

            // Obsługa błędów (np. złe stare hasło)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }

    // Klasa pomocnicza dla formularza
    public class ZmianaHaslaViewModel
    {
        [Required(ErrorMessage = "Podaj stare hasło.")]
        [DataType(DataType.Password)]
        [Display(Name = "Stare hasło")]
        public string StareHaslo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj nowe hasło.")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć min. {2} znaków.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NoweHaslo { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NoweHaslo", ErrorMessage = "Nowe hasła nie są identyczne.")]
        public string PotwierdzNoweHaslo { get; set; } = string.Empty;
    }
}