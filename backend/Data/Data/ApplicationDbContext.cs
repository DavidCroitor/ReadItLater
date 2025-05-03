using Microsoft.EntityFrameworkCore;
using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Data.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        List<IdentityRole<Guid>> roles = new List<IdentityRole<Guid>>
        {
            new IdentityRole<Guid> 
            { 
                Id = new Guid("8D04DCE2-969A-435D-BBB4-DF3F325983DC"), 
                Name = "Admin", 
                NormalizedName = "ADMIN" 
            },
            new IdentityRole<Guid> 
            { 
                Id = new Guid("1A73A03E-8466-4AFA-8577-305323C2EA7D"), 
                Name = "User", 
                NormalizedName = "USER" 
            }
        };

        modelBuilder.Entity<IdentityRole<Guid>>()
            .HasData(roles);

        modelBuilder.Entity<User>(entity => 
            {
                entity.HasKey(u => u.Id).HasName("PK_Users");
                entity.HasIndex(u => u.Email, "IX_Users_Email").IsUnique();
                entity.HasIndex(u => u.UserName, "IX_Users_Username").IsUnique();
                
                entity.HasMany(u => u.Articles)
                    .WithOne()
                    .HasForeignKey(ua => ua.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(u => u.Categories)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(rt => rt.User)
                    .HasForeignKey(rt => rt.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

            });

        modelBuilder.Entity<Article>( entity =>
            {
                entity.HasKey(am => am.Id).HasName("PK_Article");
                entity.HasIndex(am => new { am.UserId, am.URL }, "IX_Article_UserId_URL").IsUnique(); 
                entity.Property(am => am.URL).IsRequired().HasMaxLength(500);
                entity.HasMany(am => am.Categories)
                    .WithMany(c => c.Articles);

            });
        
        modelBuilder.Entity<Category>( entity => {
                entity.HasKey(c => c.Id).HasName("PK_Categories");
                entity.HasIndex(c => new {c.UserId, c.Name}, "IX_Categories_UserId_Name").IsUnique();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(c => c.Articles)
                    .WithMany(am => am.Categories);
                
        });
        
        modelBuilder.Entity<RefreshToken>( entity => {
            entity.HasKey(rt => rt.Id).HasName("PK_RefreshToken");
            entity.HasIndex(rt => rt.Token, "IX_RefreshToken_Token").IsUnique();
            entity.Property(rt => rt.Token).IsRequired();
            entity.HasIndex(rt => rt.UserId, "IX_RefreshToken_UserId");
        });

    }

}
