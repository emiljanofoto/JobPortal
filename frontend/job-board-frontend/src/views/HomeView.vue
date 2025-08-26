<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

function navigateBasedOnRole() {
  if (authStore.isAdmin) {
    router.push('/admin')
  } else if (authStore.isCandidate) {
    router.push('/jobs')
  }
}

if (authStore.isAuthenticated) {
  navigateBasedOnRole()
}
</script>

<template>
  <v-container class="fill-height">
    <v-row justify="center" align="center">
      <v-col cols="12" sm="8" md="6" class="text-center">
        <v-card class="elevation-12">
          <v-card-text class="pa-8">
            <h1 class="display-1 mb-6">Job Board Platform</h1>
            <p class="subtitle-1 mb-8">
              Welcome to our job board platform. Find your next opportunity or post jobs for talented candidates.
            </p>
            
            <div class="d-flex flex-column flex-sm-row justify-center ga-4">
              <v-btn
                size="large"
                color="primary"
                @click="$router.push('/login')"
              >
                <v-icon left>mdi-login</v-icon>
                Login
              </v-btn>
              
              <v-btn
                size="large"
                variant="outlined"
                color="primary"
                @click="$router.push('/register')"
              >
                <v-icon left>mdi-account-plus</v-icon>
                Register
              </v-btn>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
