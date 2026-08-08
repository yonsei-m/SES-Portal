using Microsoft.AspNetCore.SignalR;

namespace 社員_求人管理アプリ.Pages.Chat;

public class ChatHub : Hub
{
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }
    public async Task JoinUser(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendRead(string roomId, string userId)
    {
        await Clients
            .Group(roomId)
            .SendAsync("ReceiveRead", int.Parse(roomId), userId);
    }
}