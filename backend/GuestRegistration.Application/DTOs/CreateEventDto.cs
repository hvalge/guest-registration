using System.ComponentModel.DataAnnotations;

namespace GuestRegistration.Application.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? AdditionalInformation { get; set; }
    }
}