namespace DAL.DTO;
public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string Role { get; set; } = null!;
}
