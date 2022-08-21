using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.AccountViewModels;
using SocialNetwork.Models.Repositories;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        UserManager<NetworkUser> _userManager;
        SignInManager<NetworkUser> _signInManager;
        IUsersRepository _usersRepository;
        public AccountController(UserManager<NetworkUser> userManager, SignInManager<NetworkUser> signInManager, IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (model.BirthDate.HasValue)
            {
                if (model.BirthDate > System.DateTime.Now)
                    ModelState.AddModelError("BirthDate", "Вы не из будущего, не врите!");
            }

            NetworkUser user = await _userManager.FindByNameAsync(model.Name);
            if (user != null)
            {
                ModelState.AddModelError("Name", "Пользователь с таким именем уже существует");
            }
            else
            {
                user = new NetworkUser(model.Name);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.City))
            {
                _usersRepository.SetCityInUsersInfo(model.City, user);
            }

            if (!string.IsNullOrEmpty(model.Country))
            {
                _usersRepository.SetCityInUsersInfo(model.Country, user);
            }

            if (string.IsNullOrEmpty(model.Name))
                user.FirstName = model.FirstName;
            if (string.IsNullOrEmpty(model.LastName))
                user.Surname = model.LastName;
            if (string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (model.BirthDate.HasValue)
                user.BirthDate = model.BirthDate;

            if (model.BirthDate.HasValue)
            {
                user.BirthDate = model.BirthDate;
                user.SetAge();
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

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(ChatController.Chat), nameof(ChatController).Replace("Controller", ""));
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

        [Authorize]
        [HttpGet]
        public IActionResult EditUserData()
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser user = _usersRepository.GetUserById(userId);
            EditUserDataViewModel model = new EditUserDataViewModel()
            {
                Name = user.UserName,
                FirstName = user.FirstName,
                LastName = user.Surname,
                Email = user.Email,
                BirthDate = user.BirthDate,
                City = user.City?.Name,
                Country = user.Country?.Name,
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditUserData(EditUserDataViewModel model)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser user = _usersRepository.GetUserById(userId);

            if (model.BirthDate.HasValue)
            {
                if (model.BirthDate > System.DateTime.Now)
                    ModelState.AddModelError("BirthDate", "Вы не из будущего, не врите!");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.City))
            {
                _usersRepository.SetCityInUsersInfo(model.City, user);
            }

            if (!string.IsNullOrEmpty(model.Country))
            {
                _usersRepository.SetCountryInUsersInfo(model.Country, user);
            }

            if (string.IsNullOrEmpty(model.Name))
                user.FirstName = model.FirstName;
            if (string.IsNullOrEmpty(model.LastName))
                user.Surname = model.LastName;
            if (string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (model.BirthDate.HasValue)
            {
                user.BirthDate = model.BirthDate;
                user.SetAge();
            }
            _usersRepository.UpdateUser(user);
            return RedirectToAction(nameof(Index),nameof(SocialNetworkController).Replace("Controller",""));
        }
    }
}
