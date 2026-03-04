namespace HospitalManagement.Domain.Entities;

public enum ConsultationStatus
{
    Planned,
    Completed,
    Cancelled
}

public class Consultation
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public ConsultationStatus Status { get; set; } = ConsultationStatus.Planned;

    public string? Notes { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}