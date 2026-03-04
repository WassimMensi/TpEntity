namespace HospitalManagement.Domain.Entities;

// Classe de base abstraite
public abstract class Staff
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
}


public class MedicalDoctor : Staff
{
    public string Specialty { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}

public class Nurse : Staff
{
    public string Service { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}

public class AdminStaff : Staff
{
    public string Function { get; set; } = string.Empty;
}