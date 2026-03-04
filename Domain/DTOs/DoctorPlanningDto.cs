namespace HospitalManagement.Domain.DTOs;

public class DoctorPlanningDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<UpcomingConsultationDto> UpcomingConsultations { get; set; } = new();
}

public class UpcomingConsultationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}