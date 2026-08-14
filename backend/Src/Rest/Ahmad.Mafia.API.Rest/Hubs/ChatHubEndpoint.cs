using AhmadBase.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Ahmad.Mafia.Rest.Hubs;

/// <summary>
/// map کردن hub از طریق همان الگوی IEndpoint که WebBuilder اسکن می‌کند،
/// چون RunAsync داخل پکیج AhmadBase است و به app دسترسی مستقیم نداریم.
/// </summary>
public sealed class ChatHubEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapHub<ChatHub>("/hubs/chat");
    }
}
