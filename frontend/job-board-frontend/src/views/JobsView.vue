<template>
  <v-container>
    <v-row>
      <v-col>
        <div class="d-flex justify-space-between align-center mb-6">
          <h1>Available Jobs</h1>
          <v-btn color="error" @click="handleLogout">
            <v-icon left>mdi-logout</v-icon>
            Logout
          </v-btn>
        </div>
        
        <v-progress-linear v-if="loading" indeterminate />
        
        <v-row v-else>
          <v-col
            v-for="job in jobs"
            :key="job.id"
            cols="12"
            md="6"
            lg="4"
          >
            <v-card class="mb-4" elevation="2">
              <v-card-title>{{ job.title }}</v-card-title>
              <v-card-subtitle>{{ job.location }}</v-card-subtitle>
              <v-card-text>
                <p>{{ job.description }}</p>
                <v-chip color="primary" size="small" class="mt-2">
                  Expires: {{ formatDate(job.expirationDate) }}
                </v-chip>
              </v-card-text>
              <v-card-actions>
                <v-spacer />
                <v-btn
                  color="primary"
                  @click="openApplicationDialog(job)"
                >
                  Apply
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>

        <v-alert v-if="!loading && jobs.length === 0" type="info">
          No jobs available at the moment.
        </v-alert>
      </v-col>
    </v-row>

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
  </v-container>
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

onMounted(() => {
  jobsStore.fetchJobs()
})

function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString()
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