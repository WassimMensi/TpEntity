using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class ConsultationRepository : BaseRepository<Consultation>, IConsultationRepository
{
    public ConsultationRepository(HospitalDbContext context) : base(context) { }

    public async Task<bool> HasConflictAsync(int patientId, int doctorId, DateTime date) =>
        await _dbSet.AnyAsync(c =>
            c.PatientId == patientId &&
            c.DoctorId == doctorId &&
            c.Date == date &&
            c.Status != ConsultationStatus.Cancelled);

    public async Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId) =>
        await _dbSet.AsNoTracking()
                    .Where(c => c.PatientId == patientId
                             && c.Date > DateTime.Now
                             && c.Status != ConsultationStatus.Cancelled)
                    .OrderBy(c => c.Date)
                    .Include(c => c.Doctor)
                    .ToListAsync();

    public async Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await _dbSet.AsNoTracking()
                           .Where(c => c.DoctorId == doctorId
                                    && c.Date >= today
                                    && c.Date < tomorrow
                                    && c.Status != ConsultationStatus.Cancelled)
                           .OrderBy(c => c.Date)
                           .Include(c => c.Patient)
                           .ToListAsync();
    }
}