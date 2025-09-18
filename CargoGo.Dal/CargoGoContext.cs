using CargoGo.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoGo.Dal;

public class CargoGoContext : DbContext
{
    public CargoGoContext(DbContextOptions<CargoGoContext> options) : base(options)
    {
    }

    public DbSet<TravelerEntity> Travelers => Set<TravelerEntity>();
    public DbSet<SenderEntity> Senders => Set<SenderEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TravelerEntity>(e =>
        {
            e.ToTable("Travelers");
            e.Property(p => p.From).IsRequired().HasMaxLength(100);
            e.Property(p => p.To).IsRequired().HasMaxLength(100);
            e.Property(p => p.Weight).IsRequired();
            e.Property(p => p.Reward).IsRequired();
            e.Property(p => p.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<SenderEntity>(e =>
        {
            e.ToTable("Senders");
            e.Property(p => p.From).IsRequired().HasMaxLength(100);
            e.Property(p => p.To).IsRequired().HasMaxLength(100);
            e.Property(p => p.Weight).IsRequired();
            e.Property(p => p.Description).IsRequired().HasMaxLength(500);
            e.Property(p => p.CreatedAt).IsRequired();
        });
    }
}
