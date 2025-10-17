using Happinest.Models;
using Happinest.Services.AuthAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace Happinest.Services.AuthAPI.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<EventGuest> EventGuests { get; set; }
        public virtual DbSet<Otp> Otps { get; set; }
        public virtual DbSet<GuestSession> GuestSessions { get; set; }
        public virtual DbSet<UserRoleMaster> UserRoleMasters { get; set; }
        public virtual DbSet<EmailTemplateMaster> EmailTemplateMasters { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("AppUser");

                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.AppleUserId)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.BackGroundPicture)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                // 👇 Use timestamp with time zone for all date/datetime columns
                entity.Property(e => e.Birthday).HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.ModifyDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.RegistrationDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.TermsAcceptedDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("timestamp with time zone");

                entity.Property(e => e.City).HasMaxLength(200);
                entity.Property(e => e.DeviceId).HasMaxLength(2000);
                entity.Property(e => e.DisplayName).HasMaxLength(500);
                entity.Property(e => e.DisplayPicture).HasMaxLength(500);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.FirstName).HasMaxLength(200);
                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.IdentificationMark).HasMaxLength(500);
                entity.Property(e => e.LastName).HasMaxLength(200);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Nationality).HasMaxLength(200);
                entity.Property(e => e.Password).HasMaxLength(500);
                entity.Property(e => e.Preferences).HasMaxLength(200);
                entity.Property(e => e.SignUpSource).HasMaxLength(500);
                entity.Property(e => e.State).HasMaxLength(200);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
            });

            modelBuilder.Entity<EventGuest>(entity =>
            {
                entity.HasKey(e => e.PersonalEventInviteId);
                entity.ToTable("EventGuest");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("UserRole");
            });

            modelBuilder.Entity<Otp>(entity =>
            {
                entity.HasKey(e => e.OtpId);
                entity.ToTable("Otp");
            });

            modelBuilder.Entity<GuestSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("GuestSession");
            });

            modelBuilder.Entity<UserRoleMaster>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("UserRoleMaster");
            });
            modelBuilder.Entity<EmailTemplateMaster>(entity =>
            {
                entity.HasKey(e => e.TemplateId).HasName("PK_EmailTemplateMaster");

                entity.ToTable("EmailTemplateMaster");

                entity.Property(e => e.CreatedDate)
                      .HasColumnType("timestamp without time zone");

                entity.Property(e => e.EmailFooter)
                      .HasMaxLength(200);

                entity.Property(e => e.EmailHeader)
                      .HasMaxLength(200);

                entity.Property(e => e.EmailSubject)
                      .HasMaxLength(200);

                entity.Property(e => e.EventTypeMasterId)
                      .HasColumnName("EventTypeMasterID");

                entity.Property(e => e.LastModifiedDate)
                      .HasColumnType("timestamp without time zone");

                entity.Property(e => e.TemplateCode)
                      .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);  

                entity.Property(e => e.Language)
                      .HasMaxLength(10)
                      .IsUnicode(false)
                      .HasDefaultValue("EN");
            });

        }
    }
}
