using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class SocialNetworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
