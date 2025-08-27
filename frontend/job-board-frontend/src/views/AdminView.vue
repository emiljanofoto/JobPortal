<template>
  <div class="admin-container">
    <!-- Header Section -->
    <section class="admin-header">
      <v-container class="px-4">
        <div class="header-content py-6">
          <div class="header-main mb-4">
            <div class="header-info">
              <h1 class="admin-title">Admin Dashboard</h1>
              <p class="admin-subtitle">Manage job postings and track applications</p>
            </div>
            <div class="header-actions">
              <v-btn
                color="primary"
                size="large"
                rounded="lg"
                class="mr-3"
                @click="openJobDialog()"
              >
                <v-icon start>mdi-plus</v-icon>
                New Job
              </v-btn>
              <v-btn
                color="error"
                variant="outlined"
                size="large"
                rounded="lg"
                @click="handleLogout"
              >
                <v-icon start>mdi-logout</v-icon>
                Logout
              </v-btn>
            </div>
          </div>

          <!-- Stats Cards -->
          <v-row class="stats-row">
            <v-col cols="12" sm="6" md="3">
              <v-card class="stats-card" rounded="xl" elevation="4">
                <v-card-text class="text-center">
                  <v-avatar size="60" color="primary" variant="tonal" class="mb-3">
                    <v-icon size="30">mdi-briefcase</v-icon>
                  </v-avatar>
                  <div class="stats-number">{{ jobs.length }}</div>
                  <div class="stats-label">Total Jobs</div>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="6" md="3">
              <v-card class="stats-card" rounded="xl" elevation="4">
                <v-card-text class="text-center">
                  <v-avatar size="60" color="success" variant="tonal" class="mb-3">
                    <v-icon size="30">mdi-check-circle</v-icon>
                  </v-avatar>
                  <div class="stats-number">{{ activeJobsCount }}</div>
                  <div class="stats-label">Active Jobs</div>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="6" md="3">
              <v-card class="stats-card" rounded="xl" elevation="4">
                <v-card-text class="text-center">
                  <v-avatar size="60" color="orange" variant="tonal" class="mb-3">
                    <v-icon size="30">mdi-account-group</v-icon>
                  </v-avatar>
                  <div class="stats-number">{{ totalApplicationsCount }}</div>
                  <div class="stats-label">Applications</div>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="6" md="3">
              <v-card class="stats-card" rounded="xl" elevation="4">
                <v-card-text class="text-center">
                  <v-avatar size="60" color="info" variant="tonal" class="mb-3">
                    <v-icon size="30">mdi-trending-up</v-icon>
                  </v-avatar>
                  <div class="stats-number">{{ Math.round(totalApplicationsCount / (jobs.length || 1)) }}</div>
                  <div class="stats-label">Avg Applications</div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </div>
      </v-container>
    </section>

    <!-- Jobs Table Section -->
    <v-container class="jobs-table-section px-4">
      <!-- Loading State -->
      <div v-if="loading" class="loading-section">
        <div class="text-center py-12">
          <v-progress-circular
            size="64"
            width="4"
            color="primary"
            indeterminate
          />
          <p class="mt-4 text-h6">Loading dashboard data...</p>
        </div>
      </div>

      <!-- Jobs Table -->
      <v-card v-else class="jobs-table-card" rounded="xl" elevation="8">
        <v-card-title class="table-header">
          <h2 class="table-title">Job Postings</h2>
          <v-spacer />
          <v-btn
            color="primary"
            variant="outlined"
            prepend-icon="mdi-refresh"
            @click="jobsStore.fetchJobsWithApplications()"
          >
            Refresh
          </v-btn>
        </v-card-title>

        <v-data-table
          :headers="headers"
          :items="jobs"
          :sort-by="[{ key: 'createdAt', order: 'desc' }]"
          class="enhanced-table"
        >
          <template v-slot:item.title="{ item }">
            <div class="job-title-cell">
              <div class="job-title">{{ item.title }}</div>
              <div class="job-location">
                <v-icon size="14" class="mr-1">mdi-map-marker</v-icon>
                {{ item.location }}
              </div>
            </div>
          </template>

          <template v-slot:item.expirationDate="{ item }">
            <v-chip
              :color="isJobExpiringSoon(item.expirationDate) ? 'orange' : new Date(item.expirationDate) > new Date() ? 'success' : 'error'"
              size="small"
              variant="tonal"
            >
              <v-icon start size="14">
                {{ new Date(item.expirationDate) > new Date() ? 'mdi-clock-outline' : 'mdi-clock-alert' }}
              </v-icon>
              {{ formatDate(item.expirationDate) }}
            </v-chip>
          </template>
          
          <template v-slot:item.applicationsCount="{ item }">
            <v-chip 
              :color="getApplicationCountColor(item.applications?.length || 0)"
              size="small"
              variant="elevated"
            >
              <v-icon start size="14">mdi-account-group</v-icon>
              {{ item.applications?.length || 0 }}
            </v-chip>
          </template>

          <template v-slot:item.actions="{ item }">
            <div class="action-buttons">
              <v-tooltip text="View Applications">
                <template v-slot:activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-eye"
                    size="small"
                    color="info"
                    variant="tonal"
                    @click="viewApplications(item)"
                    class="mr-1"
                  />
                </template>
              </v-tooltip>
              
              <v-tooltip text="Edit Job">
                <template v-slot:activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-pencil"
                    size="small"
                    color="primary"
                    variant="tonal"
                    @click="editJob(item)"
                    class="mr-1"
                  />
                </template>
              </v-tooltip>
              
              <v-tooltip text="Delete Job">
                <template v-slot:activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-delete"
                    size="small"
                    color="error"
                    variant="tonal"
                    @click="confirmDeleteJob(item)"
                  />
                </template>
              </v-tooltip>
            </div>
          </template>
        </v-data-table>
      </v-card>
    </v-container>

    <!-- Job Dialog (Create/Edit) -->
    <v-dialog v-model="jobDialog" max-width="800px">
      <v-card>
        <v-card-title>
          {{ editingJob ? 'Edit Job' : 'Create New Job' }}
        </v-card-title>
        <v-card-text>
          <v-form @submit.prevent="saveJob">
            <v-text-field
              v-model="jobForm.title"
              label="Job Title"
              :rules="titleRules"
              required
            />
            <v-textarea
              v-model="jobForm.description"
              label="Job Description"
              :rules="descriptionRules"
              rows="4"
              required
            />
            <v-text-field
              v-model="jobForm.location"
              label="Location"
              :rules="locationRules"
              required
            />
            <v-text-field
              v-model="jobForm.expirationDate"
              label="Expiration Date"
              type="date"
              :rules="dateRules"
              required
            />
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text @click="closeJobDialog">Cancel</v-btn>
          <v-btn
            color="primary"
            :loading="saving"
            @click="saveJob"
          >
            {{ editingJob ? 'Update' : 'Create' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Applications Dialog -->
    <v-dialog v-model="applicationsDialog" max-width="900px">
      <v-card>
        <v-card-title>
          Applications for {{ selectedJob?.title }}
        </v-card-title>
        <v-card-text>
          <v-data-table
            :headers="applicationHeaders"
            :items="selectedJob?.applications || []"
            :sort-by="[{ key: 'appliedAt', order: 'desc' }]"
          >
            <template v-slot:item.candidateName="{ item }">
              {{ item.candidate?.firstName }} {{ item.candidate?.lastName }}
            </template>
            <template v-slot:item.appliedAt="{ item }">
              {{ formatDateTime(item.appliedAt) }}
            </template>
          </v-data-table>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text @click="applicationsDialog = false">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title>Confirm Delete</v-card-title>
        <v-card-text>
          Are you sure you want to delete the job "{{ jobToDelete?.title }}"?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text @click="deleteDialog = false">Cancel</v-btn>
          <v-btn
            color="error"
            :loading="deleting"
            @click="deleteJob"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Snackbar -->
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
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useJobsStore, type Job } from '../stores/jobs'

const router = useRouter()
const authStore = useAuthStore()
const jobsStore = useJobsStore()

const { jobs, loading } = jobsStore

const headers = [
  { title: 'Job Details', key: 'title', sortable: true, width: '30%' },
  { title: 'Status', key: 'expirationDate', sortable: true, width: '20%' },
  { title: 'Applications', key: 'applicationsCount', sortable: false, width: '15%' },
  { title: 'Actions', key: 'actions', sortable: false, width: '15%', align: 'center' }
]

const applicationHeaders = [
  { title: 'Candidate', key: 'candidateName', sortable: true },
  { title: 'Email', key: 'candidate.email', sortable: true },
  { title: 'Applied At', key: 'appliedAt', sortable: true },
  { title: 'Message', key: 'message', sortable: false }
]

const jobDialog = ref(false)
const applicationsDialog = ref(false)
const deleteDialog = ref(false)
const editingJob = ref(false)
const saving = ref(false)
const deleting = ref(false)

const selectedJob = ref<Job | null>(null)
const jobToDelete = ref<Job | null>(null)

const jobForm = ref({
  title: '',
  description: '',
  location: '',
  expirationDate: ''
})

const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')

const titleRules = [
  (v: string) => !!v || 'Title is required'
]

const descriptionRules = [
  (v: string) => !!v || 'Description is required'
]

const locationRules = [
  (v: string) => !!v || 'Location is required'
]

const dateRules = [
  (v: string) => !!v || 'Expiration date is required',
  (v: string) => new Date(v) > new Date() || 'Expiration date must be in the future'
]

const activeJobsCount = computed(() => {
  return jobs.value.filter(job => new Date(job.expirationDate) > new Date()).length
})

const totalApplicationsCount = computed(() => {
  return jobs.value.reduce((total, job) => total + (job.applications?.length || 0), 0)
})

onMounted(() => {
  jobsStore.fetchJobsWithApplications()
})

function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString()
}

function formatDateTime(dateString: string) {
  return new Date(dateString).toLocaleString()
}

function isJobExpiringSoon(dateString: string) {
  const expiryDate = new Date(dateString)
  const now = new Date()
  const diffTime = expiryDate.getTime() - now.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  return diffDays <= 7 && diffDays > 0
}

function getApplicationCountColor(count: number) {
  if (count === 0) return 'grey'
  if (count <= 2) return 'orange'
  if (count <= 5) return 'primary'
  return 'success'
}

function openJobDialog(job?: Job) {
  if (job) {
    editingJob.value = true
    selectedJob.value = job
    jobForm.value = {
      title: job.title,
      description: job.description,
      location: job.location,
      expirationDate: job.expirationDate.split('T')[0]
    }
  } else {
    editingJob.value = false
    selectedJob.value = null
    jobForm.value = {
      title: '',
      description: '',
      location: '',
      expirationDate: ''
    }
  }
  jobDialog.value = true
}

function closeJobDialog() {
  jobDialog.value = false
  editingJob.value = false
  selectedJob.value = null
}

function editJob(job: Job) {
  openJobDialog(job)
}

function viewApplications(job: Job) {
  selectedJob.value = job
  applicationsDialog.value = true
}

function confirmDeleteJob(job: Job) {
  jobToDelete.value = job
  deleteDialog.value = true
}

async function saveJob() {
  if (!jobForm.value.title || !jobForm.value.description || 
      !jobForm.value.location || !jobForm.value.expirationDate) {
    showSnackbar('Please fill in all fields', 'error')
    return
  }

  saving.value = true

  try {
    if (editingJob.value && selectedJob.value) {
      await jobsStore.updateJob(selectedJob.value.id, jobForm.value)
      showSnackbar('Job updated successfully!', 'success')
    } else {
      await jobsStore.createJob(jobForm.value)
      showSnackbar('Job created successfully!', 'success')
    }
    closeJobDialog()
  } catch (error) {
    showSnackbar('Failed to save job. Please try again.', 'error')
  } finally {
    saving.value = false
  }
}

async function deleteJob() {
  if (!jobToDelete.value) return

  deleting.value = true

  try {
    await jobsStore.deleteJob(jobToDelete.value.id)
    showSnackbar('Job deleted successfully!', 'success')
    deleteDialog.value = false
    jobToDelete.value = null
  } catch (error) {
    showSnackbar('Failed to delete job. Please try again.', 'error')
  } finally {
    deleting.value = false
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
.admin-container {
  min-height: 100vh;
  background: #f8f9fa;
}

.admin-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.header-main {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.admin-title {
  font-size: clamp(2rem, 4vw, 2.5rem);
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.admin-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
}

.header-actions .v-btn {
  font-weight: 600;
}

.header-actions .v-btn--variant-outlined {
  border-color: rgba(255, 255, 255, 0.5);
  color: white;
}

.header-actions .v-btn--variant-outlined:hover {
  background: rgba(255, 255, 255, 0.1);
}

.stats-row {
  margin-top: 2rem;
}

.stats-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
}

.stats-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15) !important;
}

.stats-number {
  font-size: 2.5rem;
  font-weight: 700;
  color: #2c3e50;
  line-height: 1;
}

.stats-label {
  font-size: 0.9rem;
  color: #6c757d;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.jobs-table-section {
  padding: 2rem 0 4rem;
}

.jobs-table-card {
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.table-header {
  padding: 2rem 2rem 1rem;
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);
}

.table-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #2c3e50;
}

.enhanced-table {
  background: transparent;
}

:deep(.enhanced-table .v-data-table__td) {
  padding: 16px !important;
  vertical-align: middle;
}

:deep(.enhanced-table .v-data-table__th) {
  font-weight: 600 !important;
  color: #2c3e50 !important;
  padding: 16px !important;
}

.job-title-cell .job-title {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.25rem;
}

.job-title-cell .job-location {
  font-size: 0.85rem;
  color: #6c757d;
  display: flex;
  align-items: center;
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
}

.loading-section {
  min-height: 50vh;
  display: flex;
  align-items: center;
  justify-content: center;
}

@media (max-width: 960px) {
  .header-main {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
  
  .stats-number {
    font-size: 2rem;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 0.125rem;
  }
}

@media (max-width: 600px) {
  .table-header {
    padding: 1rem;
  }
  
  .table-title {
    font-size: 1.25rem;
  }
  
  :deep(.enhanced-table .v-data-table__td),
  :deep(.enhanced-table .v-data-table__th) {
    padding: 8px !important;
  }
}
</style>