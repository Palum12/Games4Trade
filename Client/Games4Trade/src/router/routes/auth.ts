import type { RouteRecordRaw } from 'vue-router'

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/users/Login.vue')
  },
  {
    path: '/signup',
    name: 'Signup',
    component: () => import('@/views/users/SignUp.vue')
  }
]
