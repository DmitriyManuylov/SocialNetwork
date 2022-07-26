using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.WebSockets;

namespace SocialNetwork.Controllers
{
    public class ChatController : Controller
    {
        FileStream _stream;
        string path = "";
        public IActionResult JointoChat()
        {

            _stream = new FileStream(path, FileMode.OpenOrCreate);

            //WebSocket webSocket = WebSocketProtocol.CreateFromStream(_stream, true, WebSocket., WebSocket.DefaultKeepAliveInterval);
            //webSocket.
            return View();
        }

        
    }
}
