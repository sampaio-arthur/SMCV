using Microsoft.EntityFrameworkCore;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Example> Examples { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── Campaign ────────────────────────────────────────────────────
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            entity.Property(c => c.Niche).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Region).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.ResumeFileName).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.ResumeFilePath).HasColumnType("varchar(1000)").IsRequired();
            entity.Property(c => c.EmailSubject).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.EmailBody).HasColumnType("text").IsRequired();
            entity.Property(c => c.Status).HasColumnType("int").HasDefaultValue(CampaignStatus.Draft);
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasMany(c => c.Contacts)
                  .WithOne(ct => ct.Campaign)
                  .HasForeignKey(ct => ct.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Contact ─────────────────────────────────────────────────────
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            entity.Property(c => c.CompanyName).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Domain).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.ContactName).HasColumnType("varchar(255)");
            entity.Property(c => c.Position).HasColumnType("varchar(255)");
            entity.Property(c => c.Source).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.CampaignId).IsRequired();
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(c => c.Email);
            entity.HasIndex(c => c.CampaignId);
        });

        // ─── EmailLog ────────────────────────────────────────────────────
        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.ContactId).IsRequired();
            entity.HasIndex(e => e.ContactId).IsUnique();

            entity.Property(e => e.Status).HasColumnType("int").IsRequired();
            entity.Property(e => e.SentAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.ErrorMessage).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            entity.HasOne(el => el.Contact)
                  .WithOne(c => c.EmailLog)
                  .HasForeignKey<EmailLog>(el => el.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
