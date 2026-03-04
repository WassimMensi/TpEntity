using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IConsultationService
{
    Task<Consultation> ScheduleAsync(Consultation consultation);
    Task<Consultation> UpdateStatusAsync(int id, ConsultationStatus status);
    Task CancelAsync(int id);
    Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId);
    Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId);
}