using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<MedicalDoctor> MedicalDoctors => Set<MedicalDoctor>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<AdminStaff> AdminStaff => Set<AdminStaff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(p => p.DossierNumber).IsUnique().HasDatabaseName("IX_Patients_DossierNumber");

            entity.HasIndex(p => p.Email).IsUnique();

            entity.HasIndex(p => new { p.LastName, p.FirstName })
                  .HasDatabaseName("IX_Patients_FullName");

            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Email).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
            entity.Property(p => p.DossierNumber).IsRequired().HasMaxLength(8);
            entity.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();

            entity.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(d => d.Name).IsRequired().HasMaxLength(150);
            entity.Property(d => d.Location).HasMaxLength(200);

            entity.HasOne(dep => dep.HeadDoctor)
                  .WithMany()
                  .HasForeignKey(dep => dep.HeadDoctorId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);

            entity.HasOne(dep => dep.ParentDepartment)
                  .WithMany(dep => dep.SubDepartments)
                  .HasForeignKey(dep => dep.ParentDepartmentId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            entity.OwnsOne(d => d.ContactAddress, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });
        });

        // --- Doctor ---
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(d => d.LicenseNumber).IsUnique();

            entity.HasIndex(d => d.DepartmentId)
                  .HasDatabaseName("IX_Doctors_DepartmentId");

            entity.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Specialty).IsRequired().HasMaxLength(100);
            entity.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany(dep => dep.Doctors).HasForeignKey(d => d.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.Property(c => c.Date).IsRequired();
            entity.Property(c => c.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);
            entity.Property(c => c.Notes).HasMaxLength(1000);

            entity.HasOne(c => c.Patient)
                  .WithMany(p => p.Consultations)
                  .HasForeignKey(c => c.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Doctor)
                  .WithMany(d => d.Consultations)
                  .HasForeignKey(c => c.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => new { c.PatientId, c.DoctorId, c.Date })
                  .IsUnique();

            entity.HasIndex(c => new { c.DoctorId, c.Date })
                  .HasDatabaseName("IX_Consultations_DoctorId_Date");

            entity.HasIndex(c => new { c.PatientId, c.Date })
                  .HasDatabaseName("IX_Consultations_PatientId_Date");
        });

        // --- Héritage TPH : Staff ---
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasDiscriminator<string>("StaffType")
                  .HasValue<MedicalDoctor>("Doctor")
                  .HasValue<Nurse>("Nurse")
                  .HasValue<AdminStaff>("Admin");

            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Salary).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<MedicalDoctor>(entity =>
        {
            entity.HasIndex(d => d.LicenseNumber).IsUnique();
            entity.Property(d => d.Specialty).HasMaxLength(100);
            entity.Property(d => d.LicenseNumber).HasMaxLength(50);

            entity.HasOne(d => d.Department)
                  .WithMany()
                  .HasForeignKey(d => d.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}