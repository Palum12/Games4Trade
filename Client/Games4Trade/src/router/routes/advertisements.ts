import type { RouteRecordRaw } from 'vue-router'

export const advertisementRoutes: RouteRecordRaw[] = [
  {
    path: '/advertisements/add',
    name: 'AddAdvertisement',
    component: () => import('@/views/advertisements/CreateOrUpdateAdvertisement.vue')
  },
  {
    path: '/advertisements/:id/edit',
    name: 'EditAdvertisement',
    component: () => import('@/views/advertisements/CreateOrUpdateAdvertisement.vue')
  },
  {
    path: '/advertisements/:id',
    name: 'ShowAdvertisement',
    component: () => import('@/views/advertisements/ShowAdvertisement.vue')
  },
  {
    path: '/advertisements/search/:text?',
    name: 'SearchAdvertisement',
    component: () => import('@/views/advertisements/SearchAdvertisements.vue')
  }
]
