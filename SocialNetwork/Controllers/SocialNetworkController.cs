using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Hubs;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.Repositories;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class SocialNetworkController : Controller
    {
        ISocialNetworkRepository _socialNetworkRepository;
        IUsersRepository _usersRepository;
        UserManager<NetworkUser> _userManager;
        IHubContext<SocialNetworkHub, ISocialNetworkHubClient> _hubContext;
        
        public SocialNetworkController(ISocialNetworkRepository socialNetworkRepository
                                      ,IUsersRepository usersRepository
                                      ,UserManager<NetworkUser> userManager
                                      ,IHubContext<SocialNetworkHub, ISocialNetworkHubClient> hubContext)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _usersRepository = usersRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public IActionResult Index(int? chatId = null)
        {
            return View(chatId);
        }

        public IActionResult Data()
        {
            string userId = _userManager.GetUserId(User);

            SocialLinksViewModel model = new SocialLinksViewModel()
            {
                Chats = _socialNetworkRepository.GetUserChats(userId),
                Friends = _usersRepository.GetFriends(userId),
            };
            foreach (var item in model.Friends)
            {
                item.UserPageLink = $"/User{item.Id}";
                item.ChatId = _socialNetworkRepository.GetUsersDialog(userId, item.Id)?.Id;
            }
            return Ok(model);
        }

        /// <summary>
        /// Приглашение в друзья
        /// </summary>
        /// <param name="userIsInvitedId"></param>
        /// <returns></returns>
        public IActionResult InviteFriend(string userIsInvitedId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.InviteFriend(userId, userIsInvitedId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            _hubContext.Clients.User(userIsInvitedId).FriendshipRequested(userId);
            return Ok();
        }

        /// <summary>
        /// Принятие предложения дружбы
        /// </summary>
        /// <param name="invitorId"></param>
        /// <returns></returns>
        public IActionResult AcceptFriendship(string invitorId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.AcceptFriendship(invitorId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            _hubContext.Clients.User(invitorId).FriendshipAccepted(userId);
            return Ok();
        }

        /// <summary>
        /// Присоединение к чату
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="connectionId">Id подключения</param>
        /// <returns></returns>
        public async Task<IActionResult> JoinToChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            try
            {
                chat = _socialNetworkRepository.JoinToChat(chatId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            await _hubContext.Clients.Group(chat.Name).ChatNotifyAsync($"Пользователь {thisUser.UserName} присоединился к коллективу", DateTime.Now.ToString());
            await _hubContext.Groups.AddToGroupAsync(connectionId, chat.Name);
            List<ChatMessageViewModel> messages = _socialNetworkRepository.GetChatMessages(chatId, userId);
            foreach (ChatMessageViewModel message in messages)
                message.SenderLink = "/User" + userId;
            return Ok(messages);
        }


        /// <summary>
        /// Выход из чата
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="connectionId">Id подключения</param>
        /// <returns></returns>
        public async Task<IActionResult> LeaveFromChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            try
            {
                chat = _socialNetworkRepository.LeaveFromChat(chatId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            await _hubContext.Clients.Group(chat.Name).ChatNotifyAsync($"Пользователь {thisUser.UserName} покинул коллектив", DateTime.Now.ToString());
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chat.Name);
            return Ok();
        }

        public async Task<IActionResult> ConnectToChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat = _socialNetworkRepository.GetChatById(chatId);
            List<ChatMessageViewModel> messages = _socialNetworkRepository.GetChatMessages(chatId, userId);
            HttpContext.Session.SetString("CurrentChatName", chat.Name);
            foreach (ChatMessageViewModel message in messages)
            {
                message.SenderLink = "/User" + message.SenderId;
                message.ChatId = chatId;
            }
            await _hubContext.Groups.AddToGroupAsync(connectionId, chat.Name);
            return Ok(messages);
        }
        public IActionResult ConnectToDialog(int chatId, string interlocutorId)
        {
            string userId = _userManager.GetUserId(User);
            
            GroupChat chat = _socialNetworkRepository.GetChatById(chatId);
            if (!string.IsNullOrEmpty(chat.Name)) return BadRequest();
            List<ChatMessageViewModel> messages = _socialNetworkRepository.GetChatMessages(chatId, userId);

            foreach (ChatMessageViewModel message in messages)
            {
                message.SenderLink = "/User" + message.SenderId;
                message.ChatId = chatId;
            }
            return Ok(messages);
        }
        public async Task<IActionResult> DisconnectFromChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat = _socialNetworkRepository.GetChatById(chatId);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chat.Name);
            HttpContext.Session.Remove("CurrentChatName");
            return Ok();
        }

        public IActionResult UserPage(string Id)
        {
            string userId = _userManager.GetUserId(User);
            FriendshipFact friendshipFact = _usersRepository.GetFriendshipFact(userId, Id);
            FriendshipFactStates friendshipFactState = FriendshipFactStates.Friends;
            NetworkUser networkUser = _usersRepository.GetUserById(Id);
            if (friendshipFact == null) friendshipFactState = FriendshipFactStates.NotFriends;
            else
            {
                if (friendshipFact.InitiatorId == userId && !friendshipFact.RequestAccepted) friendshipFactState = FriendshipFactStates.OutgoingInvitation;
                if (friendshipFact.InitiatorId == Id && !friendshipFact.RequestAccepted) friendshipFactState = FriendshipFactStates.IncomingInvitation;
            }
            UserPageViewModel userPageViewModel = new UserPageViewModel()
            {
                User = networkUser,
                FriendshipFactState = friendshipFactState,
            };
            return View(userPageViewModel);
        }

        [HttpPost]
        public IActionResult FilterUsers(UsersFilter filter)
        {
            string userId = _userManager.GetUserId(User);
            List<NetworkUser> users = _usersRepository.FilterUsers(filter, userId);
            List<ExtendedUserViewModel> filteredUsers = new List<ExtendedUserViewModel>(users.Count);
            foreach (NetworkUser user in users)
            {
                ExtendedUserViewModel createdModelItem = new ExtendedUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    BirthDate = user.BirthDate,
                    Age = user.Age,
                    Email = user.Email,
                    UserPageLink = $"User{user.Id}",
                    CityName = user.City?.Name,
                    CountryName = user.Country?.Name,
                };
                string fullName;
                if (!string.IsNullOrEmpty(user.FirstName))
                {
                    fullName = user.FirstName;
                    if (!string.IsNullOrEmpty(user.Surname))
                        fullName += " " + user.Surname;
                    fullName += $"({user.UserName})";
                }
                else 
                    fullName = user.UserName;

                createdModelItem.UserName = fullName;
                filteredUsers.Add(createdModelItem);
            }

            return PartialView("FilteredUsersPart", filteredUsers);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string text)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            Message message = await Task.Run(() => _socialNetworkRepository.SendMessageToChat(thisUser.Id, text, chatId));
            ChatMessageViewModel viewModel = new ChatMessageViewModel()
            {
                ChatId = chatId,
                SenderId = userId,
                SenderName = thisUser.UserName,
                Text = text,
                DateTime = message.DateTime.ToString("f"),
                SenderLink = $"/User{userId}"
            };
            await _hubContext.Clients.Group(message.Chat.Name).MessageRecieved(viewModel);
            return Ok();
        }

        public IActionResult GetDialogId(string interlocutorId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat = _socialNetworkRepository.EnsureGetUsersDialog(userId, interlocutorId);
            object obj = new { chatId = chat.Id };
            return Ok(chat.Id);
        }
        public async Task<IActionResult> SendMessageToInterlocutor(int chatId, string text, string interlocutorId)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            GroupChat chat = _socialNetworkRepository.GetChatById(chatId);
            Message message = await Task.Run(() => _socialNetworkRepository.SendMessageToChat(userId, text, chatId));
            ChatMessageViewModel viewModel = new ChatMessageViewModel()
            {
                ChatId = chatId,
                SenderId = userId,
                SenderName = thisUser.UserName,
                Text = text,
                DateTime = message.DateTime.ToString("f"),
                SenderLink = $"/User{userId}"
            };
            await _hubContext.Clients.Users(userId, interlocutorId).MessageRecieved(viewModel);
            return Ok();
        }
    }
}
