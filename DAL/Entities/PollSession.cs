namespace DAL.Entities;

internal class PollSession
{
    public Guid Id { get; set; }
    public long StartTime { get; set; }
    public Guid PollId { get; set; }
    public Poll Poll { get; set; } = null!;
    public Guid? PollOptionId { get; set; }
    public PollOption? PollOption { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool Finished { get; set; }
}