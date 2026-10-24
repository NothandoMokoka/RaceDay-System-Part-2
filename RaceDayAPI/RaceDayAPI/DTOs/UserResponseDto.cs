// Defines the user information returned by the API.
namespace RaceDayAPI.DTOs
{
    public class UserResponseDto
    {
        public int UserID { get; set; }

        public int RoleID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
