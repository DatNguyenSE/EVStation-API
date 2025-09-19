using System;

namespace API.DTOs;

public class SeedUserDto
{
    public required String Id { get; set; }
    public required String Email { get; set; }
    public required String UserName { get; set; }
}
