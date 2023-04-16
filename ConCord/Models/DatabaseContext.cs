using Microsoft.EntityFrameworkCore;
namespace ConCord.Models;

public class DatabaseContext : DbContext 
{
  public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Channel>()
      .Property(e => e.Created)
      .HasColumnType("TIMESTAMPTZ")
      .HasDefaultValueSql("CURRENT_TIMESTAMP");

    modelBuilder.Entity<Channel>()
      .Property(e => e.Name)
      .HasColumnType("VARCHAR(50)");

    modelBuilder.Entity<Message>()
      .Property(e => e.Created)
      .HasColumnType("TIMESTAMPTZ")
      .HasDefaultValueSql("CURRENT_TIMESTAMP");

    modelBuilder.Entity<Message>()
      .Property(e => e.Text)
      .HasColumnType("VARCHAR(500)");

    modelBuilder.Entity<Message>()
      .Property(e => e.UserName)
      .HasColumnType("VARCHAR(10)");
  }

  public DbSet<Channel> Channels => Set<Channel>();
  public DbSet<Message> Messages => Set<Message>();
}