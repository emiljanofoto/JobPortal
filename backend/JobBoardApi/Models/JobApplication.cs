using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobBoardApi.Models
{
    public class JobApplication
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Job")]
        public string JobId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string CandidateId { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Job Job { get; set; } = null!;
        public User Candidate { get; set; } = null!;
    }
}