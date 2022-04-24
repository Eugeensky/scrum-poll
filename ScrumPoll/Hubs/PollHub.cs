using Microsoft.AspNetCore.SignalR;
using DAL.DTO;

namespace ScrumPoll.Hubs;
public class PollHub : Hub
{
    public async Task AddNewPoll(PollDescription poll)
    {
        await Clients.All.SendAsync("UpdatePollList", poll);
    }

    public async Task PublishPoll(PollDescription poll)
    {
        await Clients.All.SendAsync("Publish", poll);
    }

    public async Task SendPollNotification(string message)
    {
        await Clients.All.SendAsync("PollNotification", message);
    }
}