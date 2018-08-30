import Login from '../../components/users/Login'
import SignUp from '../../components/users/SignUp'
import RecoverPassword from '../../components/users/RecoverPassword'

export default {
  LOGIN: {
    path: '/login',
    name: 'Login',
    component: Login,
    children: [
      {
        path: '/recover',
        component: RecoverPassword
      }
    ]
  },
  SIGNUP: {
    path: '/signup',
    name: 'SignUp',
    component: SignUp
  }
}
