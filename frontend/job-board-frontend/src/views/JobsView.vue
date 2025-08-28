<template>
  <div class="jobs-container">
    <!-- Header Section -->
    <section class="jobs-header">
      <v-container class="px-4">
        <div class="d-flex justify-space-between align-center py-6">
          <div class="header-content">
            <h1 class="jobs-title">Available Opportunities</h1>
            <p class="jobs-subtitle">Discover your next career move</p>
          </div>
          <div class="header-actions">
            <v-btn
              color="error"
              variant="outlined"
              prepend-icon="mdi-logout"
              @click="handleLogout"
              class="logout-btn"
            >
              Logout
            </v-btn>
          </div>
        </div>
      </v-container>
    </section>

    <!-- Jobs Content -->
    <v-container class="jobs-content px-4">
      <!-- Loading State -->
      <div v-if="loading" class="loading-section">
        <div class="text-center py-12">
          <v-progress-circular
            size="64"
            width="4"
            color="primary"
            indeterminate
          />
          <p class="mt-4 text-h6">Loading amazing opportunities...</p>
        </div>
      </div>
      
      <!-- Jobs Grid -->
      <v-row v-else-if="jobs.length > 0" class="jobs-grid">
        <v-col
          v-for="job in jobs"
          :key="job.id"
          cols="12"
          md="6"
          lg="4"
          class="job-col"
        >
          <v-card class="job-card h-100" elevation="8" rounded="xl">
            <!-- Job Header -->
            <div class="job-header">
              <div class="job-company-icon">
                <v-avatar size="56" color="primary" variant="tonal">
                  <v-icon size="28">mdi-office-building</v-icon>
                </v-avatar>
              </div>
              <div class="job-status">
                <v-chip color="success" size="small" variant="elevated">
                  <v-icon start size="14">mdi-circle</v-icon>
                  Active
                </v-chip>
              </div>
            </div>

            <v-card-text class="job-content">
              <h3 class="job-title">{{ job.title }}</h3>
              
              <div class="job-meta mb-4">
                <div class="job-location">
                  <v-icon size="16" class="mr-1">mdi-map-marker</v-icon>
                  {{ job.location }}
                </div>
                <div class="job-posted">
                  <v-icon size="16" class="mr-1">mdi-calendar</v-icon>
                  {{ formatDate(job.createdAt) }}
                </div>
              </div>

              <p class="job-description">{{ truncateText(job.description, 150) }}</p>
              
              <div class="job-expiry">
                <v-chip
                  :color="isExpiringSoon(job.expirationDate) ? 'orange' : 'primary'"
                  size="small"
                  variant="tonal"
                >
                  <v-icon start size="14">mdi-clock-outline</v-icon>
                  Expires {{ formatDate(job.expirationDate) }}
                </v-chip>
              </div>
            </v-card-text>

            <v-card-actions class="job-actions">
              <v-btn
                color="primary"
                size="large"
                rounded="lg"
                block
                @click="openApplicationDialog(job)"
                class="apply-btn"
              >
                <v-icon start>mdi-send</v-icon>
                Apply Now
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>

      <!-- Empty State -->
      <div v-else class="empty-state">
        <v-card class="empty-card" elevation="0" rounded="xl">
          <v-card-text class="text-center py-12">
            <v-avatar size="120" color="grey-lighten-3" class="mb-6">
              <v-icon size="60" color="grey">mdi-briefcase-search</v-icon>
            </v-avatar>
            <h2 class="mb-4">No Jobs Available</h2>
            <p class="text-body-1 mb-6">
              There are no job postings available at the moment. Please check back later!
            </p>
            <v-btn color="primary" @click="jobsStore.fetchJobs()">
              <v-icon start>mdi-refresh</v-icon>
              Refresh
            </v-btn>
          </v-card-text>
        </v-card>
      </div>
    </v-container>

    <!-- Application Dialog -->
    <v-dialog v-model="applicationDialog" max-width="600px">
      <v-card>
        <v-card-title>Apply for {{ selectedJob?.title }}</v-card-title>
        <v-card-text>
          <v-textarea
            v-model="applicationMessage"
            label="Motivation Message"
            placeholder="Tell us why you're interested in this position..."
            rows="4"
            :rules="messageRules"
            required
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text @click="closeApplicationDialog">Cancel</v-btn>
          <v-btn
            color="primary"
            :loading="applying"
            @click="submitApplication"
          >
            Submit Application
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success/Error Snackbar -->
    <v-snackbar
      v-model="snackbar"
      :color="snackbarColor"
      timeout="3000"
    >
      {{ snackbarMessage }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useJobsStore, type Job } from '../stores/jobs'

const router = useRouter()
const authStore = useAuthStore()
const jobsStore = useJobsStore()

const { jobs, loading } = jobsStore
const applicationDialog = ref(false)
const selectedJob = ref<Job | null>(null)
const applicationMessage = ref('')
const applying = ref(false)

const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')

const messageRules = [
  (v: string) => !!v || 'Message is required', 
  (v: string) => v.length >= 10 || 'Message must be at least 10 characters'
]

onMounted(async () => {
  try {
    await jobsStore.fetchJobs()
  } catch (error) {
    console.error('Error fetching jobs:', error)
  }
})

function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString()
}

function truncateText(text: string, maxLength: number) {
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

function isExpiringSoon(dateString: string) {
  const expiryDate = new Date(dateString)
  const now = new Date()
  const diffTime = expiryDate.getTime() - now.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  return diffDays <= 7 && diffDays > 0
}

function openApplicationDialog(job: Job) {
  selectedJob.value = job
  applicationMessage.value = ''
  applicationDialog.value = true
}

function closeApplicationDialog() {
  applicationDialog.value = false
  selectedJob.value = null
  applicationMessage.value = ''
}

async function submitApplication() {
  if (!selectedJob.value || !applicationMessage.value) {
    showSnackbar('Please fill in all required fields', 'error')
    return
  }

  applying.value = true

  try {
    await jobsStore.applyToJob(selectedJob.value.id, applicationMessage.value)
    showSnackbar('Application submitted successfully!', 'success')
    closeApplicationDialog()
  } catch (error) {
    showSnackbar('Failed to submit application. Please try again.', 'error')
  } finally {
    applying.value = false
  }
}

function showSnackbar(message: string, color: string) {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.jobs-container {
  min-height: 100vh;
  background: #f8f9fa;
}

.jobs-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.jobs-title {
  font-size: clamp(2rem, 4vw, 2.5rem);
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.jobs-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
}

.logout-btn {
  border-color: rgba(255, 255, 255, 0.5);
  color: white;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.1);
}

.jobs-content {
  padding-top: 2rem;
  padding-bottom: 4rem;
}

.job-col {
  margin-bottom: 1.5rem;
}

.job-card {
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
  background: white;
}

.job-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15) !important;
}

.job-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.5rem 1.5rem 0;
}

.job-content {
  padding: 1rem 1.5rem;
}

.job-title {
  font-size: 1.4rem;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 1rem;
  line-height: 1.3;
}

.job-meta {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.job-location,
.job-posted {
  display: flex;
  align-items: center;
  color: #6c757d;
  font-size: 0.9rem;
}

.job-description {
  color: #495057;
  line-height: 1.6;
  margin-bottom: 1rem;
}

.job-actions {
  padding: 0 1.5rem 1.5rem;
}

.apply-btn {
  font-weight: 600;
  text-transform: none;
  font-size: 1rem;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 50vh;
}

.empty-card {
  max-width: 500px;
  width: 100%;
}

.loading-section {
  min-height: 50vh;
  display: flex;
  align-items: center;
  justify-content: center;
}

@media (max-width: 960px) {
  .jobs-header .d-flex {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
  
  .job-meta {
    flex-direction: column;
    gap: 0.5rem;
  }
}

@media (max-width: 600px) {
  .job-header {
    padding: 1rem 1rem 0;
  }
  
  .job-content {
    padding: 1rem;
  }
  
  .job-actions {
    padding: 0 1rem 1rem;
  }
}
</style>