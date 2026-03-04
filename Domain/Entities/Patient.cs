namespace HospitalManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

using HospitalManagement.Domain.ValueObjects;
public class Patient
{
    public int Id { get; set; }
    public string DossierNumber { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address Address { get; set; } = new();

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    // Token
    [Timestamp]
    public byte[] RowVersion { get; set; } = new byte[8];
}