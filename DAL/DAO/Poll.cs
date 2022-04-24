using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DAO;
public class Poll
{
    private readonly AppDbContext db;
    internal Poll(AppDbContext _db)
    {
        db = _db;
    }

    public async Task<Guid> Add(string description, int timeInMinutes, IEnumerable<string> options)
    {
        var optionsList = options.Select(option => new PollOption() { Id = Guid.NewGuid(), Description = option }).ToList();
        Entities.Poll poll = new()
        {
            Id = Guid.NewGuid(),
            Description = description,
            TimeInMinutes = timeInMinutes,
            Options = optionsList
        };

        db.PollOptions.AddRange(optionsList);
        db.Polls.Add(poll);
        await db.SaveChangesAsync();
        return poll.Id;
    }

    public async Task<List<DTO.PollDescription>> GetActivePollDescriptionList(string? userLogin)
    {
        var polls = await db.Polls.Include(p => p.Sessions).ThenInclude(a => a.User)
            .Where(p => !p.Published && !p.Sessions.Any(a => a.User.Login == userLogin && a.Finished))
            .ToListAsync();

        return polls.Select(poll => new DTO.PollDescription { Id = poll.Id, Description = poll.Description }).ToList();
    }

    public async Task<List<DTO.PollDescription>> GetAllPublished()
    {
        var polls = await db.Polls.Where(p => p.Published).ToListAsync();
        return polls.Select(poll => new DTO.PollDescription { Id = poll.Id, Description = poll.Description }).ToList();
    }

    public async Task<List<DTO.PollDescription>> GetAllUnpublished()
    {
        var polls = await db.Polls.Where(p => !p.Published).ToListAsync();
        return polls.Select(poll => new DTO.PollDescription { Id = poll.Id, Description = poll.Description }).ToList();
    }

    public async Task<bool> Publish(Guid id)
    {
        var updatedCount = await db.Database.ExecuteSqlRawAsync("UPDATE Polls SET Published=1 WHERE Id={0}", id);
        return updatedCount == 1;
    }

    public async Task<string?> GetPollDescription(Guid id)
    {
        return (await db.Polls.FindAsync(id))?.Description;
    }

    public async Task<DTO.PollResult> GetResult(Guid id)
    {
        var poll = await db.Polls.Include(p => p.Sessions).Include(p => p.Options).ThenInclude(o => o.Sessions)
            .FirstAsync(p => p.Id == id);
        return new DTO.PollResult
        {
            Description = poll.Description,
            VotesCount = poll.Sessions?.Count ?? 0,
            PollOptionResults = poll.Options?.Select(o => new DTO.PollOptionResult
            {
                Id = o.Id,
                Description = o.Description,
                Result = o.Sessions?.Count ?? 0,
            }).ToList()
        };

    }
}
