using Ahmad.Mafia.Application.Contract.Room.Commands;
using Ahmad.Mafia.Application.Query.Queries;
using Ahmad.Mafia.Rest.EndPoints.Room;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.Mafia.Rest.Endpoints;

public sealed class RoomEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoomConstants.Routes.BaseRoute).WithTags("Room");

        group.MapPost(RoomConstants.Routes.CreateRoom, CreateRoom)
            .WithName(RoomConstants.Names.CreateRoom)
            .WithSummary(RoomConstants.Docs.CreateRoom.Summary);

        group.MapPost(RoomConstants.Routes.JoinRoom, JoinRoom)
            .WithName(RoomConstants.Names.JoinRoom)
            .WithSummary(RoomConstants.Docs.JoinRoom.Summary);

        group.MapPost(RoomConstants.Routes.QuickJoin, QuickJoin)
            .WithName(RoomConstants.Names.QuickJoin)
            .WithSummary(RoomConstants.Docs.QuickJoin.Summary);

        group.MapGet(RoomConstants.Routes.GetRoom, GetRoom)
            .WithName(RoomConstants.Names.GetRoom)
            .WithSummary(RoomConstants.Docs.GetRoom.Summary);

        group.MapPut(RoomConstants.Routes.StartRoom, StartRoom)
            .WithName(RoomConstants.Names.StartRoom)
            .WithSummary(RoomConstants.Docs.StartRoom.Summary);

        group.MapDelete(RoomConstants.Routes.LeaveRoom, LeaveRoom)
            .WithName(RoomConstants.Names.LeaveRoom)
            .WithSummary(RoomConstants.Docs.LeaveRoom.Summary);
    }

    private static async Task<IResult> CreateRoom(
        [FromBody] CreateRoomCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<CreateRoomResult>(command, ct);
        return Results.Ok(ApiResponse<CreateRoomResult>.Ok(result));
    }

    private static async Task<IResult> JoinRoom(
        [FromBody] JoinRoomCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<JoinRoomResult>(command, ct);
        return Results.Ok(ApiResponse<JoinRoomResult>.Ok(result));
    }

    private static async Task<IResult> QuickJoin(
        [FromBody] QuickJoinCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<QuickJoinResult>(command, ct);
        return Results.Ok(ApiResponse<QuickJoinResult>.Ok(result));
    }

    private static async Task<IResult> GetRoom(
        string code, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetRoomQueryResponse>(new GetRoomQuery(code), ct);
        return Results.Ok(ApiResponse<GetRoomQueryResponse>.Ok(result));
    }

    private static async Task<IResult> StartRoom(
        long id, [FromBody] StartRoomCommand command, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { RoomId = id }, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> LeaveRoom(
        long id, long playerId, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new LeaveRoomCommand(id, playerId), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }
}
