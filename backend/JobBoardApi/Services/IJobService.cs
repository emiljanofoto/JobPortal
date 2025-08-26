using JobBoardApi.DTOs;

namespace JobBoardApi.Services
{
    public interface IJobService
    {
        Task<List<JobDto>> GetJobsAsync();
        Task<List<JobDto>> GetJobsWithApplicationsAsync();
        Task<JobDto> GetJobByIdAsync(string id);
        Task<JobDto> CreateJobAsync(CreateJobDto createJobDto);
        Task<JobDto> UpdateJobAsync(string id, UpdateJobDto updateJobDto);
        Task DeleteJobAsync(string id);
        Task<JobApplicationDto> ApplyToJobAsync(string jobId, string candidateId, CreateJobApplicationDto applicationDto);
    }
}