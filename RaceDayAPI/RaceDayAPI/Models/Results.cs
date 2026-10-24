// Represents the results recorded for a race event participant.
using System;

namespace RaceDayAPI.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrollmentID { get; set; }

        public int FinishPosition { get; set; }

        public TimeSpan FinishTime { get; set; }

        public DateTime RecordedAt { get; set; }

        public Enrollment? Enrollment { get; set; }
    }
}
