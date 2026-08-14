using Ahmad.Mafia.Application.Contract.Room.Commands;
using Ahmad.Mafia.Application.Room.Mapper;
using Ahmad.Mafia.Domain.Repositories;
using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Exceptions;
using Ahmad.Mafia.Persistence.EF;
using RoomAgg = Ahmad.Mafia.Domain.Room;

namespace Ahmad.Mafia.Application.Handlers;

public sealed class RoomHandlers(
    IRoomRepository repository,
    MafiaDbContext context) :
    ICommandHandler<CreateRoomCommand, CreateRoomResult>,
    ICommandHandler<JoinRoomCommand, JoinRoomResult>,
    ICommandHandler<LeaveRoomCommand, long>,
    ICommandHandler<StartRoomCommand, long>
{
    public async Task<CreateRoomResult> Handle(CreateRoomCommand command, CancellationToken token)
    {
        var id = await repository.GetNextIdAsync();
        var hostPlayerId = await repository.GetNextIdAsync();
        var roomCode = GenerateRoomCode();

        var room = RoomAgg.Aggregates.Room.Create(command.Map(id, roomCode, hostPlayerId));

        await repository.AddAsync(room, token);
        await context.CommitAsync(token);

        return new CreateRoomResult(room.Id, room.RoomCode, hostPlayerId);
    }

    public async Task<JoinRoomResult> Handle(JoinRoomCommand command, CancellationToken token)
    {
        var room = await repository.GetByCodeAsync(command.RoomCode, token)
            ?? throw new RoomNotFoundException();

        var playerId = await repository.GetNextIdAsync();
        room.Join(new JoinRoomArg(playerId, command.Nickname));

        await repository.UpdateAsync(room, token);
        await context.CommitAsync(token);

        return new JoinRoomResult(room.Id, playerId);
    }

    public async Task<long> Handle(LeaveRoomCommand command, CancellationToken token)
    {
        var room = await repository.GetByIdAsync(command.RoomId, token)
            ?? throw new RoomNotFoundException();

        room.Leave(command.PlayerId);

        await repository.UpdateAsync(room, token);
        await context.CommitAsync(token);
        return command.PlayerId;
    }

    public async Task<long> Handle(StartRoomCommand command, CancellationToken token)
    {
        var room = await repository.GetByIdAsync(command.RoomId, token)
            ?? throw new RoomNotFoundException();

        room.Start(command.RequestingPlayerId);

        await repository.UpdateAsync(room, token);
        await context.CommitAsync(token);
        return room.Id;
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var buffer = new char[6];
        for (var i = 0; i < buffer.Length; i++)
            buffer[i] = chars[Random.Shared.Next(chars.Length)];
        return new string(buffer);
    }
}
