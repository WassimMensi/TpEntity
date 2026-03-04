using HospitalManagement.Domain.DTOs;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly HospitalDbContext _context;

    public DashboardService(HospitalDbContext context)
    {
        _context = context;
    }

    // Vue 1 : Fiche patient avec toutes ses consultations
    public async Task<PatientRecordDto?> GetPatientRecordAsync(int patientId)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == patientId)
            .Select(p => new PatientRecordDto
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName,
                DossierNumber = p.DossierNumber,
                Email = p.Email,
                Consultations = p.Consultations
                    .OrderByDescending(c => c.Date)
                    .Select(c => new ConsultationSummaryDto
                    {
                        Id = c.Id,
                        Date = c.Date,
                        Status = c.Status.ToString(),
                        DoctorFullName = c.Doctor.FirstName + " " + c.Doctor.LastName,
                        DoctorSpecialty = c.Doctor.Specialty
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // Vue 2 : Planning médecin avec département + consultations à venir
    public async Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == doctorId)
            .Select(d => new DoctorPlanningDto
            {
                Id = d.Id,
                FullName = d.FirstName + " " + d.LastName,
                Specialty = d.Specialty,
                DepartmentName = d.Department.Name,
                UpcomingConsultations = d.Consultations
                    .Where(c => c.Date > DateTime.Now
                             && c.Status != Domain.Entities.ConsultationStatus.Cancelled)
                    .OrderBy(c => c.Date)
                    .Select(c => new UpcomingConsultationDto
                    {
                        Id = c.Id,
                        Date = c.Date,
                        PatientFullName = c.Patient.FirstName + " " + c.Patient.LastName,
                        Status = c.Status.ToString()
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // Vue 3 : Statistiques par département
    public async Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(dep => new DepartmentStatsDto
            {
                Id = dep.Id,
                Name = dep.Name,
                Location = dep.Location,
                DoctorCount = dep.Doctors.Count,
                ConsultationCount = dep.Doctors
                    .SelectMany(d => d.Consultations)
                    .Count()
            })
            .OrderBy(dep => dep.Name)
            .ToListAsync();
    }
}