<template>
  <div class="admin-container">
    <!-- Header with gradient background -->
    <div class="header-section">
      <v-container fluid class="px-4">
        <v-row>
          <v-col cols="12">
            <div class="d-flex justify-space-between align-center">
              <div class="header-content">
                <h1 class="text-h3 mb-2 text-white font-weight-bold">Admin Dashboard</h1>
                <p class="text-subtitle-1 text-white opacity-90">Manage job postings and applications</p>
                <div class="header-stats mt-3">
                  <v-chip class="mr-2" color="white" text-color="primary" size="small">
                    Welcome back, {{ authStore.user?.firstName }}!
                  </v-chip>
                </div>
              </div>
              <div>
                <v-btn 
                  @click="showJobDialog = true"
                  color="white" 
                  class="mr-3 new-job-btn" 
                  size="large"
                  elevation="2"
                >
                  <v-icon start>mdi-plus-circle</v-icon>
                  New Job
                </v-btn>
                <v-btn 
                  @click="handleLogout" 
                  variant="outlined" 
                  color="white"
                  size="large"
                >
                  <v-icon start>mdi-logout</v-icon>
                  Logout
                </v-btn>
              </div>
            </div>
          </v-col>
        </v-row>
      </v-container>
    </div>

    <v-container fluid class="content-section px-4">
      <!-- Quick Actions -->
      <v-row class="mb-6">
        <v-col cols="12">
          <v-card class="quick-actions-card" elevation="4" rounded="xl">
            <v-card-text>
              <div class="d-flex align-center justify-space-between">
                <div>
                  <h3 class="text-h6 mb-1">Quick Actions</h3>
                  <p class="text-body-2 text-medium-emphasis">Manage your job board efficiently</p>
                </div>
                <div class="d-flex gap-2">
                  <v-btn @click="showJobDialog = true" color="primary" variant="tonal">
                    <v-icon start>mdi-briefcase-plus</v-icon>
                    Post Job
                  </v-btn>
                  <v-btn @click="openCandidatesDialog" color="secondary" variant="tonal">
                    <v-icon start>mdi-account-multiple</v-icon>
                    View Candidates
                  </v-btn>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Stats Cards -->
      <v-row class="mb-6">
        <v-col cols="12" sm="6" md="3">
          <v-card class="stats-card" elevation="8" rounded="xl" hover>
            <v-card-text class="text-center pa-6">
              <div class="stats-icon-wrapper mb-3">
                <v-avatar size="60" color="primary" variant="tonal">
                  <v-icon size="30">mdi-briefcase</v-icon>
                </v-avatar>
              </div>
              <div class="text-h3 font-weight-bold mb-1">{{ totalJobs }}</div>
              <div class="text-body-1 text-medium-emphasis">Total Jobs</div>
              <v-progress-linear 
                :model-value="totalJobs > 0 ? 100 : 0" 
                color="primary" 
                height="4" 
                rounded
                class="mt-3"
              ></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="stats-card" elevation="8" rounded="xl" hover>
            <v-card-text class="text-center pa-6">
              <div class="stats-icon-wrapper mb-3">
                <v-avatar size="60" color="success" variant="tonal">
                  <v-icon size="30">mdi-check-circle</v-icon>
                </v-avatar>
              </div>
              <div class="text-h3 font-weight-bold mb-1">{{ activeJobs }}</div>
              <div class="text-body-1 text-medium-emphasis">Active Jobs</div>
              <v-progress-linear 
                :model-value="totalJobs > 0 ? (activeJobs / totalJobs) * 100 : 0" 
                color="success" 
                height="4" 
                rounded
                class="mt-3"
              ></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="stats-card" elevation="8" rounded="xl" hover>
            <v-card-text class="text-center pa-6">
              <div class="stats-icon-wrapper mb-3">
                <v-avatar size="60" color="orange" variant="tonal">
                  <v-icon size="30">mdi-account-group</v-icon>
                </v-avatar>
              </div>
              <div class="text-h3 font-weight-bold mb-1">{{ totalApplications }}</div>
              <div class="text-body-1 text-medium-emphasis">Applications</div>
              <v-progress-linear 
                :model-value="totalApplications > 0 ? Math.min((totalApplications / 10) * 100, 100) : 0" 
                color="orange" 
                height="4" 
                rounded
                class="mt-3"
              ></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="stats-card" elevation="8" rounded="xl" hover>
            <v-card-text class="text-center pa-6">
              <div class="stats-icon-wrapper mb-3">
                <v-avatar size="60" color="info" variant="tonal">
                  <v-icon size="30">mdi-trending-up</v-icon>
                </v-avatar>
              </div>
              <div class="text-h3 font-weight-bold mb-1">{{ avgApplications }}</div>
              <div class="text-body-1 text-medium-emphasis">Avg Applications</div>
              <v-progress-linear 
                :model-value="avgApplications > 0 ? Math.min((avgApplications / 5) * 100, 100) : 0" 
                color="info" 
                height="4" 
                rounded
                class="mt-3"
              ></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Jobs Table -->
      <v-row>
        <v-col cols="12">
          <v-card elevation="8" rounded="xl">
            <v-card-title class="d-flex align-center justify-space-between pa-6">
              <div>
                <h3 class="text-h5 mb-1">Job Postings</h3>
                <p class="text-body-2 text-medium-emphasis">Manage all your job listings</p>
              </div>
              <v-btn @click="showJobDialog = true" color="primary" variant="tonal">
                <v-icon start>mdi-plus</v-icon>
                Add Job
              </v-btn>
            </v-card-title>
            
            <v-card-text>
              <div v-if="loading" class="text-center pa-8">
                <v-progress-circular indeterminate color="primary" size="48"></v-progress-circular>
                <p class="text-h6 mt-4">Loading jobs...</p>
              </div>
              
              <div v-else-if="jobs.length === 0" class="text-center pa-8">
                <v-avatar size="120" color="grey-lighten-3" class="mb-4">
                  <v-icon size="60" color="grey">mdi-briefcase-outline</v-icon>
                </v-avatar>
                <h3 class="text-h6 mb-2">No jobs posted yet</h3>
                <p class="text-body-2 text-medium-emphasis mb-4">Start by creating your first job posting</p>
                <v-btn @click="showJobDialog = true" color="primary" size="large">
                  <v-icon start>mdi-plus-circle</v-icon>
                  Create First Job
                </v-btn>
              </div>
              
              <v-data-table
                v-else
                :headers="headers"
                :items="jobs"
                :loading="loading"
                class="elevation-0"
                hover
              >
                <template v-slot:item.title="{ item }">
                  <div class="font-weight-medium">{{ item.title }}</div>
                </template>
                
                <template v-slot:item.applications="{ item }">
                  <v-chip
                    :color="item.applications?.length > 0 ? 'primary' : 'grey'"
                    size="small"
                    variant="tonal"
                  >
                    <v-icon start size="14">mdi-account-multiple</v-icon>
                    {{ item.applications?.length || 0 }}
                  </v-chip>
                </template>
                
                <template v-slot:item.expirationDate="{ item }">
                  <v-chip
                    :color="new Date(item.expirationDate) > new Date() ? 'success' : 'error'"
                    size="small"
                    variant="tonal"
                  >
                    {{ new Date(item.expirationDate).toLocaleDateString() }}
                  </v-chip>
                </template>
                
                <template v-slot:item.actions="{ item }">
                  <div class="d-flex gap-1">
                    <v-btn icon size="small" variant="text" @click="viewJobDetails(item)">
                      <v-icon>mdi-eye</v-icon>
                      <v-tooltip activator="parent" location="top">View Details</v-tooltip>
                    </v-btn>
                    <v-btn icon size="small" variant="text" @click="editJob(item)">
                      <v-icon>mdi-pencil</v-icon>
                      <v-tooltip activator="parent" location="top">Edit Job</v-tooltip>
                    </v-btn>
                    <v-btn icon size="small" variant="text" color="error" @click="confirmDeleteJob(item)">
                      <v-icon>mdi-delete</v-icon>
                      <v-tooltip activator="parent" location="top">Delete Job</v-tooltip>
                    </v-btn>
                  </div>
                </template>
              </v-data-table>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <!-- New Job Dialog -->
    <v-dialog v-model="showJobDialog" max-width="800px" persistent>
      <v-card rounded="xl">
        <v-card-title class="pa-6">
          <h3 class="text-h5">Create New Job</h3>
          <p class="text-body-2 text-medium-emphasis mt-1">Fill in the details for your new job posting</p>
        </v-card-title>
        
        <v-card-text class="pa-6">
          <v-form ref="jobForm" v-model="jobFormValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="newJob.title"
                  label="Job Title"
                  variant="outlined"
                  :rules="[v => !!v || 'Job title is required']"
                  prepend-inner-icon="mdi-briefcase"
                  required
                ></v-text-field>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="newJob.location"
                  label="Location"
                  variant="outlined"
                  :rules="[v => !!v || 'Location is required']"
                  prepend-inner-icon="mdi-map-marker"
                  required
                ></v-text-field>
              </v-col>
              
              <v-col cols="12">
                <v-textarea
                  v-model="newJob.description"
                  label="Job Description"
                  variant="outlined"
                  :rules="[v => !!v || 'Description is required']"
                  prepend-inner-icon="mdi-text"
                  rows="4"
                  required
                ></v-textarea>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="newJob.expirationDate"
                  label="Expiration Date"
                  variant="outlined"
                  type="date"
                  :rules="[
                    v => !!v || 'Expiration date is required',
                    v => new Date(v) > new Date() || 'Expiration date must be in the future'
                  ]"
                  prepend-inner-icon="mdi-calendar"
                  :min="getTomorrowDate()"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="showJobDialog = false" variant="text">
            Cancel
          </v-btn>
          <v-btn 
            @click="createJob" 
            :loading="creatingJob"
            :disabled="!jobFormValid"
            color="primary"
          >
            <v-icon start>mdi-plus</v-icon>
            Create Job
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- View Candidates Dialog -->
    <v-dialog v-model="showCandidatesDialog" max-width="1200px">
      <v-card rounded="xl">
        <v-card-title class="pa-6">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h3 class="text-h5">Candidates & Applications</h3>
              <p class="text-body-2 text-medium-emphasis mt-1">View all registered candidates and their applications</p>
            </div>
            <v-btn @click="fetchCandidates" color="primary" variant="tonal" :loading="loadingCandidates">
              <v-icon start>mdi-refresh</v-icon>
              Refresh
            </v-btn>
          </div>
        </v-card-title>
        
        <v-card-text class="pa-6">
          <div v-if="loadingCandidates" class="text-center pa-8">
            <v-progress-circular indeterminate color="primary" size="48"></v-progress-circular>
            <p class="text-h6 mt-4">Loading candidates...</p>
          </div>
          
          <div v-else-if="candidates.length === 0" class="text-center pa-8">
            <v-avatar size="120" color="grey-lighten-3" class="mb-4">
              <v-icon size="60" color="grey">mdi-account-group-outline</v-icon>
            </v-avatar>
            <h3 class="text-h6 mb-2">No candidates yet</h3>
            <p class="text-body-2 text-medium-emphasis">Candidates will appear here once they register and apply to jobs</p>
          </div>
          
          <v-data-table
            v-else
            :headers="candidatesHeaders"
            :items="candidates"
            :loading="loadingCandidates"
            class="elevation-0"
            hover
            :items-per-page="10"
          >
            <template v-slot:item.name="{ item }">
              <div class="d-flex align-center">
                <v-avatar color="primary" size="32" class="mr-3">
                  <span class="text-white text-body-2">{{ getInitials(item.firstName, item.lastName) }}</span>
                </v-avatar>
                <div>
                  <div class="font-weight-medium">{{ item.firstName }} {{ item.lastName }}</div>
                  <div class="text-caption text-medium-emphasis">{{ item.email }}</div>
                </div>
              </div>
            </template>
            
            <template v-slot:item.role="{ item }">
              <v-chip
                :color="item.role === 'admin' ? 'primary' : 'secondary'"
                size="small"
                variant="tonal"
              >
                {{ item.role }}
              </v-chip>
            </template>
            
            <template v-slot:item.applications="{ item }">
              <v-chip
                :color="item.applications?.length > 0 ? 'success' : 'grey'"
                size="small"
                variant="tonal"
              >
                {{ item.applications?.length || 0 }} applications
              </v-chip>
            </template>
            
            <template v-slot:item.actions="{ item }">
              <div class="d-flex gap-1">
                <v-btn icon size="small" variant="text" @click="viewCandidateDetails(item)">
                  <v-icon>mdi-eye</v-icon>
                  <v-tooltip activator="parent" location="top">View Details</v-tooltip>
                </v-btn>
              </div>
            </template>
          </v-data-table>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="showCandidatesDialog = false" variant="text">
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Candidate Details Dialog -->
    <v-dialog v-model="showCandidateDetails" max-width="800px">
      <v-card rounded="xl" v-if="selectedCandidate">
        <v-card-title class="pa-6">
          <div class="d-flex align-center">
            <v-avatar color="primary" size="48" class="mr-4">
              <span class="text-white text-h6">{{ getInitials(selectedCandidate.firstName, selectedCandidate.lastName) }}</span>
            </v-avatar>
            <div>
              <h3 class="text-h5">{{ selectedCandidate.firstName }} {{ selectedCandidate.lastName }}</h3>
              <p class="text-body-2 text-medium-emphasis">{{ selectedCandidate.email }}</p>
            </div>
          </div>
        </v-card-title>
        
        <v-card-text class="pa-6">
          <div class="mb-6">
            <h4 class="text-h6 mb-3">Candidate Information</h4>
            <v-row>
              <v-col cols="6">
                <div class="text-body-2 text-medium-emphasis">Email</div>
                <div class="font-weight-medium">{{ selectedCandidate.email }}</div>
              </v-col>
              <v-col cols="6">
                <div class="text-body-2 text-medium-emphasis">Role</div>
                <v-chip :color="selectedCandidate.role === 'admin' ? 'primary' : 'secondary'" size="small" variant="tonal">
                  {{ selectedCandidate.role }}
                </v-chip>
              </v-col>
            </v-row>
          </div>
          
          <div v-if="selectedCandidate.applications && selectedCandidate.applications.length > 0">
            <h4 class="text-h6 mb-3">Applications ({{ selectedCandidate.applications.length }})</h4>
            <v-card v-for="app in selectedCandidate.applications" :key="app.id" class="mb-3" variant="tonal">
              <v-card-text>
                <div class="d-flex justify-space-between align-start mb-2">
                  <div class="font-weight-medium">{{ app.jobTitle || 'Job Application' }}</div>
                  <v-chip size="small" color="info" variant="tonal">
                    {{ new Date(app.appliedAt).toLocaleDateString() }}
                  </v-chip>
                </div>
                <p class="text-body-2 mb-0">{{ app.message || 'No message provided' }}</p>
              </v-card-text>
            </v-card>
          </div>
          
          <div v-else>
            <h4 class="text-h6 mb-3">Applications</h4>
            <v-alert type="info" variant="tonal">
              This candidate hasn't submitted any applications yet.
            </v-alert>
          </div>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="showCandidateDetails = false" variant="text">
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Job Details Dialog -->
    <v-dialog v-model="showJobDetailsDialog" max-width="800px">
      <v-card rounded="xl" v-if="selectedJob">
        <v-card-title class="pa-6">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h3 class="text-h5">{{ selectedJob.title }}</h3>
              <p class="text-body-2 text-medium-emphasis mt-1">Job Details</p>
            </div>
            <v-chip
              :color="new Date(selectedJob.expirationDate) > new Date() ? 'success' : 'error'"
              variant="tonal"
            >
              {{ new Date(selectedJob.expirationDate) > new Date() ? 'Active' : 'Expired' }}
            </v-chip>
          </div>
        </v-card-title>
        
        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="12" md="6">
              <div class="mb-4">
                <div class="text-body-2 text-medium-emphasis mb-1">Location</div>
                <div class="font-weight-medium">
                  <v-icon size="16" class="mr-1">mdi-map-marker</v-icon>
                  {{ selectedJob.location }}
                </div>
              </div>
            </v-col>
            <v-col cols="12" md="6">
              <div class="mb-4">
                <div class="text-body-2 text-medium-emphasis mb-1">Expiration Date</div>
                <div class="font-weight-medium">
                  <v-icon size="16" class="mr-1">mdi-calendar</v-icon>
                  {{ new Date(selectedJob.expirationDate).toLocaleDateString() }}
                </div>
              </div>
            </v-col>
            <v-col cols="12" md="6">
              <div class="mb-4">
                <div class="text-body-2 text-medium-emphasis mb-1">Created</div>
                <div class="font-weight-medium">
                  <v-icon size="16" class="mr-1">mdi-clock-outline</v-icon>
                  {{ new Date(selectedJob.createdAt).toLocaleDateString() }}
                </div>
              </div>
            </v-col>
            <v-col cols="12" md="6">
              <div class="mb-4">
                <div class="text-body-2 text-medium-emphasis mb-1">Applications</div>
                <div class="font-weight-medium">
                  <v-icon size="16" class="mr-1">mdi-account-multiple</v-icon>
                  {{ selectedJob.applications?.length || 0 }} applications
                </div>
              </div>
            </v-col>
            <v-col cols="12">
              <div class="mb-4">
                <div class="text-body-2 text-medium-emphasis mb-2">Description</div>
                <div class="font-weight-medium">{{ selectedJob.description }}</div>
              </div>
            </v-col>
          </v-row>
          
          <div v-if="selectedJob.applications && selectedJob.applications.length > 0">
            <v-divider class="my-4"></v-divider>
            <h4 class="text-h6 mb-3">Applications ({{ selectedJob.applications.length }})</h4>
            <v-card v-for="app in selectedJob.applications" :key="app.id" class="mb-3" variant="tonal">
              <v-card-text>
                <div class="d-flex justify-space-between align-start mb-2">
                  <div>
                    <div class="font-weight-medium">{{ app.candidate?.firstName }} {{ app.candidate?.lastName }}</div>
                    <div class="text-body-2 text-medium-emphasis">{{ app.candidate?.email }}</div>
                  </div>
                  <v-chip size="small" color="info" variant="tonal">
                    {{ new Date(app.appliedAt).toLocaleDateString() }}
                  </v-chip>
                </div>
                <p class="text-body-2 mb-0">{{ app.message || 'No message provided' }}</p>
              </v-card-text>
            </v-card>
          </div>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="showJobDetailsDialog = false" variant="text">
            Close
          </v-btn>
          <v-btn @click="editJobFromDetails" color="primary" variant="tonal">
            <v-icon start>mdi-pencil</v-icon>
            Edit Job
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Edit Job Dialog -->
    <v-dialog v-model="showEditJobDialog" max-width="800px" persistent>
      <v-card rounded="xl">
        <v-card-title class="pa-6">
          <h3 class="text-h5">Edit Job</h3>
          <p class="text-body-2 text-medium-emphasis mt-1">Update job posting details</p>
        </v-card-title>
        
        <v-card-text class="pa-6">
          <v-form ref="editJobFormRef" v-model="jobFormValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="editJobForm.title"
                  label="Job Title"
                  variant="outlined"
                  :rules="[v => !!v || 'Job title is required']"
                  prepend-inner-icon="mdi-briefcase"
                  required
                ></v-text-field>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="editJobForm.location"
                  label="Location"
                  variant="outlined"
                  :rules="[v => !!v || 'Location is required']"
                  prepend-inner-icon="mdi-map-marker"
                  required
                ></v-text-field>
              </v-col>
              
              <v-col cols="12">
                <v-textarea
                  v-model="editJobForm.description"
                  label="Job Description"
                  variant="outlined"
                  :rules="[v => !!v || 'Description is required']"
                  prepend-inner-icon="mdi-text"
                  rows="4"
                  required
                ></v-textarea>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="editJobForm.expirationDate"
                  label="Expiration Date"
                  variant="outlined"
                  type="date"
                  :rules="[
                    v => !!v || 'Expiration date is required',
                    v => new Date(v) > new Date() || 'Expiration date must be in the future'
                  ]"
                  prepend-inner-icon="mdi-calendar"
                  :min="getTomorrowDate()"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="cancelEditJob" variant="text">
            Cancel
          </v-btn>
          <v-btn 
            @click="updateJob" 
            :loading="editingJob"
            :disabled="!jobFormValid"
            color="primary"
          >
            <v-icon start>mdi-content-save</v-icon>
            Update Job
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="showDeleteConfirm" max-width="500px">
      <v-card rounded="xl">
        <v-card-title class="pa-6">
          <h3 class="text-h5">Delete Job</h3>
          <p class="text-body-2 text-medium-emphasis mt-1">This action cannot be undone</p>
        </v-card-title>
        
        <v-card-text class="pa-6" v-if="selectedJob">
          <v-alert type="warning" variant="tonal" class="mb-4">
            <strong>Are you sure you want to delete this job?</strong>
          </v-alert>
          
          <div class="mb-3">
            <div class="font-weight-medium">{{ selectedJob.title }}</div>
            <div class="text-body-2 text-medium-emphasis">{{ selectedJob.location }}</div>
          </div>
          
          <div v-if="selectedJob.applications?.length > 0" class="mb-3">
            <v-alert type="error" variant="tonal">
              This job has {{ selectedJob.applications.length }} application(s). Deleting it will also remove all applications.
            </v-alert>
          </div>
        </v-card-text>
        
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn @click="showDeleteConfirm = false" variant="text">
            Cancel
          </v-btn>
          <v-btn 
            @click="deleteJob" 
            :loading="deletingJob"
            color="error"
          >
            <v-icon start>mdi-delete</v-icon>
            Delete Job
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success/Error Snackbar -->
    <v-snackbar
      v-model="showSuccessMessage"
      :color="successMessage.includes('Failed') ? 'error' : 'success'"
      timeout="5000"
      location="top"
    >
      {{ successMessage }}
      <template v-slot:actions>
        <v-btn variant="text" @click="showSuccessMessage = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

// Job data
const jobs = ref([])
const loading = ref(false)

// Job dialog
const showJobDialog = ref(false)
const jobFormValid = ref(false)
const creatingJob = ref(false)
const newJob = ref({
  title: '',
  description: '',
  location: '',
  expirationDate: ''
})

// Success message
const showSuccessMessage = ref(false)
const successMessage = ref('')

// Candidates dialog
const showCandidatesDialog = ref(false)
const loadingCandidates = ref(false)
const candidates = ref([])
const showCandidateDetails = ref(false)
const selectedCandidate = ref(null)

// Job details and edit dialog
const showJobDetailsDialog = ref(false)
const showEditJobDialog = ref(false)
const showDeleteConfirm = ref(false)
const selectedJob = ref(null)
const editingJob = ref(false)
const deletingJob = ref(false)
const editJobForm = ref({
  title: '',
  description: '',
  location: '',
  expirationDate: ''
})

const headers = [
  { title: 'Title', key: 'title' },
  { title: 'Location', key: 'location' },
  { title: 'Applications', key: 'applications' },
  { title: 'Expiration', key: 'expirationDate' },
  { title: 'Actions', key: 'actions', sortable: false }
]

const candidatesHeaders = [
  { title: 'Name', key: 'name' },
  { title: 'Role', key: 'role' }, 
  { title: 'Applications', key: 'applications' },
  { title: 'Actions', key: 'actions', sortable: false }
]

const totalJobs = computed(() => jobs.value.length)
const activeJobs = computed(() => jobs.value.filter(job => new Date(job.expirationDate) > new Date()).length)
const totalApplications = computed(() => jobs.value.reduce((total, job) => total + (job.applications?.length || 0), 0))
const avgApplications = computed(() => Math.round(totalApplications.value / (totalJobs.value || 1)))

async function fetchJobs() {
  loading.value = true
  
  try {
    const response = await fetch('http://localhost:5000/api/jobs/with-applications', {
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
  } finally {
    loading.value = false
  }
}

async function createJob() {
  if (!jobFormValid.value) return
  
  creatingJob.value = true
  
  try {
    const response = await fetch('http://localhost:5000/api/jobs', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${authStore.token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(newJob.value),
    })

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Unknown error' }))
      throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`)
    }

    const createdJob = await response.json()
    jobs.value.push(createdJob)
    
    newJob.value = {
      title: '',
      description: '',
      location: '',
      expirationDate: ''
    }
    showJobDialog.value = false
    
    successMessage.value = `Job "${createdJob.title}" created successfully!`
    showSuccessMessage.value = true
    
  } catch (error) {
    console.error('Error creating job:', error)
    successMessage.value = `Failed to create job: ${error.message}`
    showSuccessMessage.value = true
  } finally {
    creatingJob.value = false
  }
}

async function fetchCandidates() {
  loadingCandidates.value = true
  
  try {
    const response = await fetch('http://localhost:5000/api/jobs/with-applications', {
      headers: {
        'Authorization': `Bearer ${authStore.token}`,
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) throw new Error('Failed to fetch jobs with applications')

    const jobsWithApplications = await response.json()
    
    // get unique candidates from applications 
    const candidatesMap = new Map()
    
    jobsWithApplications.forEach(job => {
      if (job.applications && job.applications.length > 0) {
        job.applications.forEach(application => {
          if (application.candidate) {
            const candidateId = application.candidate.id
            
            if (!candidatesMap.has(candidateId)) {
              candidatesMap.set(candidateId, {
                ...application.candidate,
                applications: []
              })
            }
            
            candidatesMap.get(candidateId).applications.push({
              id: application.id,
              jobId: application.jobId,
              jobTitle: job.title,
              message: application.message,
              appliedAt: application.appliedAt
            })
          }
        })
      }
    })
    
    candidates.value = Array.from(candidatesMap.values())
  } catch (error) {
    console.error('Error fetching candidates:', error)
    candidates.value = []
  } finally {
    loadingCandidates.value = false
  }
}

function getTomorrowDate() {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow.toISOString().split('T')[0]
}

function openCandidatesDialog() {
  showCandidatesDialog.value = true
  fetchCandidates()
}

function getInitials(firstName, lastName) {
  return `${firstName?.charAt(0) || ''}${lastName?.charAt(0) || ''}`.toUpperCase()
}

function viewCandidateDetails(candidate) {
  selectedCandidate.value = candidate
  showCandidateDetails.value = true
}

// Job management functions
function viewJobDetails(job) {
  selectedJob.value = job
  showJobDetailsDialog.value = true
}

function editJob(job) {
  selectedJob.value = job
  editJobForm.value = {
    title: job.title,
    description: job.description,
    location: job.location,
    expirationDate: job.expirationDate.split('T')[0] // Convert to YYYY-MM-DD format
  }
  showEditJobDialog.value = true
}

function editJobFromDetails() {
  showJobDetailsDialog.value = false
  editJob(selectedJob.value)
}

function cancelEditJob() {
  showEditJobDialog.value = false
  selectedJob.value = null
  editJobForm.value = {
    title: '',
    description: '',
    location: '',
    expirationDate: ''
  }
}

async function updateJob() {
  if (!jobFormValid.value || !selectedJob.value) return
  
  editingJob.value = true
  
  try {
    const response = await fetch(`http://localhost:5000/api/jobs/${selectedJob.value.id}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${authStore.token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(editJobForm.value),
    })

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Unknown error' }))
      throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`)
    }

    const updatedJob = await response.json()
    
    const jobIndex = jobs.value.findIndex(j => j.id === selectedJob.value.id)
    if (jobIndex !== -1) {
      jobs.value[jobIndex] = updatedJob
    }
    
    cancelEditJob()
    
    successMessage.value = `Job "${updatedJob.title}" updated successfully!`
    showSuccessMessage.value = true
    
  } catch (error) {
    console.error('Error updating job:', error)
    successMessage.value = `Failed to update job: ${error.message}`
    showSuccessMessage.value = true
  } finally {
    editingJob.value = false
  }
}

function confirmDeleteJob(job) {
  selectedJob.value = job
  showDeleteConfirm.value = true
}

async function deleteJob() {
  if (!selectedJob.value) return
  
  deletingJob.value = true
  
  try {
    const response = await fetch(`http://localhost:5000/api/jobs/${selectedJob.value.id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${authStore.token}`,
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Unknown error' }))
      throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`)
    }

    const jobTitle = selectedJob.value.title
    jobs.value = jobs.value.filter(j => j.id !== selectedJob.value.id)
    
    showDeleteConfirm.value = false
    selectedJob.value = null
    
    successMessage.value = `Job "${jobTitle}" deleted successfully!`
    showSuccessMessage.value = true
    
  } catch (error) {
    console.error('Error deleting job:', error)
    successMessage.value = `Failed to delete job: ${error.message}`
    showSuccessMessage.value = true
  } finally {
    deletingJob.value = false
  }
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  console.log('🎯 Admin Dashboard mounted')
  fetchJobs()
})
</script>

<style scoped>
.admin-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

.header-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem 0;
  margin-bottom: 2rem;
  position: relative;
  overflow: hidden;
}

.header-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.1'%3E%3Ccircle cx='30' cy='30' r='4'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E") repeat;
}

.header-content {
  position: relative;
  z-index: 2;
}

.content-section {
  margin-top: -40px;
  position: relative;
  z-index: 2;
}

.new-job-btn {
  background: rgba(255, 255, 255, 0.95) !important;
  color: #667eea !important;
  backdrop-filter: blur(10px);
  font-weight: 600;
  text-transform: none;
}

.quick-actions-card {
  background: linear-gradient(145deg, #ffffff 0%, #f8f9ff 100%);
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.stats-card {
  background: linear-gradient(145deg, #ffffff 0%, #f8f9ff 100%);
  border: 1px solid rgba(102, 126, 234, 0.08);
  transition: all 0.3s ease;
}

.stats-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.15) !important;
}

.stats-icon-wrapper {
  position: relative;
}

.stats-icon-wrapper::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(102, 126, 234, 0.1) 0%, transparent 70%);
  z-index: 0;
}

@media (max-width: 960px) {
  .header-section {
    padding: 1.5rem 0;
  }
  
  .content-section {
    margin-top: -20px;
  }
  
  .d-flex.justify-space-between {
    flex-direction: column;
    gap: 1rem;
  }
  
  .d-flex.gap-2 {
    justify-content: center;
    flex-wrap: wrap;
  }
}

@media (max-width: 600px) {
  .header-section .d-flex {
    flex-direction: column;
    text-align: center;
    gap: 1.5rem;
  }
}
</style>