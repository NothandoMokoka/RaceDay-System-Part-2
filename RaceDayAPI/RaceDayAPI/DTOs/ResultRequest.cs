using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class ResultRequest
    {
        [Required]
        public int EnrollmentID { get; set; }

        [Range(1, int.MaxValue)]
        public int FinishPosition { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }
    }
}