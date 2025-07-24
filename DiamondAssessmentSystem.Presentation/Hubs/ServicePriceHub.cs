using Microsoft.AspNetCore.SignalR;

namespace DiamondAssessmentSystem.Presentation.Hubs
{
    public class ServicePriceHub : Hub
    {
        public async Task NotifyChange(string action, object data)
        {
            await Clients.All.SendAsync("ServicePriceChanged", action, data);
        }
    }
}
