namespace Ahmad.Mafia.Domain.GameSession.Enums;

public enum Role
{
    SimpleCitizen = 0,
    SimpleMafia = 1,
}

public enum GamePhase
{
    Night = 0,
    Day = 1,
    Ended = 2,
}

public enum WinningTeam
{
    None = 0,
    Town = 1,
    Mafia = 2,
}

public enum ConnectionState
{
    Connected = 0,
    Disconnected = 1,
}
