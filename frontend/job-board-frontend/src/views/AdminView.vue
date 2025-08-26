<template>
  <v-container>
    <v-row>
      <v-col>
        <div class="d-flex justify-space-between align-center mb-6">
          <h1>Admin Dashboard</h1>
          <div>
            <v-btn color="primary" class="mr-2" @click="openJobDialog()">
              <v-icon left>mdi-plus</v-icon>
              New Job
            </v-btn>
            <v-btn color="error" @click="handleLogout">
              <v-icon left>mdi-logout</v-icon>
              Logout
            </v-btn>
          </div>
        </div>

        <v-progress-linear v-if="loading" indeterminate />

        <v-data-table
          v-else
          :headers="headers"
          :items="jobs"
          :sort-by="[{ key: 'createdAt', order: 'desc' }]"
        >
          <template v-slot:item.expirationDate="{ item }">
            {{ formatDate(item.expirationDate) }}
          </template>
          
          <template v-slot:item.applicationsCount="{ item }">
            <v-chip color="primary" size="small">
              {{ item.applications?.length || 0 }} applications
            </v-chip>
          </template>

          <template v-slot:item.actions="{ item }">
            <v-btn
              icon="mdi-eye"
              size="small"
              @click="viewApplications(item)"
              class="mr-2"
            />
            <v-btn
              icon="mdi-pencil"
              size="small"
              @click="editJob(item)"
              class="mr-2"
            />
            <v-btn
              icon="mdi-delete"
              size="small"
              color="error"
              @click="confirmDeleteJob(item)"
            />
          </template>
        </v-data-table>
      </v-col>
    </v-row>

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

const headers = [
  { title: 'Title', key: 'title', sortable: true },
  { title: 'Location', key: 'location', sortable: true },
  { title: 'Expiration Date', key: 'expirationDate', sortable: true },
  { title: 'Applications', key: 'applicationsCount', sortable: false },
  { title: 'Actions', key: 'actions', sortable: false }
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

onMounted(() => {
  jobsStore.fetchJobsWithApplications()
})

function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString()
}

function formatDateTime(dateString: string) {
  return new Date(dateString).toLocaleString()
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