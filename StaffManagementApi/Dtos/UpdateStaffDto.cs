using System;

namespace StaffManagementApi.Dtos;

public class UpdateStaffDto
{
    public required int Id { get; set; }
    public required string FullName { get; set; }
    public required string NIM { get; set; }
    public required string BinusianId { get; set; }
    public required string Gender { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public int ActiveSemester { get; set; }
    public required string BinusianStatus { get; set; }
    public required string NIK { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string Address { get; set; }
    public required string NPWP { get; set; }
    public required string BankAccountNumber { get; set; }
    public required string BankBranch { get; set; }
    public required string AccountHolderName { get; set; }
    public required string ParentGuardianName { get; set; }
    public required string ParentGuardianPhone { get; set; }
    public required string EmergencyContact { get; set; }
    public required string EmergencyRelation { get; set; }
    public bool IsActive { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
