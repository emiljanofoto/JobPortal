using Microsoft.EntityFrameworkCore;
using JobBoardApi.Data;
using JobBoardApi.DTOs;
using JobBoardApi.Models;

namespace JobBoardApi.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobDto>> GetJobsAsync()
        {
            var jobs = await _context.Jobs
                .Where(j => j.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return jobs.Select(job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExpirationDate = job.ExpirationDate,
                CreatedAt = job.CreatedAt
            }).ToList();
        }

        public async Task<List<JobDto>> GetJobsWithApplicationsAsync()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Candidate)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return jobs.Select(job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExpirationDate = job.ExpirationDate,
                CreatedAt = job.CreatedAt,
                Applications = job.Applications.Select(app => new JobApplicationDto
                {
                    Id = app.Id,
                    JobId = app.JobId,
                    CandidateId = app.CandidateId,
                    Message = app.Message,
                    AppliedAt = app.AppliedAt,
                    Candidate = new UserDto
                    {
                        Id = app.Candidate.Id,
                        Email = app.Candidate.Email,
                        FirstName = app.Candidate.FirstName,
                        LastName = app.Candidate.LastName,
                        Role = app.Candidate.Role
                    }
                }).ToList()
            }).ToList();
        }

        public async Task<JobDto> GetJobByIdAsync(string id)
        {
            var job = await _context.Jobs
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Candidate)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                throw new KeyNotFoundException("Job not found");
            }

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExpirationDate = job.ExpirationDate,
                CreatedAt = job.CreatedAt,
                Applications = job.Applications.Select(app => new JobApplicationDto
                {
                    Id = app.Id,
                    JobId = app.JobId,
                    CandidateId = app.CandidateId,
                    Message = app.Message,
                    AppliedAt = app.AppliedAt,
                    Candidate = new UserDto
                    {
                        Id = app.Candidate.Id,
                        Email = app.Candidate.Email,
                        FirstName = app.Candidate.FirstName,
                        LastName = app.Candidate.LastName,
                        Role = app.Candidate.Role
                    }
                }).ToList()
            };
        }

        public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto)
        {
            var job = new Job
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                Location = createJobDto.Location,
                ExpirationDate = createJobDto.ExpirationDate
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExpirationDate = job.ExpirationDate,
                CreatedAt = job.CreatedAt
            };
        }

        public async Task<JobDto> UpdateJobAsync(string id, UpdateJobDto updateJobDto)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                throw new KeyNotFoundException("Job not found");
            }

            if (!string.IsNullOrEmpty(updateJobDto.Title))
                job.Title = updateJobDto.Title;

            if (!string.IsNullOrEmpty(updateJobDto.Description))
                job.Description = updateJobDto.Description;

            if (!string.IsNullOrEmpty(updateJobDto.Location))
                job.Location = updateJobDto.Location;

            if (updateJobDto.ExpirationDate.HasValue)
                job.ExpirationDate = updateJobDto.ExpirationDate.Value;

            await _context.SaveChangesAsync();

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExpirationDate = job.ExpirationDate,
                CreatedAt = job.CreatedAt
            };
        }

        public async Task DeleteJobAsync(string id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                throw new KeyNotFoundException("Job not found");
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }

        public async Task<JobApplicationDto> ApplyToJobAsync(string jobId, string candidateId, CreateJobApplicationDto applicationDto)
        {
            // Check if job exists and is not expired
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                throw new KeyNotFoundException("Job not found");
            }

            if (job.ExpirationDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Job has expired");
            }

            // Check if candidate exists
            var candidate = await _context.Users.FindAsync(candidateId);
            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate not found");
            }

            // Check if candidate already applied
            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.CandidateId == candidateId);

            if (existingApplication != null)
            {
                throw new InvalidOperationException("You have already applied to this job");
            }

            // Create application
            var application = new JobApplication
            {
                JobId = jobId,
                CandidateId = candidateId,
                Message = applicationDto.Message
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            return new JobApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                CandidateId = application.CandidateId,
                Message = application.Message,
                AppliedAt = application.AppliedAt,
                Candidate = new UserDto
                {
                    Id = candidate.Id,
                    Email = candidate.Email,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    Role = candidate.Role
                }
            };
        }
    }
}