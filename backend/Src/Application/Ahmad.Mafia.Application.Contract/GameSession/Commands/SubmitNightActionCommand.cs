using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

public record SubmitNightActionCommand(
    [property: JsonIgnore] long GameSessionId,
    long ActorId,
    long TargetId,
    /// <summary>"Kill" | "Save" | "Investigate" — پیش‌فرض Kill برای سازگاری با سناریوی مافیای روسی.</summary>
    string ActionType = "Kill"
) : ICommand<long>;
