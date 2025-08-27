<template>
  <div>
    <h1>Admin Dashboard Debug</h1>
    <p>If you can see this, the route navigation worked!</p>
    <p>User: {{ authStore.user?.email || 'Not logged in' }}</p>
    <p>Role: {{ authStore.user?.role || 'No role' }}</p>
    <p>Token exists: {{ !!authStore.token }}</p>
    <v-btn @click="handleLogout">Logout</v-btn>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

console.log('🎯 Admin Debug View Loaded')
console.log('👤 Auth Store:', authStore)
console.log('👤 User:', authStore.user)
console.log('🔑 Token:', !!authStore.token)

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>