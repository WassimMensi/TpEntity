namespace HospitalManagement.Domain.DTOs;

public class PatientRecordDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string DossierNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ConsultationSummaryDto> Consultations { get; set; } = new();
}

public class ConsultationSummaryDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DoctorFullName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
}