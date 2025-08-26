using System.ComponentModel.DataAnnotations;

namespace JobBoardApi.DTOs
{
    public class JobDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<JobApplicationDto>? Applications { get; set; }
    }

    public class CreateJobDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime ExpirationDate { get; set; }
    }

    public class UpdateJobDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class JobApplicationDto
    {
        public string Id { get; set; } = string.Empty;
        public string JobId { get; set; } = string.Empty;
        public string CandidateId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public UserDto? Candidate { get; set; }
    }

    public class CreateJobApplicationDto
    {
        [Required]
        [MinLength(10)]
        public string Message { get; set; } = string.Empty;
    }
}