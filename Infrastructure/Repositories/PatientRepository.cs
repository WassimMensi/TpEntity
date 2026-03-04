using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(HospitalDbContext context) : base(context) { }

    public async Task<Patient?> GetByDossierNumberAsync(string dossierNumber) =>
        await _dbSet.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.DossierNumber == dossierNumber);

    public async Task<Patient?> GetByEmailAsync(string email) =>
        await _dbSet.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == email);

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name) =>
        await _dbSet.AsNoTracking()
                    .Where(p => p.LastName.ToLower().Contains(name.ToLower())
                             || p.FirstName.ToLower().Contains(name.ToLower()))
                    .OrderBy(p => p.LastName)
                    .ToListAsync();

    public async Task<IEnumerable<Patient>> GetAllAlphabeticalAsync(int page, int pageSize) =>
        await _dbSet.AsNoTracking()
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

    public async Task<bool> HasConsultationsAsync(int patientId) =>
        await _context.Consultations.AnyAsync(c => c.PatientId == patientId);

    public async Task<Dictionary<string, int>> CountPatientsByDepartmentAsync() =>
        await _context.Consultations
                      .AsNoTracking()
                      .GroupBy(c => c.Doctor.Department.Name)
                      .Select(g => new
                      {
                          Department = g.Key,
                          Count = g.Select(c => c.PatientId).Distinct().Count()
                      })
                      .ToDictionaryAsync(x => x.Department, x => x.Count);
}