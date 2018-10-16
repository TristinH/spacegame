using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SpaceGame
{
    public class NotifyHub : Hub<ITypedHubClient>
    {
        public void Update(string groupName)
        {
            // TODO: Figure out how to update the correct group
            Clients.Group(groupName).UpdateGroupClients(null);
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            Groups.AddToGroupAsync(Context.ConnectionId, name);
            return base.OnConnectedAsync();
        }
    }
}
