using SpaceGame.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceGame
{
    public interface ITypedHubClient
    {
        Task UpdateGroupClients(List<Player> clients);
    }
}
