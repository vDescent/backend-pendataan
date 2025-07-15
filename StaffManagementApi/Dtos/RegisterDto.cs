namespace StaffManagementApi.Dtos
{
    public class RegisterDto
    {
        public required string Nama { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
