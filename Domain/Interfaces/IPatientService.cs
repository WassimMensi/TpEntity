using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IPatientService
{
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient?> GetByIdAsync(int id);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    Task<IEnumerable<Patient>> GetAllAlphabeticalAsync(int page, int pageSize);
    Task<Patient> UpdateAsync(Patient patient);
    Task DeleteAsync(int id);
}