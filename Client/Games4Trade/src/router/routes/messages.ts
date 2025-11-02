import type { RouteRecordRaw } from 'vue-router'

export const messageRoutes: RouteRecordRaw[] = [
  {
    path: '/messages',
    name: 'Messages',
    component: () => import('@/views/Messages.vue'),
    children: [
      {
        path: ':otherUserId/conversation',
        component: () => import('@/components/messages/Conversation.vue')
      }
    ]
  }
]
