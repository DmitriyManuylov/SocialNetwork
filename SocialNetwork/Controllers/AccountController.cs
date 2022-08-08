using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Models;
using Microsoft.AspNetCore.Identity;
using SocialNetwork.Models.ViewModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        UserManager<NetworkUser> _userManager;
        SignInManager<NetworkUser> _signInManager;
        public AccountController(UserManager<NetworkUser> userManager, SignInManager<NetworkUser> signInManager, SocialNetworkDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registrationModel)
        {
            //HttpContext.Request.Form.
            if(!ModelState.IsValid)
            {
                return View(registrationModel);
            }
            NetworkUser user = await _userManager.FindByNameAsync(registrationModel.Name);
            if (user != null)
            {
                ModelState.AddModelError("Name", "Пользователь с таким именем уже существует");
                return View(registrationModel);
            }
            
            user = new NetworkUser(registrationModel.Name);
            user.Email = registrationModel.Email;

            var result = await _userManager.CreateAsync(user, registrationModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                   ModelState.AddModelError("", error.Description);
                return View(registrationModel);
            }
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
            
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            NetworkUser user = await _userManager.FindByNameAsync(loginModel.Name);
            
            if(user == null)
            {
                ModelState.AddModelError(nameof(loginModel.Name), "Пользователь с таким именем не существует");
                return View(loginModel);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(nameof(loginModel.Password), "Неверный пароль");
                return View(loginModel);
               
            }

            return RedirectToAction(nameof(SocialNetworkController.Index), nameof(SocialNetworkController).Replace("Controller", ""));

        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Settings(NetworkUser User)
        {
            return View();
        }
    }
}
