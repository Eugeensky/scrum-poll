using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DAO;
public class PollSession
{
    private readonly AppDbContext db;
    internal PollSession(AppDbContext _db)
    {
        db = _db;
    }

    public async Task<DTO.PollSession?> GetOrCreatePollSession(string? userLogin, Guid pollId)
    {
        var pollAnswer = await db.PollSessions.Include(pa => pa.User).Include(pa => pa.Poll).ThenInclude(p => p.Options)
            .FirstOrDefaultAsync(pa => pa.PollId == pollId && pa.User != null && pa.User.Login == userLogin);

        if (pollAnswer is not null && pollAnswer.PollOptionId is not null) return null;
        if (pollAnswer is null)
        {
            var poll = await db.Polls.Include(p => p.Options).FirstAsync(p => p.Id == pollId);
            var user = await db.Users.FirstAsync(u => u.Login == userLogin);
            var newPollAnwer = new Entities.PollSession
            {
                Id = Guid.NewGuid(),
                Poll = poll,
                StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                User = user,
                
            };
            db.PollSessions.Add(newPollAnwer);
            await db.SaveChangesAsync();
            pollAnswer = newPollAnwer;
        }

        return new DTO.PollSession
        {
            Id = pollAnswer.Id,
            StartTime = pollAnswer.StartTime,
            PollDescription = pollAnswer.Poll?.Description ?? "",
            TimeInMinutes = pollAnswer.Poll?.TimeInMinutes ?? 0,
            Options = pollAnswer.Poll?.Options.Select(o => new DTO.PollOptionDescription { Id = o.Id, Description = o.Description }).ToList()
        };
    }

    public async Task<bool> SubmitPollAnswer(Guid sessionId, Guid optionId)
    {
        var pollSession = await db.PollSessions.AsNoTracking().Include(pa => pa.Poll).FirstOrDefaultAsync(pa => pa.Id == sessionId);
        if (pollSession is null) return false;
        var endTime = pollSession.StartTime + pollSession.Poll.TimeInMinutes * 60000 + 5000; // +5 sec delay
        if (endTime > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
        {
            var updatedCount = await db.Database.
                ExecuteSqlRawAsync("UPDATE PollSessions SET PollOptionId={0}, Finished=1 WHERE Id={1}", optionId, sessionId);
            return updatedCount == 1;
        }
        return false;
    }

    public async Task FinishPollSession(Guid sessionId)
    {
        var pollSession = await db.PollSessions.AsNoTracking().FirstAsync(pa => pa.Id == sessionId);
        if (pollSession.PollOptionId is null)
        {
            await db.Database.ExecuteSqlRawAsync("UPDATE PollSessions SET Finished=1 WHERE Id={0}", sessionId);
        }   
    }
}
