using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class EventRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Distance { get; set; }

        [Required]
        [RegularExpression("^(Running|Walking|Cycling)$")]
        public string EventType { get; set; } = string.Empty;
    }
}