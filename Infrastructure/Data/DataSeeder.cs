using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.ValueObjects;
using HospitalManagement.Infrastructure.Data;

namespace HospitalManagement.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(HospitalDbContext context)
    {
        // On ne seed que si la base est vide
        if (context.Patients.Any()) return;

        // --- Départements ---
        var cardio = new Department { Name = "Cardiologie", Location = "Bâtiment A - 2ème étage" };
        var neuro = new Department { Name = "Neurologie", Location = "Bâtiment B - 1er étage" };
        var pediatrie = new Department
        {
            Name = "Pédiatrie",
            Location = "Bâtiment C - RDC",
            SubDepartments = new List<Department>
            {
                new() { Name = "Pédiatrie générale", Location = "Bâtiment C - RDC" },
                new() { Name = "Néonatologie", Location = "Bâtiment C - 1er étage" }
            }
        };

        context.Departments.AddRange(cardio, neuro, pediatrie);
        await context.SaveChangesAsync();

        // --- Médecins ---
        var doctorMartin = new Doctor
        {
            FirstName = "Sophie",
            LastName = "Martin",
            Specialty = "Cardiologue",
            LicenseNumber = "CM-10001",
            DepartmentId = cardio.Id
        };

        var doctorLeroy = new Doctor
        {
            FirstName = "Pierre",
            LastName = "Leroy",
            Specialty = "Neurologue",
            LicenseNumber = "CM-10002",
            DepartmentId = neuro.Id
        };

        var doctorBernard = new Doctor
        {
            FirstName = "Claire",
            LastName = "Bernard",
            Specialty = "Pédiatre",
            LicenseNumber = "CM-10003",
            DepartmentId = pediatrie.Id
        };

        context.Doctors.AddRange(doctorMartin, doctorLeroy, doctorBernard);
        await context.SaveChangesAsync();

        // --- Responsables de département ---
        cardio.HeadDoctorId = doctorMartin.Id;
        neuro.HeadDoctorId = doctorLeroy.Id;
        pediatrie.HeadDoctorId = doctorBernard.Id;
        await context.SaveChangesAsync();

        // --- Patients ---
        var patients = new List<Patient>
        {
            new()
            {
                FirstName = "Jean",
                LastName = "Dupont",
                DateOfBirth = new DateTime(1980, 3, 15),
                Email = "jean.dupont@email.com",
                Phone = "0612345678",
                RowVersion = new byte[8],
                Address = new Address
                {
                    Street = "12 rue de la Paix",
                    City = "Lyon",
                    PostalCode = "69001",
                    Country = "France"
                }
            },
            new()
            {
                FirstName = "Marie",
                LastName = "Lambert",
                DateOfBirth = new DateTime(1975, 7, 22),
                Email = "marie.lambert@email.com",
                Phone = "0698765432",
                RowVersion = new byte[8],
                Address = new Address
                {
                    Street = "5 avenue des Fleurs",
                    City = "Paris",
                    PostalCode = "75008",
                    Country = "France"
                }
            },
            new()
            {
                FirstName = "Thomas",
                LastName = "Moreau",
                DateOfBirth = new DateTime(1992, 11, 8),
                Email = "thomas.moreau@email.com",
                Phone = "0756789012",
                RowVersion = new byte[8],
                Address = new Address
                {
                    Street = "8 boulevard Victor Hugo",
                    City = "Marseille",
                    PostalCode = "13001",
                    Country = "France"
                }
            },
            new()
            {
                FirstName = "Lucie",
                LastName = "Petit",
                DateOfBirth = new DateTime(2000, 1, 30),
                Email = "lucie.petit@email.com",
                Phone = "0634567890",
                RowVersion = new byte[8],
                Address = new Address
                {
                    Street = "3 rue du Moulin",
                    City = "Bordeaux",
                    PostalCode = "33000",
                    Country = "France"
                }
            },
            new()
            {
                FirstName = "Paul",
                LastName = "Girard",
                DateOfBirth = new DateTime(1965, 9, 5),
                Email = "paul.girard@email.com",
                Phone = "0623456789",
                RowVersion = new byte[8],
                Address = new Address
                {
                    Street = "27 rue Nationale",
                    City = "Lille",
                    PostalCode = "59000",
                    Country = "France"
                }
            }
        };

        context.Patients.AddRange(patients);
        await context.SaveChangesAsync();

        // --- Consultations ---
        var consultations = new List<Consultation>
        {
            // Consultations passées (réalisées)
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctorMartin.Id,
                Date = DateTime.Now.AddDays(-10),
                Status = ConsultationStatus.Completed,
                Notes = "Bilan cardiaque annuel, résultats satisfaisants."
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctorLeroy.Id,
                Date = DateTime.Now.AddDays(-5),
                Status = ConsultationStatus.Completed,
                Notes = "Suivi post-opératoire, évolution positive."
            },
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctorMartin.Id,
                Date = DateTime.Now.AddDays(-3),
                Status = ConsultationStatus.Completed,
                Notes = "Première consultation, ECG normal."
            },

            // Consultations du jour (planifiées)
            new()
            {
                PatientId = patients[3].Id,
                DoctorId = doctorBernard.Id,
                Date = DateTime.Today.AddHours(9),
                Status = ConsultationStatus.Planned,
                Notes = "Consultation pédiatrique de routine."
            },
            new()
            {
                PatientId = patients[4].Id,
                DoctorId = doctorMartin.Id,
                Date = DateTime.Today.AddHours(11),
                Status = ConsultationStatus.Planned,
                Notes = "Contrôle tension artérielle."
            },

            // Consultations futures
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctorLeroy.Id,
                Date = DateTime.Now.AddDays(7),
                Status = ConsultationStatus.Planned,
                Notes = "Consultation neurologique de suivi."
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctorBernard.Id,
                Date = DateTime.Now.AddDays(14),
                Status = ConsultationStatus.Planned,
                Notes = null
            },

            // Consultation annulée
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctorLeroy.Id,
                Date = DateTime.Now.AddDays(3),
                Status = ConsultationStatus.Cancelled,
                Notes = "Annulée par le patient."
            }
        };

        context.Consultations.AddRange(consultations);
        await context.SaveChangesAsync();
    }
}