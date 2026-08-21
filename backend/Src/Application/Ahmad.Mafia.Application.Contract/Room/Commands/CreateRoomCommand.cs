using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public sealed record CreateRoomResult(long RoomId, string RoomCode, long HostPlayerId);

public record CreateRoomCommand(
    string HostNickname,
    int Capacity,
    /// <summary>"Public" | "Private" — ورودی رشته‌ای است تا این لایه به enum دامنه وابسته نشود.</summary>
    string Visibility = "Private",
    /// <summary>"RussianMafia" | "MafiaNights" — ورودی رشته‌ای است تا این لایه به enum دامنه وابسته نشود.</summary>
    string Scenario = "RussianMafia"
) : ICommand<CreateRoomResult>;
