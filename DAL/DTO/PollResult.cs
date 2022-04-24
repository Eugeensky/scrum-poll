namespace DAL.DTO;
public class PollResult
{
    public string Description { get; set; } = "";
    public int VotesCount { get; set; }
    public List<PollOptionResult>? PollOptionResults { get; set; }
}
