using Hipp.Domain.Entities.Identity;
using Hipp.Domain.Entities.Audit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hipp.Infrastructure.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Menaxher> Menaxhers { get; set; } = null!;
    public virtual DbSet<Komercialist> Komercialists { get; set; } = null!;
    public virtual DbSet<Shofer> Shofers { get; set; } = null!;
    public virtual DbSet<Etiketues> Etiketueses { get; set; } = null!;
    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Identity tables
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.PendingEmail).HasMaxLength(256);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(1000);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(1000);
            entity.Property(e => e.DeletedBy).HasMaxLength(450);
        });

        // Configure UserActivityLog
        builder.Entity<UserActivityLog>(entity =>
        {
            entity.ToTable("UserActivityLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.AdditionalInfo).HasMaxLength(1000);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure role-specific entities
        builder.Entity<Menaxher>(entity =>
        {
            entity.ToTable("Menaxhers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Menaxher>(e => e.UserId);
        });

        builder.Entity<Komercialist>(entity =>
        {
            entity.ToTable("Komercialists");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Komercialist>(e => e.UserId);
        });

        builder.Entity<Shofer>(entity =>
        {
            entity.ToTable("Shofers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Shofer>(e => e.UserId);
        });

        builder.Entity<Etiketues>(entity =>
        {
            entity.ToTable("Etiketueses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Etiketues>(e => e.UserId);
        });
    }
} 