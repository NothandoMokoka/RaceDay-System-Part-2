// Represents a user role that determines access permissions.
namespace RaceDayAPI.Models
{
    public class Role
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
