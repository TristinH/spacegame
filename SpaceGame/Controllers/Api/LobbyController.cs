using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpaceGame.Data.Models;
using SpaceGame.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SpaceGame.Controllers.Api
{
    #region Request models
    public class StartGroupRequest
    {
        public string HostPlayerName { get; set; }
        public string GroupName { get; set; }
    }

    public class AddPlayerRequest
    {
        public string PlayerName { get; set; }
    }
    #endregion Request models

    [Produces("application/json")]
    public class LobbyController : Controller
    {
        private LobbyService _lobbyService;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private UserManager<IdentityUser> _userManager;

        public LobbyController(LobbyService groupService, 
            IHubContext<NotifyHub, ITypedHubClient> hubContext, 
            UserManager<IdentityUser> userManager)
        {
            _lobbyService = groupService;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("lobby")]
        public JsonResult GetJoinableGroups()
        {
            List<Group> groups = _lobbyService.GetActiveGroups().ToList();
            return Json(groups);
        }

        [Authorize]
        [HttpPost("lobby")]
        public async Task<JsonResult> StartGroup([FromBody] StartGroupRequest requestInfo)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            Group group = _lobbyService.StartGroup(user, requestInfo);
            if (group == null)
            {
                return Json(new
                {
                    ErrorMessage = "Could not create group"
                });
            }

            return Json(group);
        }

        [AllowAnonymous]
        [HttpGet("lobby/{id}")]
        public ActionResult GetGroup(long id)
        {
            Group group = _lobbyService.GetGroupById(id);
            if (group == null)
            {
                return NotFound(new
                {
                    ErrorMessage = "Group not found"
                });
            }

            return Json(group);
        }

        [Authorize]
        [HttpDelete("lobby/{id}")]
        public async Task<ActionResult> DeactivateGroup(long id)
        {
            Task<IdentityUser> getUserTask = _userManager.GetUserAsync(User);
            Group group = _lobbyService.GetGroupById(id);
            if (group == null)
            {
                return NotFound(new
                {
                    ErrorMessage = "Group not found"
                });
            }

            IdentityUser currentUser = await getUserTask;
            if (currentUser.Id != group.HostPlayer.User.Id)
            {
                return StatusCode((int) HttpStatusCode.Forbidden, new
                {
                    ErrorMessage = "Can't deactivate group you're not the host of"
                });
            }

            _lobbyService.DeactivateGroup(group);
            return Json(new
            {
                Success = true
            });
        }

        [AllowAnonymous]
        [HttpGet("lobby/{id}/clients")]
        public ActionResult GetGroupClients(long id)
        {
            Group group = _lobbyService.GetGroupById(id);
            if (group == null)
            {
                return NotFound(new
                {
                    ErrorMessage = "Group not found"
                });
            }

            return Json(group.Clients);
        }

        [Authorize]
        [HttpPost("lobby/{id}/clients")]
        public async Task<ActionResult> AddPlayer(long id, [FromBody] AddPlayerRequest requestInfo)
        {
            Task<IdentityUser> getUserTask = _userManager.GetUserAsync(User);
            Group group = _lobbyService.GetGroupById(id);
            if (group == null)
            {
                return NotFound(new
                {
                    ErrorMessage = "Group not found"
                });
            }
            else if (!group.IsActive || group.IsInGame)
            {
                return StatusCode((int) HttpStatusCode.Forbidden, new
                {
                    ErrorMessage = "Can't change group"
                });
            }

            IdentityUser user = await getUserTask;
            List<Player> clients = _lobbyService.AddPlayerToGroup(user, group, requestInfo);
            await _hubContext.Clients.All.UpdateGroupClients(clients);

            return Json(clients);
        }

        [Authorize]
        [HttpDelete("lobby/{id}/clients/{playerId}")]
        public async Task<ActionResult> RemovePlayer(long id, long playerId)
        {
            Task<IdentityUser> getUserTask = _userManager.GetUserAsync(User);
            Group group = _lobbyService.GetGroupById(id);
            if (group == null)
            {
                return NotFound(new
                {
                    ErrorMessage = "Group not found"
                });
            }
            else if (!group.IsActive || group.IsInGame)
            {
                return StatusCode((int) HttpStatusCode.Forbidden, new
                {
                    ErrorMessage = "Can't change group"
                });
            }

            IdentityUser user = await getUserTask;
            if (user.Id != group.HostPlayer.User.Id || user.Id != group.Clients.FirstOrDefault(g => g.Id == playerId)?.User.Id)
            {
                return StatusCode((int) HttpStatusCode.Forbidden, new
                {
                    ErrorMessage = "You don't have permission to remove this user"
                });
            }

            List<Player> clients = _lobbyService.RemovePlayerFromGroup(group, playerId);
            await _hubContext.Clients.All.UpdateGroupClients(clients);

            return Json(clients);
        }
    }
}
