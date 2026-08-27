using Ahmad.Mafia.Application.Contract.Identity.Commands;
using Ahmad.Mafia.Rest.EndPoints.Identity;
using AhmadBase.Application;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.Mafia.Rest.Endpoints;

public sealed class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(AuthConstants.Routes.BaseRoute).WithTags("Auth");

        group.MapPost(AuthConstants.Routes.RequestOtp, RequestOtp)
            .WithName(AuthConstants.Names.RequestOtp)
            .WithSummary(AuthConstants.Docs.RequestOtp.Summary)
            .WithDescription(AuthConstants.Docs.RequestOtp.Description);

        group.MapPost(AuthConstants.Routes.VerifyOtp, VerifyOtp)
            .WithName(AuthConstants.Names.VerifyOtp)
            .WithSummary(AuthConstants.Docs.VerifyOtp.Summary)
            .WithDescription(AuthConstants.Docs.VerifyOtp.Description);
    }

    private static async Task<IResult> RequestOtp(
        [FromBody] RequestOtpCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<RequestOtpResult>(command, ct);
        return Results.Ok(ApiResponse<RequestOtpResult>.Ok(result));
    }

    private static async Task<IResult> VerifyOtp(
        [FromBody] VerifyOtpCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<VerifyOtpResult>(command, ct);
        return Results.Ok(ApiResponse<VerifyOtpResult>.Ok(result));
    }
}
