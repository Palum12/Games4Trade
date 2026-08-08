import type { RouteRecordRaw } from 'vue-router'

export const announcementRoutes: RouteRecordRaw[] = [
  {
    path: '/announcements/add',
    name: 'AddAnnouncement',
    component: () => import('@/views/announcements/CreateOrUpdateAnnouncement.vue')
  },
  {
    path: '/announcements/:id/edit',
    name: 'EditAnnouncement',
    component: () => import('@/views/announcements/CreateOrUpdateAnnouncement.vue')
  },
  {
    path: '/announcements/:id',
    name: 'Announcement',
    component: () => import('@/views/announcements/ShowAnnouncement.vue')
  }
]
