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
            await _hubContext.Clients.Group(chatRoom.Name).SendAsync("GroupNotify", $"Пользователь {userName} присоединился к чату");
            List<OutSimpleMessageViewModel> messages = _chatRoomsRepository.GetLiteChatMessagesToView(roomId);
            return Json(messages);
        }

        public async Task<IActionResult> ExitFromGroup(LiteChatRoomViewModel room, string userName, string connectionId)
        {
            await _hubContext.Clients.Group(room.Name).SendAsync("GroupNotify", $"Пользователь {userName} покинул чат");
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, room.Name);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Send(int roomId, InSimpleMessageViewModel sendingMessage)
        {
            SimpleMessage message;
            try
            {
                message = _chatRoomsRepository.SendMessageToLiteChatRoom(roomId, sendingMessage);
            }catch (LiteChatException ex)
            {
                return BadRequest(ex.Message);
            }
            OutSimpleMessageViewModel messageViewModel= new OutSimpleMessageViewModel() { Id = message.Id, DateTime = message.DateTime.ToString(), Sender = message.SenderName, Text = message.Text };
            await _hubContext.Clients.All.SendAsync("Recieve", messageViewModel);
            return Ok();
        }
    }
}
