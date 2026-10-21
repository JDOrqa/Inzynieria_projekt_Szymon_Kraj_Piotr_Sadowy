using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KomisSamochodowy.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

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
                    Roles = roles.ToList()
                });
            }

            ViewBag.WszystkieRole = _roleManager.Roles
                                                 .Select(r => r.Name!)
                                                 .ToList();
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
    }

    public class UserWithRolesViewModel
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
    }
}