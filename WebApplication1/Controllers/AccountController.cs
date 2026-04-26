using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SubscriptionService _subscriptionService;
        
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            SubscriptionService subscriptionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _subscriptionService = subscriptionService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        return user.Role switch
                        {
                            "Admin" => RedirectToAction("Index", "Admin"),
                            "Barber" => RedirectToAction("Dashboard", "Barber"),
                            "Client" => RedirectToAction("Index", "Home"),
                            _ => RedirectToAction("Index", "Home")
                        };
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
            }
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role,
                    CreatedAt = DateTime.Now
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Add user to role
                    await _userManager.AddToRoleAsync(user, model.Role);
                    
                    // If barber, create empty profile and assign FREE subscription
                    if (model.Role == "Barber")
                    {
                        var dbContext = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
                        var barberProfile = new Models.BarberProfile
                        {
                            UserId = user.Id,
                            IsActive = true
                        };
                        dbContext.BarberProfiles.Add(barberProfile);
                        await dbContext.SaveChangesAsync();
                        
                        // Assign FREE subscription automatically
                        await _subscriptionService.AssignFreeSubscriptionAsync(user.Id);
                    }
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return user.Role switch
                    {
                        "Admin" => RedirectToAction("Index", "Admin"),
                        "Barber" => RedirectToAction("Dashboard", "Barber"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
