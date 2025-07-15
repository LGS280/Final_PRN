using DiamondAssessmentSystem.Application.DTO;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var conversationId = httpContext?.Request.Query["conversationId"];

            if (!string.IsNullOrEmpty(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var conversationId = Context.GetHttpContext()?.Request.Query["conversationId"];
            if (!string.IsNullOrEmpty(conversationId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Gọi từ Client để gửi tin nhắn → phát ra cho group
        public async Task SendToConversation(string conversationId, MessageResponseDTO message)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", message);
        }
    }
}
