using System;

namespace RaceDayAPI.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }

        public int ParticipantID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; } = "CONFIRMED";

        public User? Participant { get; set; }

        public Event? Event { get; set; }

        public Category? Category { get; set; }
        public Result? Result { get ; set; }   
    }
}