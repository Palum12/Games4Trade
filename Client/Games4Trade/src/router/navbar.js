import Login from '../views/users/Login'
import SignUp from '../views/users/SignUp'

export default {
  LOGIN: {
    path: '/login',
    name: 'login',
    component: Login
  },
  SIGNUP: {
    path: '/signup',
    name: 'signup',
    component: SignUp
  }
}
