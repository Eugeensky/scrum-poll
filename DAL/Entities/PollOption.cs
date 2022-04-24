namespace DAL.Entities;

internal class PollOption
{
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public Guid PollId { get; set; }
    public Poll Poll { get; set; } = null!;
    public List<PollSession> Sessions { get; set; } = new();
}