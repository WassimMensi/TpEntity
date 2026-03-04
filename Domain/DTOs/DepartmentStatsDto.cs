namespace HospitalManagement.Domain.DTOs;

public class DepartmentStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int ConsultationCount { get; set; }
}