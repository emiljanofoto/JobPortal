using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobBoardApi.DTOs;
using JobBoardApi.Services;

namespace JobBoardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobDto>>> GetJobs()
        {
            try
            {
                var jobs = await _jobService.GetJobsAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching jobs" });
            }
        }

        [HttpGet("with-applications")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<JobDto>>> GetJobsWithApplications()
        {
            try
            {
                var jobs = await _jobService.GetJobsWithApplicationsAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching jobs with applications" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobDto>> GetJob(string id)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the job" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<JobDto>> CreateJob([FromBody] CreateJobDto createJobDto)
        {
            try
            {
                if (createJobDto.ExpirationDate <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Expiration date must be in the future" });
                }

                var job = await _jobService.CreateJobAsync(createJobDto);
                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the job" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<JobDto>> UpdateJob(string id, [FromBody] UpdateJobDto updateJobDto)
        {
            try
            {
                if (updateJobDto.ExpirationDate.HasValue && updateJobDto.ExpirationDate <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Expiration date must be in the future" });
                }

                var job = await _jobService.UpdateJobAsync(id, updateJobDto);
                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the job" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteJob(string id)
        {
            try
            {
                await _jobService.DeleteJobAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the job" });
            }
        }

        [HttpPost("{id}/apply")]
        [Authorize(Roles = "candidate")]
        public async Task<ActionResult<JobApplicationDto>> ApplyToJob(string id, [FromBody] CreateJobApplicationDto applicationDto)
        {
            try
            {
                var candidateId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(candidateId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var application = await _jobService.ApplyToJobAsync(id, candidateId, applicationDto);
                return Ok(application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while applying to the job" });
            }
        }
    }
}