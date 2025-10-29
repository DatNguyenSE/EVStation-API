using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // 1 tài xế có thể có nhiều xe
    public ICollection<Vehicle>? Vehicles { get; set; }
    public ICollection<DriverPackage>? DriverPackages { get; set; }
    public ICollection<Receipt>? Receipts { get; set; }

    // === Thuộc tính chỉ dành cho STAFF ===
    // 1 Staff có thể có nhiều Assignment (Phân công)
    public ICollection<Assignment>? Assignments { get; set; }

    // 1 Staff (CreatedBy) có thể tạo nhiều Report
    [InverseProperty("CreatedByStaff")] // Chỉ định rõ FK
    public ICollection<Report>? ReportsCreated { get; set; }
    
    // 1 Staff (Technician) có thể sửa nhiều Report
    [InverseProperty("Technician")] // Chỉ định rõ FK
    public ICollection<Report>? ReportsAssignedToFix { get; set; }
}
