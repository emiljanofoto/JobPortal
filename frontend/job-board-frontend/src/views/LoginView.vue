<template>
  <div class="login-container">
    <div class="login-background"></div>
    <v-container class="fill-height">
      <v-row justify="center" align="center">
        <v-col cols="12" sm="8" md="6" lg="5" xl="4">
          <div class="login-wrapper">
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

            <!-- Login Card -->
            <v-card class="login-card" elevation="24" rounded="xl">
              <div class="login-header">
                <div class="login-icon mb-4">
                  <v-avatar size="80" color="primary" variant="tonal">
                    <v-icon size="40">mdi-account-circle</v-icon>
                  </v-avatar>
                </div>
                <h1 class="login-title">Welcome Back!</h1>
                <p class="login-subtitle">Sign in to your account</p>
              </div>

              <v-card-text class="login-form-container">
                <v-form @submit.prevent="handleLogin" class="login-form">
                  <v-text-field
                    v-model="form.email"
                    label="Email Address"
                    type="email"
                    prepend-inner-icon="mdi-email-outline"
                    variant="outlined"
                    rounded="lg"
                    :rules="emailRules"
                    :error-messages="error && error.includes('credentials') ? ['Invalid email or password'] : []"
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
                    :error-messages="error && error.includes('credentials') ? ['Invalid email or password'] : []"
                    class="mb-6"
                    required
                  />

                  <v-btn
                    type="submit"
                    size="x-large"
                    color="primary"
                    :loading="loading"
                    rounded="lg"
                    block
                    class="login-btn mb-4"
                  >
                    <v-icon start>mdi-login</v-icon>
                    Sign In
                  </v-btn>
                </v-form>
              </v-card-text>

              <v-divider class="mx-6"></v-divider>

              <v-card-actions class="px-6 pb-6">
                <div class="text-center w-100">
                  <p class="text-body-2 mb-4">Don't have an account?</p>
                  <v-btn
                    variant="outlined"
                    size="large"
                    rounded="lg"
                    block
                    @click="$router.push('/register')"
                  >
                    <v-icon start>mdi-account-plus</v-icon>
                    Create Account
                  </v-btn>
                </div>
              </v-card-actions>
            </v-card>

            <!-- Demo Credentials -->
            <v-card class="demo-card mt-6" variant="tonal" rounded="xl">
              <v-card-text class="text-center">
                <p class="text-subtitle-2 mb-2">Demo Credentials</p>
                <div class="demo-credentials">
                  <p><strong>Admin:</strong> admin@jobboard.com / admin123</p>
                  <p><strong>Candidate:</strong> Register as candidate</p>
                </div>
              </v-card-text>
            </v-card>
          </div>
        </v-col>
      </v-row>
    </v-container>

    <!-- Error Snackbar -->
    <v-snackbar
      v-model="showError"
      color="error"
      timeout="5000"
      location="top"
    >
      {{ error }}
      <template v-slot:actions>
        <v-btn variant="text" @click="showError = false">
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
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref('')
const showError = ref(false)

const emailRules = [
  (v: string) => !!v || 'Email is required',
  (v: string) => /.+@.+\..+/.test(v) || 'Email must be valid'
]

const passwordRules = [
  (v: string) => !!v || 'Password is required'
]

async function handleLogin() {
  if (!form.value.email || !form.value.password) {
    error.value = 'Please fill in all fields'
    return
  }

  console.log('🔐 Starting login process...')
  loading.value = true
  error.value = ''

  try {
    console.log('🔐 Calling authStore.login...')
    await authStore.login(form.value.email, form.value.password)
    
    console.log('✅ Login successful!')
    console.log('👤 User after login:', authStore.user)
    console.log('🔑 Is Admin:', authStore.isAdmin)
    console.log('🔑 Is Candidate:', authStore.isCandidate)
    
    if (authStore.isAdmin) {
      console.log('🎯 Redirecting to admin...')
      router.push('/admin')
    } else if (authStore.isCandidate) {
      console.log('🎯 Redirecting to jobs...')
      router.push('/jobs')
    } else {
      console.log('🎯 Redirecting to home...')
      router.push('/')
    }
  } catch (err) {
    console.error('💥 Login failed:', err)
    error.value = 'Invalid credentials'
    showError.value = true
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  position: relative;
  display: flex;
  align-items: center;
}

.login-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  opacity: 0.1;
}

.login-wrapper {
  position: relative;
  z-index: 2;
}

.back-btn {
  color: #666;
  text-transform: none;
}

.login-card {
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.98) !important;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.login-header {
  text-align: center;
  padding: 3rem 2rem 1rem;
}

.login-title {
  font-size: 2rem;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.login-subtitle {
  color: #6c757d;
  font-size: 1.1rem;
}

.login-form-container {
  padding: 1rem 2rem;
}

.login-btn {
  font-weight: 600;
  font-size: 1.1rem;
}

.demo-card {
  background: rgba(33, 150, 243, 0.1) !important;
}

.demo-credentials p {
  margin-bottom: 0.25rem;
  font-size: 0.9rem;
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
  .login-header {
    padding: 2rem 1rem 1rem;
  }
  
  .login-form-container {
    padding: 1rem;
  }
  
  .login-title {
    font-size: 1.75rem;
  }
}
</style>