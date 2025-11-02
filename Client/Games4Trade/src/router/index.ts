import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

import { advertisementRoutes } from './routes/advertisements'
import { announcementRoutes } from './routes/announcements'
import { authRoutes } from './routes/auth'
import { messageRoutes } from './routes/messages'
import { userRoutes } from './routes/users'

const routes: RouteRecordRaw[] = [
  ...userRoutes,
  ...authRoutes,
  ...announcementRoutes,
  ...advertisementRoutes,
  ...messageRoutes,
  {
    path: '/',
    name: 'Home',
    component: () => import('@/views/Home.vue')
  },
  {
    path: '/admin',
    name: 'Admin',
    component: () => import('@/views/AdminPanel.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ left: 0, top: 0 })
})

export default router
