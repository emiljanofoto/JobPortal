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
  <div class="home-container">
    <!-- Hero Section -->
    <section class="hero-section">
      <v-container class="fill-height">
        <v-row justify="center" align="center" class="fill-height">
          <v-col cols="12" lg="10" xl="9">
            <div class="text-center hero-content">
              <div class="hero-badge mb-6">
                <v-chip color="primary" variant="outlined" size="large">
                  <v-icon start>mdi-briefcase</v-icon>
                  Job Board Platform
                </v-chip>
              </div>
              
              <h1 class="hero-title mb-6">
                Find Your Dream Job or 
                <span class="text-primary">Hire Top Talent</span>
              </h1>
              
              <p class="hero-subtitle mb-8">
                Connect talented candidates with amazing opportunities. 
                Our platform makes job searching and hiring effortless and effective.
              </p>
              
              <div class="hero-actions mb-12">
                <v-btn
                  size="x-large"
                  color="primary"
                  class="mr-4 mb-4"
                  rounded="lg"
                  @click="$router.push('/register')"
                >
                  <v-icon start>mdi-rocket-launch</v-icon>
                  Get Started
                </v-btn>
                
                <v-btn
                  size="x-large"
                  variant="outlined"
                  color="primary"
                  class="mb-4"
                  rounded="lg"
                  @click="$router.push('/login')"
                >
                  <v-icon start>mdi-login</v-icon>
                  Sign In
                </v-btn>
              </div>
            </div>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- Features Section -->
    <section class="features-section">
      <v-container class="px-4">
        <v-row justify="center">
          <v-col cols="12" class="text-center mb-8">
            <h2 class="section-title">Why Choose Our Platform?</h2>
            <p class="section-subtitle">
              Powerful features designed to make hiring and job searching seamless
            </p>
          </v-col>
        </v-row>
        
        <v-row>
          <v-col cols="12" md="4" class="mb-6">
            <v-card class="feature-card h-100" elevation="8" rounded="xl">
              <v-card-text class="pa-8 text-center">
                <div class="feature-icon mb-4">
                  <v-avatar size="80" color="primary" variant="tonal">
                    <v-icon size="40">mdi-account-group</v-icon>
                  </v-avatar>
                </div>
                <h3 class="feature-title mb-3">For Candidates</h3>
                <p class="feature-description">
                  Browse amazing job opportunities, apply with ease, and track your applications
                </p>
              </v-card-text>
            </v-card>
          </v-col>
          
          <v-col cols="12" md="4" class="mb-6">
            <v-card class="feature-card h-100" elevation="8" rounded="xl">
              <v-card-text class="pa-8 text-center">
                <div class="feature-icon mb-4">
                  <v-avatar size="80" color="success" variant="tonal">
                    <v-icon size="40">mdi-office-building</v-icon>
                  </v-avatar>
                </div>
                <h3 class="feature-title mb-3">For Employers</h3>
                <p class="feature-description">
                  Post job openings, manage applications, and find the perfect candidates
                </p>
              </v-card-text>
            </v-card>
          </v-col>
          
          <v-col cols="12" md="4" class="mb-6">
            <v-card class="feature-card h-100" elevation="8" rounded="xl">
              <v-card-text class="pa-8 text-center">
                <div class="feature-icon mb-4">
                  <v-avatar size="80" color="orange" variant="tonal">
                    <v-icon size="40">mdi-shield-check</v-icon>
                  </v-avatar>
                </div>
                <h3 class="feature-title mb-3">Secure & Fast</h3>
                <p class="feature-description">
                  Built with security in mind, fast performance, and modern technology
                </p>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- CTA Section -->
    <section class="cta-section">
      <v-container class="px-4">
        <v-row justify="center">
          <v-col cols="12" md="8" class="text-center">
            <v-card class="cta-card" elevation="16" rounded="xl">
              <v-card-text class="pa-12">
                <h2 class="cta-title mb-4">Ready to Get Started?</h2>
                <p class="cta-description mb-8">
                  Join thousands of professionals who trust our platform for their career journey
                </p>
                <div class="cta-actions">
                  <v-btn
                    size="large"
                    color="primary"
                    class="mr-4 mb-4"
                    rounded="lg"
                    @click="$router.push('/register')"
                  >
                    Create Account
                  </v-btn>
                  <v-btn
                    size="large"
                    variant="text"
                    class="mb-4"
                    @click="$router.push('/login')"
                  >
                    Already have an account?
                  </v-btn>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>
  </div>
</template>

<style scoped>
.home-container {
  min-height: 100vh;
  width: 100%;
  max-width: 100vw;
}

.hero-section {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  position: relative;
}

.hero-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.3);
  z-index: 1;
}

.hero-content {
  position: relative;
  z-index: 2;
}

.hero-badge :deep(.v-chip) {
  background: rgba(255, 255, 255, 0.1) !important;
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white !important;
}

.hero-title {
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 800;
  line-height: 1.2;
  margin-bottom: 1.5rem;
}

.hero-subtitle {
  font-size: clamp(1.1rem, 2vw, 1.5rem);
  opacity: 0.9;
  max-width: 600px;
  margin: 0 auto;
}

.hero-actions .v-btn {
  min-width: 180px;
  font-weight: 600;
}

.features-section {
  padding: 120px 0;
  background: #f8f9fa;
}

.section-title {
  font-size: clamp(2rem, 4vw, 3rem);
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 1rem;
}

.section-subtitle {
  font-size: 1.25rem;
  color: #6c757d;
  max-width: 600px;
  margin: 0 auto;
}

.feature-card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.feature-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1) !important;
}

.feature-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #2c3e50;
}

.feature-description {
  color: #6c757d;
  font-size: 1rem;
  line-height: 1.6;
}

.cta-section {
  padding: 120px 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.cta-card {
  background: rgba(255, 255, 255, 0.95) !important;
  backdrop-filter: blur(10px);
}

.cta-title {
  font-size: clamp(2rem, 3vw, 2.5rem);
  font-weight: 700;
  color: #2c3e50;
}

.cta-description {
  font-size: 1.25rem;
  color: #6c757d;
}

@media (max-width: 960px) {
  .hero-section {
    min-height: 80vh;
  }
  
  .features-section,
  .cta-section {
    padding: 80px 0;
  }
  
  .hero-actions .v-btn {
    min-width: 160px;
    display: block;
    width: 100%;
    max-width: 300px;
    margin: 0 auto 1rem;
  }
}
</style>
