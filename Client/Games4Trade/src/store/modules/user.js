import axios from 'axios'
import jwt from 'jwt-decode'

const state = {
  userLoggedIn: false,
  userData: {
    name: null,
    role: null,
    obtainingTime: null,
    expirationTime: null,
    token: null
  }
}

const getters = {
  isAuthenticated (state) {
    return state.userLoggedIn
  }
}

const mutations = {
  authUser (state, userData) {
    state.userLoggedIn = true
    state.userData.name = userData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    state.userData.role = userData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    state.userData.expirationTime = userData['exp']
    state.userData.obtainingTime = userData['nbf']
    state.userData.token = userData['token']
  },
  clearAuthData (state) {
    state.user = null
    state.userLoggedIn = false
  }
}

const actions = {
  login ({commit}, authData) {
    axios.post('login', {
      login: authData.login,
      password: authData.password
    }).then(response => {
      let decodedToken = jwt(response.data)
      decodedToken['token'] = `Bearer ${response.data}`
      commit('authUser', decodedToken)
    }).catch(error => {
      console.log(error)
      commit('clearAuthData')
    })
  }
}

export default{
  state,
  mutations,
  getters,
  actions
}
