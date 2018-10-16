using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpaceGame.Data.Models;
using SpaceGame.Data.Services;

namespace SpaceGame.Controllers.Api
{
    #region Request models
    public class StartGameRequest
    {
        public long GroupId { get; set; }
    }
    #endregion Request models

    [Produces("application/json")]
    public class GameController : Controller
    {
        private LobbyService _lobbyService;
        private GameService _gameService;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public GameController(IHubContext<NotifyHub, ITypedHubClient> hubContext, LobbyService lobbyService, GameService gameService)
        {
            _hubContext = hubContext;
            _lobbyService = lobbyService;
            _gameService = gameService;
        }

        [HttpPost("/game")]
        public JsonResult StartGame([FromBody] StartGameRequest request)
        {
            Group group = _lobbyService.GetGroupById(request.GroupId);
            _gameService.StartGame(group);

            return Json(new
            {
                Success = true
            });
        }
    }
}