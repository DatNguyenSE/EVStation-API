namespace API.Data;

using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole {
                Id = "2",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole {
                Id = "3",
                Name = "Staff",
                NormalizedName = "STAFF"
            }
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}