using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL;
public  class Database
{
    public Database(IConfiguration configuration)
    {
        DbContextOptionsBuilder<AppDbContext> builder = new();
        builder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        AppDbContext dbContext = new(builder.Options);

        User = new DAO.User(dbContext);
        Poll = new DAO.Poll(dbContext);
        PollSession = new DAO.PollSession(dbContext);
    }

    public DAO.User User { get; private set; }

    public DAO.Poll Poll { get; private set; }

    public DAO.PollSession PollSession { get; private set; }
}
