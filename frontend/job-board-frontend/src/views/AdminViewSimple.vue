<template>
  <div class="admin-container">
    <v-container class="pa-4">
      <div class="d-flex justify-space-between align-center mb-6">
        <div>
          <h1 class="text-h3 mb-2">Admin Dashboard</h1>
          <p class="text-subtitle-1">Manage job postings and applications</p>
        </div>
        <div>
          <v-btn color="primary" class="mr-2">
            <v-icon start>mdi-plus</v-icon>
            New Job
          </v-btn>
          <v-btn color="error" @click="handleLogout">
            <v-icon start>mdi-logout</v-icon>
            Logout
          </v-btn>
        </div>
      </div>

      <!-- Stats Cards -->
      <v-row class="mb-6">
        <v-col cols="12" sm="6" md="3">
          <v-card>
            <v-card-text class="text-center">
              <v-icon size="48" color="primary" class="mb-2">mdi-briefcase</v-icon>
              <div class="text-h4">{{ jobs.length }}</div>
              <div class="text-caption">Total Jobs</div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <v-card>
            <v-card-text class="text-center">
              <v-icon size="48" color="success" class="mb-2">mdi-check</v-icon>
              <div class="text-h4">{{ activeJobsCount }}</div>
              <div class="text-caption">Active Jobs</div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <v-card>
            <v-card-text class="text-center">
              <v-icon size="48" color="orange" class="mb-2">mdi-account-group</v-icon>
              <div class="text-h4">{{ totalApplicationsCount }}</div>
              <div class="text-caption">Applications</div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <v-card>
            <v-card-text class="text-center">
              <v-icon size="48" color="info" class="mb-2">mdi-trending-up</v-icon>
              <div class="text-h4">{{ Math.round(totalApplicationsCount / (jobs.length || 1)) }}</div>
              <div class="text-caption">Avg Applications</div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Jobs Table -->
      <v-card>
        <v-card-title>Job Postings</v-card-title>
        <v-data-table
          :headers="headers"
          :items="jobs"
          :loading="loading"
          class="elevation-1"
        >
          <template v-slot:item.actions="{ item }">
            <v-btn icon size="small" class="mr-1">
              <v-icon>mdi-eye</v-icon>
            </v-btn>
            <v-btn icon size="small" class="mr-1">
              <v-icon>mdi-pencil</v-icon>
            </v-btn>
            <v-btn icon size="small" color="error">
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </template>
        </v-data-table>
      </v-card>
    </v-container>
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
  { title: 'Title', key: 'title' },
  { title: 'Location', key: 'location' },
  { title: 'Expiration', key: 'expirationDate' },
  { title: 'Actions', key: 'actions', sortable: false }
]

const activeJobsCount = computed(() => {
  return jobs.value.filter(job => new Date(job.expirationDate) > new Date()).length
})

const totalApplicationsCount = computed(() => {
  return jobs.value.reduce((total, job) => total + (job.applications?.length || 0), 0)
})

onMounted(async () => {
  console.log('🎯 AdminViewSimple mounted')
  console.log('📊 Jobs Store:', jobsStore)
  console.log('💼 Current jobs:', jobs.value)
  
  try {
    console.log('📥 Fetching jobs...')
    await jobsStore.fetchJobsWithApplications()
    console.log('✅ Jobs fetched successfully:', jobs.value)
  } catch (error) {
    console.error('❌ Error fetching jobs:', error)
  }
})

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.admin-container {
  min-height: 100vh;
  background: #f5f5f5;
}
</style>