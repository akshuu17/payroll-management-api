using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Models;

namespace PayrollManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🔹 TABLES
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SalaryGrade> SalaryGrades { get; set; }
        public DbSet<Payslip> Payslips { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔐 USER TABLE CONFIG
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.HasIndex(u => u.UserId);
                entity.HasIndex(u => u.EPFNo);
            });

            // 👨‍💼 EMPLOYEE TABLE CONFIG
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EPFNo);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Position)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Address)
                      .HasMaxLength(200);

                entity.Property(e => e.BasicSalary)
                      .HasDefaultValue(20000);
            });

            // 💰 SALARY GRADE CONFIG
            modelBuilder.Entity<SalaryGrade>(entity =>
            {
                entity.HasKey(s => s.Id);
            });

            // 🧾 PAYSLIP CONFIG
            modelBuilder.Entity<Payslip>(entity =>
            {
                entity.HasKey(p => p.Id);
            });
        
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventRegistration>()
                .HasIndex(r => new { r.EventId, r.EPFNo })
                .IsUnique();
        }
    }
}