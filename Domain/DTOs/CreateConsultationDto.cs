namespace HospitalManagement.Domain.DTOs;

public class CreateConsultationDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
}
