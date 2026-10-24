using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class CategoryRequest
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 120)]
        public int MinimumAge { get; set; }

        [Range(0, 120)]
        public int MaximumAge { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal? Distance { get; set; }
    }
}