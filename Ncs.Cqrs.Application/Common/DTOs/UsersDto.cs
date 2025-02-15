using System;

namespace Ncs.Cqrs.Application.Common.DTOs;

public class UsersDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Firstname { get; set; }
    public string Middlename { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string EmployeeNumber { get; set; }
    public int? CompanyId { get; set; }
    public int? PersonalTypeId { get; set; }
    public string PersonalIdNumber { get; set; }
    public string GuestCompanyName { get; set; }
    public string RfidTag { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string Fullname { get; set; }
    public virtual CompaniesDto Company { get; set; }
    public virtual PersonalIdTypeDto PersonalIdType { get; set; }
    public virtual UsersDto CreatedByUser { get; set; }
    public virtual UsersDto UpdatedByUser { get; set; }
    // Many-to-Many Relationship: User has multiple roles
    public List<RolesDto> Roles { get; set; } = new List<RolesDto>();
}
