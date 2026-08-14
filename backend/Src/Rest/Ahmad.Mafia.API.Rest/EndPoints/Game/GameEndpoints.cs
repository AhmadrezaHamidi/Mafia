using Ahmad.Mafia.Application.Contract.GameSession.Commands;
using Ahmad.Mafia.Application.Query.Queries;
using Ahmad.Mafia.Rest.EndPoints.Game;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.Mafia.Rest.Endpoints;

public sealed class GameEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(GameConstants.Routes.BaseRoute).WithTags("Game");

        group.MapGet(GameConstants.Routes.GetState, GetState)
            .WithName(GameConstants.Names.GetState)
            .WithSummary(GameConstants.Docs.GetState.Summary);

        group.MapGet(GameConstants.Routes.GetResult, GetResult)
            .WithName(GameConstants.Names.GetResult)
            .WithSummary(GameConstants.Docs.GetResult.Summary);

        group.MapPost(GameConstants.Routes.SubmitNightAction, SubmitNightAction)
            .WithName(GameConstants.Names.SubmitNightAction)
            .WithSummary(GameConstants.Docs.SubmitNightAction.Summary);

        group.MapPost(GameConstants.Routes.CastVote, CastVote)
            .WithName(GameConstants.Names.CastVote)
            .WithSummary(GameConstants.Docs.CastVote.Summary);

        group.MapDelete(GameConstants.Routes.RetractVote, RetractVote)
            .WithName(GameConstants.Names.RetractVote)
            .WithSummary(GameConstants.Docs.RetractVote.Summary);

        group.MapPost(GameConstants.Routes.Rematch, Rematch)
            .WithName(GameConstants.Names.Rematch)
            .WithSummary(GameConstants.Docs.Rematch.Summary);
    }

    private static async Task<IResult> GetState(
        long id, long requestingPlayerId, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetGameStateQueryResponse>(
            new GetGameStateQuery(id, requestingPlayerId), ct);
        return Results.Ok(ApiResponse<GetGameStateQueryResponse>.Ok(result));
    }

    private static async Task<IResult> GetResult(
        long id, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetGameResultQueryResponse>(new GetGameResultQuery(id), ct);
        return Results.Ok(ApiResponse<GetGameResultQueryResponse>.Ok(result));
    }

    private static async Task<IResult> SubmitNightAction(
        long id, [FromBody] SubmitNightActionCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { GameSessionId = id }, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> CastVote(
        long id, [FromBody] CastVoteCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { GameSessionId = id }, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> RetractVote(
        long id, long voterId, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new RetractVoteCommand(id, voterId), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> Rematch(
        long id, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new RequestRematchCommand(id), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }
}
