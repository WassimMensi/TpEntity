using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Services;

public class ConsultationService : IConsultationService
{
    private readonly HospitalDbContext _context;

    public ConsultationService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation> ScheduleAsync(Consultation consultation)
    {
        // Vérifie qu'il n'y a pas déjà une consultation à ce créneau
        bool conflict = await _context.Consultations.AnyAsync(c =>
            c.PatientId == consultation.PatientId &&
            c.DoctorId == consultation.DoctorId &&
            c.Date == consultation.Date &&
            c.Status != ConsultationStatus.Cancelled);

        if (conflict)
            throw new InvalidOperationException(
                "Ce patient a déjà une consultation avec ce médecin à cette date.");

        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<Consultation> UpdateStatusAsync(int id, ConsultationStatus status)
    {
        var consultation = await _context.Consultations.FindAsync(id)
            ?? throw new KeyNotFoundException($"Consultation {id} introuvable.");

        consultation.Status = status;
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task CancelAsync(int id)
    {
        await UpdateStatusAsync(id, ConsultationStatus.Cancelled);
    }

    // Consultations futures d'un patient (pas annulées)
    public async Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId)
    {
        return await _context.Consultations
            .Where(c => c.PatientId == patientId
                     && c.Date > DateTime.Now
                     && c.Status != ConsultationStatus.Cancelled)
            .OrderBy(c => c.Date)
            .Include(c => c.Doctor)   // Eager loading du médecin
            .ToListAsync();
    }

    // Consultations du jour pour un médecin
    public async Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await _context.Consultations
            .Where(c => c.DoctorId == doctorId
                     && c.Date >= today
                     && c.Date < tomorrow
                     && c.Status != ConsultationStatus.Cancelled)
            .OrderBy(c => c.Date)
            .Include(c => c.Patient)  // Eager loading du patient
            .ToListAsync();
    }
}