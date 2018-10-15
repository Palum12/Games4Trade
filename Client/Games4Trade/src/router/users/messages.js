import Messages from '../../views/Messages'
import Conversation from '../../components/messages/Conversation'

export default {
  MESSAGES: {
    path: '/messages',
    name: 'Messages',
    component: Messages,
    children: [
      {
        path: ':otherUserId/conversation',
        component: Conversation
      }
    ]
  }
}
