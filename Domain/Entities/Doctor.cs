namespace HospitalManagement.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

}