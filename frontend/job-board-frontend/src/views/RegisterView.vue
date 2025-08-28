<template>
  <div class="register-container">
    <div class="register-background"></div>
    <v-container class="fill-height">
      <v-row justify="center" align="center">
        <v-col cols="12" sm="10" md="8" lg="7" xl="6">
          <div class="register-wrapper">
            <!-- Back to Home Button -->
            <div class="mb-6">
              <v-btn
                variant="text"
                prepend-icon="mdi-arrow-left"
                @click="$router.push('/')"
                class="back-btn"
              >
                Back to Home
              </v-btn>
            </div>

            <!-- Register Card -->
            <v-card class="register-card" elevation="24" rounded="xl">
              <div class="register-header">
                <div class="register-icon mb-4">
                  <v-avatar size="80" color="success" variant="tonal">
                    <v-icon size="40">mdi-account-plus</v-icon>
                  </v-avatar>
                </div>
                <h1 class="register-title">Create Account</h1>
                <p class="register-subtitle">Join our platform and start your journey</p>
              </div>

              <v-card-text class="register-form-container">
                <v-form @submit.prevent="handleRegister" class="register-form">
                  <v-row>
                    <v-col cols="12" sm="6">
                      <v-text-field
                        v-model="form.firstName"
                        label="First Name"
                        prepend-inner-icon="mdi-account-outline"
                        variant="outlined"
                        rounded="lg"
                        :rules="nameRules"
                        class="mb-4"
                        required
                      />
                    </v-col>
                    <v-col cols="12" sm="6">
                      <v-text-field
                        v-model="form.lastName"
                        label="Last Name"
                        prepend-inner-icon="mdi-account-outline"
                        variant="outlined"
                        rounded="lg"
                        :rules="nameRules"
                        class="mb-4"
                        required
                      />
                    </v-col>
                  </v-row>

                  <v-text-field
                    v-model="form.email"
                    label="Email Address"
                    type="email"
                    prepend-inner-icon="mdi-email-outline"
                    variant="outlined"
                    rounded="lg"
                    :rules="emailRules"
                    class="mb-4"
                    required
                  />

                  <v-text-field
                    v-model="form.password"
                    label="Password"
                    type="password"
                    prepend-inner-icon="mdi-lock-outline"
                    variant="outlined"
                    rounded="lg"
                    :rules="passwordRules"
                    class="mb-4"
                    required
                  />

                  <v-text-field
                    v-model="form.confirmPassword"
                    label="Confirm Password"
                    type="password"
                    prepend-inner-icon="mdi-lock-check-outline"
                    variant="outlined"
                    rounded="lg"
                    :rules="confirmPasswordRules"
                    class="mb-4"
                    required
                  />

                  <v-select
                    v-model="form.role"
                    label="I am a..."
                    :items="roleOptions"
                    item-title="text"
                    item-value="value"
                    prepend-inner-icon="mdi-account-group-outline"
                    variant="outlined"
                    rounded="lg"
                    :rules="roleRules"
                    class="mb-6"
                    required
                  />

                  <v-btn
                    type="submit"
                    size="x-large"
                    color="success"
                    :loading="loading"
                    rounded="lg"
                    block
                    class="register-btn mb-4"
                  >
                    <v-icon start>mdi-account-plus</v-icon>
                    Create Account
                  </v-btn>
                </v-form>
              </v-card-text>

              <v-divider class="mx-6"></v-divider>

              <v-card-actions class="px-6 pb-6">
                <div class="text-center w-100">
                  <p class="text-body-2 mb-4">Already have an account?</p>
                  <v-btn
                    variant="outlined"
                    size="large"
                    rounded="lg"
                    block
                    @click="$router.push('/login')"
                  >
                    <v-icon start>mdi-login</v-icon>
                    Sign In
                  </v-btn>
                </div>
              </v-card-actions>
            </v-card>
          </div>
        </v-col>
      </v-row>
    </v-container>

    <!-- Error/Success Snackbar -->
    <v-snackbar
      v-model="showMessage"
      :color="messageColor"
      timeout="5000"
      location="top"
    >
      {{ message }}
      <template v-slot:actions>
        <v-btn variant="text" @click="showMessage = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
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
const message = ref('')
const messageColor = ref('error')
const showMessage = ref(false)

const roleOptions = [
  { text: 'Candidate - Looking for jobs', value: 'candidate' },
  { text: 'Admin - Hiring manager', value: 'admin' }
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
    showError('Please fill in all fields')
    return
  }

  if (form.value.password !== form.value.confirmPassword) {
    showError('Passwords do not match')
    return
  }

  loading.value = true

  try {
    await authStore.register(
      form.value.email,
      form.value.password,
      form.value.firstName,
      form.value.lastName,
      form.value.role
    )
    
    showSuccess('Account created successfully! Welcome aboard!')
    
    // Small delay to show success message
    setTimeout(() => {
      if (authStore.isAdmin) {
        router.push('/admin')
      } else if (authStore.isCandidate) {
        router.push('/jobs')
      } else {
        router.push('/')
      }
    }, 1500)
  } catch (err) {
    showError('Registration failed. Please try again.')
  } finally {
    loading.value = false
  }
}

function showError(msg: string) {
  message.value = msg
  messageColor.value = 'error'
  showMessage.value = true
}

function showSuccess(msg: string) {
  message.value = msg
  messageColor.value = 'success'
  showMessage.value = true
}
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  position: relative;
  display: flex;
  align-items: center;
  padding: 2rem 0;
}

.register-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, #4caf50 0%, #2e7d32 100%);
  opacity: 0.1;
}

.register-wrapper {
  position: relative;
  z-index: 2;
}

.back-btn {
  color: #666;
  text-transform: none;
}

.register-card {
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.98) !important;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.register-header {
  text-align: center;
  padding: 3rem 2rem 1rem;
}

.register-title {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.register-subtitle {
  color: #6c757d;
  font-size: 1.1rem;
}

.register-form-container {
  padding: 1rem 2rem;
}

.register-btn {
  font-weight: 600;
  font-size: 1.1rem;
}

:deep(.v-field--outlined) {
  --v-field-border-opacity: 0.3;
}

:deep(.v-field--outlined:hover) {
  --v-field-border-opacity: 0.6;
}

:deep(.v-field--focused) {
  --v-field-border-opacity: 1;
}

@media (max-width: 600px) {
  .register-container {
    padding: 1rem 0;
  }
  
  .register-header {
    padding: 2rem 1rem 1rem;
  }
  
  .register-form-container {
    padding: 1rem;
  }
  
  .register-title {
    font-size: 1.75rem;
  }
}
</style>