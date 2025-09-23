namespace API.DTOs;

public class UserDto // lấy thông tin lên để sử dụng
{   
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    // public required string Token { get; set; } jwt

}