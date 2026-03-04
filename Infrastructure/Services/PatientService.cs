using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        // Validation date de naissance
        if (patient.DateOfBirth >= DateTime.Today)
            throw new ArgumentException("La date de naissance doit être dans le passé.");

        // Vérification email unique
        if (await _context.Patients.AnyAsync(p => p.Email == patient.Email))
            throw new InvalidOperationException("Cet email est déjà utilisé.");

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<Patient?> GetByDossierNumberAsync(string dossierNumber)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.DossierNumber == dossierNumber);
    }

    // Recherche par nom OU prénom, insensible à la casse
    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.LastName.ToLower().Contains(name.ToLower())
                     || p.FirstName.ToLower().Contains(name.ToLower()))
            .OrderBy(p => p.LastName)
            .ToListAsync();
    }

    // Liste alphabétique avec pagination
    public async Task<IEnumerable<Patient>> GetAllAlphabeticalAsync(int page, int pageSize)
    {
        return await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)   // On saute les pages précédentes
            .Take(pageSize)                 // On prend uniquement la page demandée
            .ToListAsync();
    }

    public async Task<Patient> UpdateAsync(Patient patient)
    {
        if (patient.DateOfBirth >= DateTime.Today)
            throw new ArgumentException("La date de naissance doit être dans le passé.");

        _context.Patients.Update(patient);

        try
        {
            await _context.SaveChangesAsync();
            return patient;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Récupère l'entrée en conflit
            var entry = ex.Entries.Single();

            // Valeurs actuelles en base (ce qu'un autre utilisateur a sauvegardé)
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues is null)
                throw new InvalidOperationException(
                    "Le patient a été supprimé par un autre utilisateur.");

            var dbPatient = (Patient)databaseValues.ToObject();

            throw new InvalidOperationException(
                $"Conflit de mise à jour : le patient a été modifié par un autre utilisateur. " +
                $"Dernière modification : {dbPatient.FirstName} {dbPatient.LastName}.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient is null)
            throw new KeyNotFoundException($"Patient {id} introuvable.");

        // Vérifie s'il a des consultations avant de supprimer
        bool hasConsultations = await _context.Consultations
            .AnyAsync(c => c.PatientId == id);

        if (hasConsultations)
            throw new InvalidOperationException(
                "Impossible de supprimer un patient ayant des consultations.");

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }

    // Compter les patients par département
    public async Task<Dictionary<string, int>> CountPatientsByDepartmentAsync()
    {
        return await _context.Consultations
            .AsNoTracking()
            .GroupBy(c => c.Doctor.Department.Name)
            .Select(g => new
            {
                Department = g.Key,
                Count = g.Select(c => c.PatientId).Distinct().Count()
            })
            .ToDictionaryAsync(x => x.Department, x => x.Count);
    }
}