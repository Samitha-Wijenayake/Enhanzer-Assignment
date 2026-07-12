using Microsoft.EntityFrameworkCore;
using TestEnhanzer.Models.Entities;

namespace TestEnhanzer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserDetails> UserDetails => Set<UserDetails>();
    public DbSet<UserLocation> UserLocation => Set<UserLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLocation>(entity =>
        {
            entity.ToTable("Location_Details");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LocationCode).HasColumnName("Location_Code").HasMaxLength(100);
            entity.Property(e => e.LocationName).HasColumnName("Location_Name").HasMaxLength(200);

            entity.HasOne(e => e.UserDetails)
                .WithMany(u => u.UserLocations)
                .HasForeignKey(e => e.UserDetailsId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserDetailsId, e.LocationCode }).IsUnique();
        });

        modelBuilder.Entity<UserDetails>(entity =>
        {
            entity.ToTable("UserDetails");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.Username }).IsUnique();
        });
    }
}
