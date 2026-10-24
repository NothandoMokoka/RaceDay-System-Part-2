// Manages the application's database entities and database access.
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasKey(r => r.RoleID);

                entity.Property(r => r.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(r => r.RoleName)
                    .IsUnique();

                entity.Property(r => r.Description)
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(u => u.UserID);

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(u => u.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Event_Distance",
                        "[Distance] > 0");

                    table.HasCheckConstraint(
                        "CK_Event_EventType",
                        "[EventType] IN ('Running', 'Walking', 'Cycling')");
                });

                entity.HasKey(e => e.EventID);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Distance)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Organizer)
                    .WithMany()
                    .HasForeignKey(e => e.OrganizerID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Category_AgeRange",
                        "[MinimumAge] <= [MaximumAge]");

                    table.HasCheckConstraint(
                        "CK_Category_Distance",
                        "[Distance] IS NULL OR [Distance] > 0");
                });

                entity.HasKey(c => c.CategoryID);

                entity.Property(c => c.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Distance)
                    .HasColumnType("decimal(10,2)");

                entity.HasIndex(c => new { c.EventID, c.CategoryName })
                    .IsUnique();

                entity.HasOne(c => c.Event)
                    .WithMany()
                    .HasForeignKey(c => c.EventID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Enrollment_Status",
                        "[Status] IN ('CONFIRMED', 'CANCELLED')");
                });

                entity.HasKey(e => e.EnrollmentID);

                entity.Property(e => e.EnrollmentDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("CONFIRMED");

                entity.HasIndex(e => new { e.ParticipantID, e.EventID })
                    .IsUnique();

                entity.HasOne(e => e.Participant)
                    .WithMany()
                    .HasForeignKey(e => e.ParticipantID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("Result", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Result_FinishPosition",
                        "[FinishPosition] > 0");
                });

                entity.HasKey(r => r.ResultID);

                entity.Property(r => r.RecordedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(r => r.EnrollmentID)
                    .IsUnique();

                entity.HasOne(r => r.Enrollment)
                    .WithOne(e => e.Result)
                    .HasForeignKey<Result>(r => r.EnrollmentID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
