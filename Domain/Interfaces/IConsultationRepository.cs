using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IConsultationRepository : IRepository<Consultation>
{
    Task<bool> HasConflictAsync(int patientId, int doctorId, DateTime date);
    Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId);
    Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId);
}