namespace DiamondAssessmentSystem.Application.DTO
{
    public class AccountDto
    {
        public string UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public enum RoleEnum
    {
        Customer = 1,
        Admin = 2,
        Staff = 3
    }
}
