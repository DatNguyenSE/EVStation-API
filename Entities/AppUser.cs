namespace API.Entities;

public class AppUser
{
    public String Id { get; set; } = Guid.NewGuid().ToString(); // random & consistent if adding manually
    public required String UserName { get; set; }
    public required String Email { get; set; }
    public required byte[] PasswordHash { get; set; }   // save encryption
    public required byte[] PasswordSalt { get; set; }   // key to decryption


    //navigation property   
    // public EVCars car { get; set; } = null!;

}
