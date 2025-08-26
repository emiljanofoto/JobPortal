import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from './auth'

export interface Job {
  id: string
  title: string
  description: string
  location: string
  expirationDate: string
  createdAt: string
  applications?: JobApplication[]
}

export interface JobApplication {
  id: string
  jobId: string
  candidateId: string
  message: string
  appliedAt: string
  candidate?: {
    id: string
    email: string
    firstName: string
    lastName: string
  }
}

export const useJobsStore = defineStore('jobs', () => {
  const jobs = ref<Job[]>([])
  const loading = ref(false)
  const authStore = useAuthStore()

  async function fetchJobs() {
    loading.value = true
    try {
      const response = await fetch('http://localhost:5000/api/jobs', {
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
      })

      if (!response.ok) {
        throw new Error('Failed to fetch jobs')
      }

      jobs.value = await response.json()
    } catch (error) {
      console.error('Error fetching jobs:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function fetchJobsWithApplications() {
    loading.value = true
    try {
      const response = await fetch('http://localhost:5000/api/jobs/with-applications', {
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
      })

      if (!response.ok) {
        throw new Error('Failed to fetch jobs with applications')
      }

      jobs.value = await response.json()
    } catch (error) {
      console.error('Error fetching jobs with applications:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createJob(jobData: Omit<Job, 'id' | 'createdAt'>) {
    try {
      const response = await fetch('http://localhost:5000/api/jobs', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(jobData),
      })

      if (!response.ok) {
        throw new Error('Failed to create job')
      }

      const newJob = await response.json()
      jobs.value.push(newJob)
      return newJob
    } catch (error) {
      console.error('Error creating job:', error)
      throw error
    }
  }

  async function updateJob(id: string, jobData: Partial<Job>) {
    try {
      const response = await fetch(`http://localhost:5000/api/jobs/${id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(jobData),
      })

      if (!response.ok) {
        throw new Error('Failed to update job')
      }

      const updatedJob = await response.json()
      const index = jobs.value.findIndex(j => j.id === id)
      if (index !== -1) {
        jobs.value[index] = updatedJob
      }
      return updatedJob
    } catch (error) {
      console.error('Error updating job:', error)
      throw error
    }
  }

  async function deleteJob(id: string) {
    try {
      const response = await fetch(`http://localhost:5000/api/jobs/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
      })

      if (!response.ok) {
        throw new Error('Failed to delete job')
      }

      jobs.value = jobs.value.filter(j => j.id !== id)
    } catch (error) {
      console.error('Error deleting job:', error)
      throw error
    }
  }

  async function applyToJob(jobId: string, message: string) {
    try {
      const response = await fetch(`http://localhost:5000/api/jobs/${jobId}/apply`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${authStore.token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ message }),
      })

      if (!response.ok) {
        throw new Error('Failed to apply to job')
      }

      return await response.json()
    } catch (error) {
      console.error('Error applying to job:', error)
      throw error
    }
  }

  return {
    jobs,
    loading,
    fetchJobs,
    fetchJobsWithApplications,
    createJob,
    updateJob,
    deleteJob,
    applyToJob
  }
})