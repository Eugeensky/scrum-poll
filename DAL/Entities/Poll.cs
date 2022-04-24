namespace DAL.Entities;

internal class Poll
{
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public int TimeInMinutes { get; set; }
    public bool Published { get; set; }
    public List<PollOption> Options { get; set; } = new();
    public List<PollSession> Sessions { get; set; } = new();
}