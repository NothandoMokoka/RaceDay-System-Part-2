using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class EnrollmentRequest
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}