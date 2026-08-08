import type { RouteRecordRaw } from 'vue-router'

export const userRoutes: RouteRecordRaw[] = [
  {
    path: '/password/change',
    name: 'ChangePassword',
    component: () => import('@/views/users/ChangePassword.vue')
  },
  {
    path: '/userpanel',
    name: 'UserPanel',
    component: () => import('@/views/users/UserPanel.vue')
  },
  {
    path: '/users/:id/advertisements',
    name: 'UsersAdvertisements',
    component: () => import('@/views/users/UsersAdvertisements.vue')
  },
  {
    path: '/users/:id',
    name: 'UserProfile',
    component: () => import('@/views/users/UserProfile.vue')
  }
]
