<template>
  <v-container class="fill-height">
    <v-row justify="center" align="center">
      <v-col cols="12" sm="8" md="6">
        <v-card class="elevation-12">
          <v-toolbar color="primary" dark flat>
            <v-toolbar-title>Register</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form @submit.prevent="handleRegister">
              <v-text-field
                v-model="form.firstName"
                label="First Name"
                name="firstName"
                prepend-icon="mdi-account"
                :rules="nameRules"
                required
              />
              <v-text-field
                v-model="form.lastName"
                label="Last Name"
                name="lastName"
                prepend-icon="mdi-account"
                :rules="nameRules"
                required
              />
              <v-text-field
                v-model="form.email"
                label="Email"
                name="email"
                prepend-icon="mdi-email"
                type="email"
                :rules="emailRules"
                required
              />
              <v-text-field
                v-model="form.password"
                label="Password"
                name="password"
                prepend-icon="mdi-lock"
                type="password"
                :rules="passwordRules"
                required
              />
              <v-text-field
                v-model="form.confirmPassword"
                label="Confirm Password"
                name="confirmPassword"
                prepend-icon="mdi-lock"
                type="password"
                :rules="confirmPasswordRules"
                required
              />
              <v-select
                v-model="form.role"
                label="Role"
                :items="roleOptions"
                item-title="text"
                item-value="value"
                prepend-icon="mdi-account-group"
                :rules="roleRules"
                required
              />
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn 
              color="primary" 
              :loading="loading" 
              @click="handleRegister"
            >
              Register
            </v-btn>
          </v-card-actions>
          <v-divider />
          <v-card-actions>
            <v-spacer />
            <v-btn text @click="$router.push('/login')">
              Already have an account? Login
            </v-btn>
            <v-spacer />
          </v-card-actions>
        </v-card>
        <v-alert
          v-if="error"
          type="error"
          class="mt-4"
          dismissible
          @click:close="error = ''"
        >
          {{ error }}
        </v-alert>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  role: '' as 'admin' | 'candidate' | ''
})

const loading = ref(false)
const error = ref('')

const roleOptions = [
  { text: 'Candidate', value: 'candidate' },
  { text: 'Admin', value: 'admin' }
]

const nameRules = [
  (v: string) => !!v || 'Name is required'
]

const emailRules = [
  (v: string) => !!v || 'Email is required',
  (v: string) => /.+@.+\..+/.test(v) || 'Email must be valid'
]

const passwordRules = [
  (v: string) => !!v || 'Password is required',
  (v: string) => v.length >= 6 || 'Password must be at least 6 characters'
]

const confirmPasswordRules = [
  (v: string) => !!v || 'Password confirmation is required',
  (v: string) => v === form.value.password || 'Passwords must match'
]

const roleRules = [
  (v: string) => !!v || 'Role is required'
]

async function handleRegister() {
  if (!form.value.firstName || !form.value.lastName || !form.value.email || 
      !form.value.password || !form.value.confirmPassword || !form.value.role) {
    error.value = 'Please fill in all fields'
    return
  }

  if (form.value.password !== form.value.confirmPassword) {
    error.value = 'Passwords do not match'
    return
  }

  loading.value = true
  error.value = ''

  try {
    await authStore.register(
      form.value.email,
      form.value.password,
      form.value.firstName,
      form.value.lastName,
      form.value.role
    )
    
    if (authStore.isAdmin) {
      router.push('/admin')
    } else if (authStore.isCandidate) {
      router.push('/jobs')
    } else {
      router.push('/')
    }
  } catch (err) {
    error.value = 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>