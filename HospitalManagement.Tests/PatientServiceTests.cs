using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Repositories;
using HospitalManagement.Infrastructure.Services;
using HospitalManagement.Tests.Helpers;

namespace HospitalManagement.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidPatient_ReturnsCreatedPatient()
    {
        var context = DbContextFactory.CreateInMemory("CreatePatient_Valid");
        var repository = new PatientRepository(context);
        var service = new PatientService(context);

        var patient = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean.dupont@email.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            Phone = "0612345678"
        };

        var result = await service.CreateAsync(patient);

        Assert.NotNull(result);
        Assert.Equal("Jean", result.FirstName);
        Assert.NotEmpty(result.DossierNumber);
    }

    [Fact]
    public async Task CreateAsync_FutureDateOfBirth_ThrowsArgumentException()
    {
        var context = DbContextFactory.CreateInMemory("CreatePatient_FutureDate");
        var service = new PatientService(context);

        var patient = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean@email.com",
            DateOfBirth = DateTime.Today.AddDays(1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(patient));
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var context = DbContextFactory.CreateInMemory("CreatePatient_DuplicateEmail");
        var service = new PatientService(context);

        var patient1 = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "same@email.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var patient2 = new Patient
        {
            FirstName = "Marie",
            LastName = "Martin",
            Email = "same@email.com",
            DateOfBirth = new DateTime(1985, 5, 15)
        };

        await service.CreateAsync(patient1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(patient2));
    }

    [Fact]
    public async Task DeleteAsync_PatientWithConsultations_ThrowsInvalidOperationException()
    {
        var context = DbContextFactory.CreateInMemory("DeletePatient_WithConsultations");
        var service = new PatientService(context);

        var patient = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean@email.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        await service.CreateAsync(patient);

        // On ajoute une consultation liée au patient
        var consultation = new Consultation
        {
            PatientId = patient.Id,
            DoctorId = 1,
            Date = DateTime.Now.AddDays(1),
            Status = ConsultationStatus.Planned
        };
        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(patient.Id));
    }
}