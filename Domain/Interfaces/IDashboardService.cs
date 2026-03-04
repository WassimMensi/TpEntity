using HospitalManagement.Domain.DTOs;

namespace HospitalManagement.Domain.Interfaces;

public interface IDashboardService
{
    Task<PatientRecordDto?> GetPatientRecordAsync(int patientId);
    Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId);
    Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync();
}