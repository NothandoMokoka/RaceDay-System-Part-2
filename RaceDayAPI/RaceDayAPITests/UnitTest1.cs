using System.ComponentModel.DataAnnotations;
using RaceDayAPI.DTOs;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EventValidationTests
    {
        [Fact]
        public void EventRequest_WithValidData_PassesValidation()
        {
            var request = new EventRequest
            {
                Name = "Test Running Event",
                Description = "A test event",
                EventDate = DateTime.Now.AddDays(7),
                Location = "Pretoria",
                Distance = 5,
                EventType = "Running"
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                request, context, results, validateAllProperties: true);

            Assert.True(isValid);
        }

        [Fact]
        public void EventRequest_WithInvalidDistance_FailsValidation()
        {
            var request = new EventRequest
            {
                Name = "Test Running Event",
                EventDate = DateTime.Now.AddDays(7),
                Location = "Pretoria",
                Distance = 0,
                EventType = "Running"
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                request, context, results, validateAllProperties: true);

            Assert.False(isValid);
        }
    }
}