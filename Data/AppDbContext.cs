namespace API.Data;

using Microsoft.EntityFrameworkCore;
using API.Entities;
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    //Là property đại diện cho bảng Users (được ánh xạ từ AppUser class).

}