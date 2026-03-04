using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Services;
using HospitalManagement.Tests.Helpers;

namespace HospitalManagement.Tests;

public class ConsultationServiceTests
{
    [Fact]
    public async Task ScheduleAsync_ValidConsultation_ReturnsCreatedConsultation()
    {
        var context = DbContextFactory.CreateInMemory("Schedule_Valid");
        var service = new ConsultationService(context);

        var consultation = new Consultation
        {
            PatientId = 1,
            DoctorId = 1,
            Date = DateTime.Now.AddDays(1),
            Status = ConsultationStatus.Planned
        };

        var result = await service.ScheduleAsync(consultation);

        Assert.NotNull(result);
        Assert.Equal(ConsultationStatus.Planned, result.Status);
    }

    [Fact]
    public async Task ScheduleAsync_ConflictingConsultation_ThrowsInvalidOperationException()
    {
        var context = DbContextFactory.CreateInMemory("Schedule_Conflict");
        var service = new ConsultationService(context);
        var date = DateTime.Now.AddDays(1);

        var consultation1 = new Consultation
        {
            PatientId = 1,
            DoctorId = 1,
            Date = date,
            Status = ConsultationStatus.Planned
        };

        var consultation2 = new Consultation
        {
            PatientId = 1,
            DoctorId = 1,
            Date = date,
            Status = ConsultationStatus.Planned
        };

        await service.ScheduleAsync(consultation1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ScheduleAsync(consultation2));
    }

    [Fact]
    public async Task CancelAsync_ExistingConsultation_StatusIsCancelled()
    {
        var context = DbContextFactory.CreateInMemory("Cancel_Valid");
        var service = new ConsultationService(context);

        var consultation = new Consultation
        {
            PatientId = 1,
            DoctorId = 1,
            Date = DateTime.Now.AddDays(1),
            Status = ConsultationStatus.Planned
        };
        await service.ScheduleAsync(consultation);

        await service.CancelAsync(consultation.Id);

        var updated = await context.Consultations.FindAsync(consultation.Id);
        Assert.Equal(ConsultationStatus.Cancelled, updated!.Status);
    }
}