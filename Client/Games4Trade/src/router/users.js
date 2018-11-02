import ChangePassword from '../views/users/ChangePassword'
import UserPanel from '../views/users/UserPanel'
import UserProfile from '../views/users/UserProfile'
import UsersAdvertisements from '../views/users/UsersAdvertisements'

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
  USER_ADVERTISEMENTS: {
    path: '/users/:id/advertisements',
    name: 'UsersAdvertisements',
    component: UsersAdvertisements
  },
  USER_PROFILE: {
    path: '/users/:id',
    name: 'UserProfile',
    component: UserProfile
  }
}
