namespace HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.ValueObjects;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Address? ContactAddress { get; set; }

    public int? HeadDoctorId { get; set; }
    public Doctor? HeadDoctor { get; set; }

    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}