using Ahmad.Mafia.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.Mafia.Application.Tests.Fakes;

/// <summary>InMemory MafiaDbContext برای تست‌ها</summary>
public static class FakeAppDb
{
    public static MafiaDbContext Create()
    {
        var opts = new DbContextOptionsBuilder<MafiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MafiaDbContext(opts);
    }
}
