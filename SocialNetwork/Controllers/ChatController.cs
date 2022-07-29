using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Models;
using System.IO;

namespace SocialNetwork.Controllers
{
    public class ChatController : Controller
    {
        IHubContext<ChatHub> _hubContext { get; set; }  
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            
            _hubContext = hubContext;
        }

        [Authorize]
        public IActionResult Chat()
        {
           // _hubContext.Clients.Client("").
            return View();
        }

        public IActionResult Send(Message message)
        {

            return View(message);
        }
    }
}
