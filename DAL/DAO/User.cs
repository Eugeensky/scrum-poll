using DAL.Helpers;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DAO
{
    public class User
    {
        private readonly AppDbContext db;
        internal User(AppDbContext _db)
        {
            db = _db;
        }
        public async Task<DTO.User?> GetVerifiedUser(string login, string password)
        {
            var hashedPassword = Hasher.HashString(password);
            var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == login && u.Password == hashedPassword);
            if (user is null) return null;
            return new DTO.User { Id = user.Id, Login = login, Role = user.Role.Name };
        }
    }
}
