using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Pipelines;
using SocialNetwork.Hubs;
using SocialNetwork.Models.ViewModels.LiteChatViewModels;
using SocialNetwork.Models.LiteChatModels;

namespace SocialNetwork.Controllers
{

    public class ChatController : Controller
    {
        IHubContext<ChatHub> _hubContext { get; set; }  
        ILiteChatRoomsRepository _chatRoomsRepository { get; set; }
        public ChatController(IHubContext<ChatHub> hubContext, ILiteChatRoomsRepository liteChatRoomRepository)
        {
            _chatRoomsRepository = liteChatRoomRepository;
            _hubContext = hubContext;
        }

        public IActionResult Chat()
        {
            return View();
        }
        public IActionResult Index()
        {
            List<LiteChatRoom> rooms = _chatRoomsRepository.Rooms;
            List<LiteChatRoomViewModel> viewModel = new List<LiteChatRoomViewModel>(rooms.Count);
            foreach(LiteChatRoom room in rooms)
            {
                viewModel.Add(new LiteChatRoomViewModel() { Id = room.Id, Name = room.Name });
            }
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string roomName)
        {

            LiteChatRoom chatRoom;
            try
            {
                chatRoom = _chatRoomsRepository.CreateLiteChatRoom(roomName);
            }
            catch (LiteChatException ex)
            {
                return Conflict(ex.Message);
            }
            LiteChatRoomViewModel room = new LiteChatRoomViewModel() { Id = chatRoom.Id, Name = chatRoom.Name };
            await _hubContext.Clients.All.SendAsync("LiteChatRoomCreated", room);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> JoinToGroup(int roomId, string userName, string connectionId)
        {
            LiteChatRoom chatRoom = _chatRoomsRepository.GetLiteChatRoomById(roomId);
            await _hubContext.Groups.AddToGroupAsync(connectionId, chatRoom.Name);
            await _hubContext.Clients.GroupExcept(chatRoom.Name, connectionId).SendAsync("GroupNotify", $"Пользователь {userName} присоединился к чату", roomId, DateTime.Now.ToString("f"));
            List<OutSimpleMessageViewModel> messages = _chatRoomsRepository.GetLiteChatMessagesToView(roomId);
            messages.Add(new OutSimpleMessageViewModel()
            {
                Sender = "System",
                RoomId = roomId,
                Text = $"Вы присоединились к чату \"{chatRoom.Name}\"",
                DateTime = DateTime.Now.ToString("f")
            });
            return Json(messages);
        }

        public async Task<IActionResult> ExitFromGroup(int roomId, string userName, string connectionId)
        {
            var chatRoom = _chatRoomsRepository.GetLiteChatRoomById(roomId);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatRoom.Name);
            await _hubContext.Clients.Group(chatRoom.Name).SendAsync("GroupNotify", $"Пользователь {userName} покинул чат", roomId, DateTime.Now.ToString("f"));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Send(int roomId, InSimpleMessageViewModel sendingMessage)
        {
            var chatRoom = _chatRoomsRepository.GetLiteChatRoomById(roomId);
            SimpleMessage message;
            try
            {
                message = _chatRoomsRepository.SendMessageToLiteChatRoom(roomId, sendingMessage);
            }catch (LiteChatException ex)
            {
                return BadRequest(ex.Message);
            }
            OutSimpleMessageViewModel messageViewModel= new OutSimpleMessageViewModel() { Id = message.Id, RoomId = roomId, DateTime = message.DateTime.ToString("f"), Sender = message.SenderName, Text = message.Text };
            await _hubContext.Clients.Group(chatRoom.Name).SendAsync("Recieve", messageViewModel);
            return Ok();
        }
    }
}
