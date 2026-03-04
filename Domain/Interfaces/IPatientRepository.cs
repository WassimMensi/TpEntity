using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByDossierNumberAsync(string dossierNumber);
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    Task<IEnumerable<Patient>> GetAllAlphabeticalAsync(int page, int pageSize);
    Task<bool> HasConsultationsAsync(int patientId);
    Task<Dictionary<string, int>> CountPatientsByDepartmentAsync();
}