using Microsoft.AspNetCore.SignalR;

namespace ConCord.Hubs;
public class ChatHub : Hub
{
  public override Task OnConnectedAsync()
  {
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception exception)
  {
    return base.OnDisconnectedAsync(exception);
  }

  public async Task AddToGroup(string groupName)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
  }

  public async Task RemoveFromGroup(string groupName)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
  }
}