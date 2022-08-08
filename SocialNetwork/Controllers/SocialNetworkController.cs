using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.Repositories;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class SocialNetworkController : Controller
    {
        ISocialNetworkRepository _socialNetworkRepository;
        IUsersRepository _usersRepository;
        UserManager<NetworkUser> _userManager;
        string userId;
        public SocialNetworkController(ISocialNetworkRepository socialNetworkRepository, IUsersRepository usersRepository, UserManager<NetworkUser> userManager)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _usersRepository = usersRepository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            userId = _userManager.GetUserId(User);
            return View();
        }

        public IActionResult InviteFriend(string userIsInvitedId)
        {
            try
            {
                _usersRepository.InviteFriend(userId, userIsInvitedId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        public IActionResult AcceptFriendship(string invitorId)
        {
            try
            {
                _usersRepository.AcceptFriendship(invitorId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        public IActionResult JoinToChat(int chatId)
        {
            try
            {
                _socialNetworkRepository.JoinToChat(chatId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        
        public IActionResult LeaveFromChat(int chatId)
        {
            try
            {
                _socialNetworkRepository.LeaveFromChat(chatId, userId);
            }
            catch (ChatException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        //public IActionResult SendMessage()
    }
}
