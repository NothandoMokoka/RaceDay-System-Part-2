// Represents a category linked to a race event.
namespace RaceDayAPI.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int MinimumAge { get; set; }

        public int MaximumAge { get; set; }

        public decimal? Distance { get; set; }

        public Event? Event { get; set; }
    }
}
