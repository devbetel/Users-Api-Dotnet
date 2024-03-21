using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace users_api_dotnet.context
{
    
    public class AppDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Criando usuario admin
            IdentityUser<int> admin = new IdentityUser<int>
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                Id = 1
            };
            
            PasswordHasher<IdentityUser<int>> hasher1 = new PasswordHasher<IdentityUser<int>>();
            admin.PasswordHash = hasher1.HashPassword(admin, "Admin123!");

            builder.Entity<IdentityUser<int>>().HasData(admin);


            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "admin", NormalizedName = "ADMIN" }
                );


            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 2, Name = "regular", NormalizedName = "REGULAR" }
                );

            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { RoleId = 1, UserId = 1 }
                );
          
        }
    }       
}