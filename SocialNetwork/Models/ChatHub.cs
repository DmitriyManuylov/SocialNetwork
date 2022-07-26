using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class ChatHub: Hub
    {

        public async Task Send(string sender, string message)
        {
            await Clients.All.SendAsync("Recieve", sender, message);
        }
        public async override Task OnConnectedAsync()
        {
           await Clients.All.SendAsync($"Пользователь {Context.ConnectionId} подключился");
        }
    }
}
