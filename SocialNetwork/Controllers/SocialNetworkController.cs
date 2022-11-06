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
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics.CodeAnalysis;
using SocialNetwork.Models.ViewModels.UsersViewModels;

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
                                      , IUsersRepository usersRepository
                                      , UserManager<NetworkUser> userManager
                                      , IHubContext<SocialNetworkHub, ISocialNetworkHubClient> hubContext)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _usersRepository = usersRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }


        [HttpGet]
        public IActionResult Index(int? chatId = null)
        {
            return View(chatId);
        }


        [HttpGet]
        public IActionResult Data()
        {
            string userId = _userManager.GetUserId(User);
            SocialLinksViewModel model;
            try
            {
                model = new SocialLinksViewModel()
                {
                    UserId = userId,
                    Chats = _socialNetworkRepository.GetUserChats(userId),
                    Friends = _usersRepository.GetFriends(userId),
                    Interlocutors = _usersRepository.GetInterlocutors(userId)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
        }


        /// <summary>
        /// Приглашение в друзья
        /// </summary>
        /// <param name="calledUserId"></param>
        /// <returns></returns>
        public IActionResult InviteFriend(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.InviteFriend(userId, calledUserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser user = _usersRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserPageLink = $"User{user.Id}",
            };
            userViewModel.SetFullName(user);
            _hubContext.Clients.User(calledUserId).FriendshipRequested(userViewModel);
            return Ok();
        }


        /// <summary>
        /// Принятие предложения дружбы
        /// </summary>
        /// <param name="calledUserId"></param>
        /// <returns></returns>
        public IActionResult AcceptFriendship(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.AcceptFriendship(calledUserId, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser user = _usersRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserPageLink = $"User{user.Id}"
            };
            userViewModel.SetFullName(user);

            _hubContext.Clients.User(calledUserId).FriendshipAccepted(userViewModel);
            return Ok();
        }


        public async Task<IActionResult> CancelFriendshipInvitation(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.DeleteFriend(userId, calledUserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser user = _usersRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserPageLink = $"User{user.Id}"
            };
            userViewModel.SetFullName(user);

            await _hubContext.Clients.User(calledUserId).FriendshipInvitationCanceled(userViewModel);

            return Ok();
        }


        public async Task<IActionResult> RejectFriendship(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.DeleteFriend(userId, calledUserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser user = _usersRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserPageLink = $"User{user.Id}"
            };
            userViewModel.SetFullName(user);

            await _hubContext.Clients.User(calledUserId).FriendshipRejected(userViewModel);

            return Ok();
        }


        public async Task<IActionResult> DeleteUserFromFriends(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            try
            {
                _usersRepository.DeleteFriend(userId, calledUserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser user = _usersRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserPageLink = $"User{user.Id}"
            };
            userViewModel.SetFullName(user);

            await _hubContext.Clients.User(calledUserId).DeletedByUserFromFriends(userViewModel);

            return Ok();
        }


        public async Task<IActionResult> CreateChat(string chatName, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            if (string.IsNullOrEmpty(chatName))
            {
                ModelState.AddModelError("chatName", "Не задано название чата");
                return BadRequest();
            }
            try
            {
                chat = _socialNetworkRepository.CreateChat(chatName, userId);
            }
            catch
            {
                return BadRequest();
            }
            ChatViewModel chatViewModel = new ChatViewModel()
            {
                Id = chat.Id,
                Name = chat.Name,
                ChatLink = $"Chat{chat.Id}",
                IsUserAMember = true
            };

            if (!string.IsNullOrEmpty(connectionId))
                await _hubContext.Groups.AddToGroupAsync(connectionId, chat.Name);

            return Ok(chatViewModel);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            await _hubContext.Clients.Group(chat.Name).ChatNotifyAsync($"Пользователь {thisUser.UserName} присоединился к коллективу", DateTime.Now.ToString());
            if (!string.IsNullOrEmpty(connectionId))
                await _hubContext.Groups.AddToGroupAsync(connectionId, chat.Name);

            return RedirectToAction(nameof(ChatPage), new { chatId = chatId });
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
            catch (Exception)
            {
                return BadRequest();
            }
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            await _hubContext.Clients.Group(chat.Name).ChatNotifyAsync($"Пользователь {thisUser.UserName} покинул коллектив", DateTime.Now.ToString());
            if (!string.IsNullOrEmpty(connectionId))
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chat.Name);
            return RedirectToAction(nameof(ChatPage), new { chatId = chatId });
        }


        public async Task<IActionResult> ConnectToChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            List<ChatMessageViewModel> messages;
            try
            {
                chat = _socialNetworkRepository.GetChatById(chatId);
                messages = _socialNetworkRepository.GetChatMessages(chatId, userId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            foreach (ChatMessageViewModel message in messages)
            {
                message.SenderLink = "/User" + message.SenderId;
                message.ChatId = chatId;
            }
            await _hubContext.Groups.AddToGroupAsync(connectionId, chat.Name);
            return Ok(messages);
        }


        public async Task<IActionResult> DisconnectFromChat(int chatId, string connectionId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            try
            {
                chat = _socialNetworkRepository.GetChatById(chatId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chat.Name);

            return Ok();
        }


        public IActionResult GetDialogId(string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            GroupChat chat;
            Dialog dialog;
            try
            {
                dialog = _socialNetworkRepository.EnsureGetUsersDialog(userId, calledUserId);
                chat = dialog.Chat;
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(chat.Id);
        }


        public IActionResult ConnectToDialog(int chatId, string calledUserId)
        {
            string userId = _userManager.GetUserId(User);

            List<ChatMessageViewModel> messages;
            try
            {
                messages = _socialNetworkRepository.GetDialogMessages(chatId, userId, calledUserId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(messages);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string text)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser thisUser = await _userManager.GetUserAsync(User);

            Message message;
            try
            {
                message = await Task.Run(() => _socialNetworkRepository.SendMessageToChat(thisUser.Id, text, chatId));
            }
            catch (Exception)
            {
                return BadRequest();
            }

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


        public async Task<IActionResult> SendMessageToInterlocutor(int chatId, string text, string calledUserId)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser thisUser = await _userManager.GetUserAsync(User);
            GroupChat chat;
            Message message;
            try
            {
                chat = _socialNetworkRepository.GetChatById(chatId);
                message = await Task.Run(() => _socialNetworkRepository.SendMessageToDialog(userId, text, calledUserId));
            }
            catch (Exception)
            {
                return BadRequest();
            }

            ChatMessageViewModel viewModel = new ChatMessageViewModel()
            {
                ChatId = chatId,
                SenderId = userId,
                SenderName = thisUser.UserName,
                Text = text,
                DateTime = message.DateTime.ToString("f"),
                SenderLink = $"/User{userId}"
            };
            await _hubContext.Clients.Users(userId, calledUserId).MessageRecieved(viewModel);

            return Ok();
        }


        public IActionResult UserPage(string Id)
        {
            string userId = _userManager.GetUserId(User);
            NetworkUser networkUser;
            FriendshipFact friendshipFact;
            try
            {
                networkUser = _usersRepository.GetUserById(Id);
                friendshipFact = _usersRepository.GetFriendshipFact(userId, Id);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            FriendshipFactStates friendshipFactState = FriendshipFactStates.Friends;
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
                Friends = _usersRepository.GetFriends(userId).Select(user => new UserViewModel { Id = userId, UserName = user.UserName, UserPageLink = $"User{user.Id}" }).ToList(),
                Chats = _socialNetworkRepository.GetUserChats(userId)
            };
            return View(userPageViewModel);
        }


        [HttpPost]
        public IActionResult FilterUsers(UsersFilter filter)
        {
            string userId = _userManager.GetUserId(User);
            List<NetworkUser> users;
            List<ExtendedUserViewModel> filteredUsers;
            try
            {
                users = _usersRepository.FilterUsers(filter, userId);
                filteredUsers = new List<ExtendedUserViewModel>(users.Count);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            foreach (NetworkUser user in users)
            {
                ExtendedUserViewModel createdModelItem = new ExtendedUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.Surname,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Age = user.Age,
                    Email = user.Email,
                    UserPageLink = $"User{user.Id}",
                    CityName = user.City?.Name,
                    CountryName = user.Country?.Name,
                };

                filteredUsers.Add(createdModelItem);
            }

            return PartialView("FilteredUsersPart", filteredUsers);
        }

        public IActionResult ChatPage(int chatId) 
        {
            string userId = _userManager.GetUserId(User);
            ChatViewModelWithUsers chat = _socialNetworkRepository.GetChatPageViewModel(chatId);
            chat.IsUserAMember = false;

            foreach (var member in chat.Members)
            {
                if (member.Id == userId)
                {
                    chat.IsUserAMember = true;
                    break;
                }
            }
            return View(chat); 
        }

        public IActionResult Chats()
        {
            string userId = _userManager.GetUserId(User);
            var userChats = _socialNetworkRepository.GetUserChats(userId);
            return View(userChats);
        }

        public IActionResult FilterChats(ChatFilter filter)
        {
            string userId = _userManager.GetUserId(User);
            List<ChatViewModel> filteredChats = _socialNetworkRepository.FilterChats(filter, userId);

            return PartialView("FilteredChatsPart", filteredChats);
        }

    }
}
