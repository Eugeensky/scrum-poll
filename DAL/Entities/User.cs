namespace DAL.Entities;
internal class User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public List<PollSession> Sessions { get; set; } = new();
}