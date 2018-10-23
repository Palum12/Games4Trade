import Login from '../components/users/Login'
import SignUp from '../components/users/SignUp'

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
