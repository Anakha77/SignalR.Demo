using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class LoginHub : Hub
    {
        public async Task SendLogoutMessage(string userId)
        {
            if (Clients == null)
                return;

            await Clients.All.SendAsync("UserLoggedOut");
        }

        public async Task SendLoginMessage(string userId)
        {
            if (Clients == null)
                return;

            await Clients.All.SendAsync("UserLoggedIn");
        }
    }
}
