using Microsoft.EntityFrameworkCore;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── User ─────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();
            entity.Property(u => u.Name).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnType("varchar(500)").IsRequired();
            entity.Property(u => u.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.UserProfile)
                  .WithOne(up => up.User)
                  .HasForeignKey<UserProfile>(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Campaigns)
                  .WithOne(c => c.User)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── UserProfile ───────────────────────────────────────────────────
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(up => up.Id);
            entity.Property(up => up.Id).ValueGeneratedNever();
            entity.Property(up => up.UserId).IsRequired();
            entity.Property(up => up.ResumeFilePath).HasColumnType("varchar(1000)");
            entity.Property(up => up.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(up => up.UserId).IsUnique();
        });

        // ─── Campaign ──────────────────────────────────────────────────────
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.UserId).IsRequired();
            entity.Property(c => c.Name).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Niche).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Region).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.EmailSubject).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.EmailBody).HasColumnType("text").IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasDefaultValue(CampaignStatus.Draft);
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasMany(c => c.Contacts)
                  .WithOne(ct => ct.Campaign)
                  .HasForeignKey(ct => ct.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Contact ───────────────────────────────────────────────────────
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.CampaignId).IsRequired();
            entity.Property(c => c.CompanyName).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.EmailStatus).HasConversion<string>().HasDefaultValue(EmailStatus.Pending);
            entity.Property(c => c.EmailSentAt).HasColumnType("timestamp with time zone");
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(c => c.CampaignId);
            entity.HasIndex(c => c.Email);

            entity.HasOne(c => c.EmailLog)
                  .WithOne(el => el.Contact)
                  .HasForeignKey<EmailLog>(el => el.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── EmailLog ──────────────────────────────────────────────────────
        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(el => el.Id);
            entity.Property(el => el.Id).ValueGeneratedNever();
            entity.Property(el => el.ContactId).IsRequired();
            entity.Property(el => el.ErrorMessage).HasColumnType("text");
            entity.Property(el => el.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(el => el.ContactId).IsUnique();
        });
    }
}
