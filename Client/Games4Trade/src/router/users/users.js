import ChangePassword from '../../components/users/ChangePassword'
import UserPanel from '../../views/UserPanel'
import UserProfile from '../../views/UserProfile'

export default {
  CHANGE_PASSWORD: {
    path: '/password/change',
    name: 'ChangePassword',
    component: ChangePassword
  },
  USER_PANEL: {
    path: '/userpanel',
    name: 'UserPanel',
    component: UserPanel
  },
  USER_PROFILE: {
    path: '/users/:id',
    name: 'UserProfile',
    component: UserProfile
  }
}
