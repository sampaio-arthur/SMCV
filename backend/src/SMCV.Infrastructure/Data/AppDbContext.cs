using Microsoft.EntityFrameworkCore;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureUserProfile(modelBuilder);
        ConfigureCampaign(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureEmailLog(modelBuilder);
    }

    // ─── User ─────────────────────────────────────────────────────────
    // Raiz do sistema. Autenticação local (PasswordHash).
    // Possui um perfil (1:1) e várias campanhas (1:N).
    // Id gerado no construtor via Guid.NewGuid() (ValueGeneratedNever).
    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // Chave primária
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();

            // Propriedades
            entity.Property(u => u.Name).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnType("varchar(500)").IsRequired();
            entity.Property(u => u.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            // Índices
            entity.HasIndex(u => u.Email).IsUnique();

            // Relacionamentos
            // User 1:1 UserProfile — cascade: remover user remove o perfil
            entity.HasOne(u => u.UserProfile)
                  .WithOne(up => up.User)
                  .HasForeignKey<UserProfile>(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // User 1:N Campaigns — cascade: remover user remove suas campanhas
            entity.HasMany(u => u.Campaigns)
                  .WithOne(c => c.User)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ─── UserProfile ──────────────────────────────────────────────────
    // Perfil complementar do usuário (currículo, dados extras).
    // Relação 1:1 com User; FK UserId é única.
    private static void ConfigureUserProfile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            // Chave primária
            entity.HasKey(up => up.Id);
            entity.Property(up => up.Id).ValueGeneratedNever();

            // Propriedades
            entity.Property(up => up.UserId).IsRequired();
            entity.Property(up => up.ResumeFilePath).HasColumnType("varchar(1000)");
            entity.Property(up => up.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            // Índices — garante 1:1 no banco
            entity.HasIndex(up => up.UserId).IsUnique();
        });
    }

    // ─── Campaign ─────────────────────────────────────────────────────
    // Campanha de e-mail pertencente a um User (N:1).
    // Possui vários Contacts (1:N). Status armazenado como string.
    private static void ConfigureCampaign(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>(entity =>
        {
            // Chave primária
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            // Propriedades
            entity.Property(c => c.UserId).IsRequired();
            entity.Property(c => c.Name).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Niche).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.Region).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.EmailSubject).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.EmailBody).HasColumnType("text").IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasDefaultValue(CampaignStatus.Draft);
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            // Relacionamentos
            // Campaign 1:N Contacts — cascade: remover campanha remove seus contatos
            entity.HasMany(c => c.Contacts)
                  .WithOne(ct => ct.Campaign)
                  .HasForeignKey(ct => ct.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ─── Contact ──────────────────────────────────────────────────────
    // Contato vinculado a uma Campaign (N:1).
    // Possui um EmailLog (1:1). EmailStatus armazenado como string.
    private static void ConfigureContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            // Chave primária
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            // Propriedades
            entity.Property(c => c.CampaignId).IsRequired();
            entity.Property(c => c.CompanyName).HasColumnType("varchar(500)").IsRequired();
            entity.Property(c => c.Email).HasColumnType("varchar(255)").IsRequired();
            entity.Property(c => c.EmailStatus).HasConversion<string>().HasDefaultValue(EmailStatus.Pending);
            entity.Property(c => c.EmailSentAt).HasColumnType("timestamp with time zone");
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            // Índices
            entity.HasIndex(c => c.CampaignId);
            entity.HasIndex(c => c.Email);

            // Relacionamentos
            // Contact 1:1 EmailLog — cascade: remover contato remove seu log
            entity.HasOne(c => c.EmailLog)
                  .WithOne(el => el.Contact)
                  .HasForeignKey<EmailLog>(el => el.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ─── EmailLog ─────────────────────────────────────────────────────
    // Registro de envio/erro de e-mail. Relação 1:1 com Contact.
    // FK ContactId é única — cada contato tem no máximo um log.
    private static void ConfigureEmailLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailLog>(entity =>
        {
            // Chave primária
            entity.HasKey(el => el.Id);
            entity.Property(el => el.Id).ValueGeneratedNever();

            // Propriedades
            entity.Property(el => el.ContactId).IsRequired();
            entity.Property(el => el.ErrorMessage).HasColumnType("text");
            entity.Property(el => el.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();

            // Índices — garante 1:1 no banco
            entity.HasIndex(el => el.ContactId).IsUnique();
        });
    }
}
