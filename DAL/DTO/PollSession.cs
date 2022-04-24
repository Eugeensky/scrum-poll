namespace DAL.DTO;
public class PollSession
{
    public Guid Id { get; set; }
    public long StartTime { get; set; }
    public int TimeInMinutes { get; set; }
    public string PollDescription { get; set; } = "";
    public List<PollOptionDescription>? Options { get; set; }
}
