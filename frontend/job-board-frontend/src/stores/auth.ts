import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface User {
  id: string
  email: string
  role: 'admin' | 'candidate'
  firstName?: string
  lastName?: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = ref<string | null>(localStorage.getItem('token'))

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'admin')
  const isCandidate = computed(() => user.value?.role === 'candidate')

  function setAuth(userData: User, authToken: string) {
    user.value = userData
    token.value = authToken
    localStorage.setItem('token', authToken)
  }

  function clearAuth() {
    user.value = null
    token.value = null
    localStorage.removeItem('token')
  }

  async function login(email: string, password: string) {
    try {
      const response = await fetch('http://localhost:5000/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      })

      if (!response.ok) {
        throw new Error('Login failed')
      }

      const data = await response.json()
      setAuth(data.user, data.token)
      return data
    } catch (error) {
      console.error('Login error:', error)
      throw error
    }
  }

  async function register(email: string, password: string, firstName: string, lastName: string, role: 'admin' | 'candidate') {
    try {
      const response = await fetch('http://localhost:5000/api/auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password, firstName, lastName, role }),
      })

      if (!response.ok) {
        throw new Error('Registration failed')
      }

      const data = await response.json()
      setAuth(data.user, data.token)
      return data
    } catch (error) {
      console.error('Registration error:', error)
      throw error
    }
  }

  function logout() {
    clearAuth()
  }

  return {
    user,
    token,
    isAuthenticated,
    isAdmin,
    isCandidate,
    login,
    register,
    logout,
    setAuth,
    clearAuth
  }
})