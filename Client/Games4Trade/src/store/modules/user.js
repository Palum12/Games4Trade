import axios from 'axios'
import router from '../../router/router'
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
  },
  isAdmin (state) {
    if (state.userLoggedIn) {
      return state.userData.role === 'Admin'
    }
    return false
  },
  getToken (state) {
    return state.userData.token
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
    state.userData = {
      name: null,
      role: null,
      obtainingTime: null,
      expirationTime: null,
      token: null
    }
    state.userLoggedIn = false
  }
}

const actions = {
  signUp ({commit, dispatch}, newUser) {
    return axios.post('users', {
      email: newUser.email,
      login: newUser.login,
      password: newUser.password
    }).then(response => {
      return Promise.resolve(response)
    })
      .catch(error => {
        return Promise.reject(error)
      })
  },
  login ({commit}, authData) {
    return axios.post('login', {
      login: authData.login,
      password: authData.password
    }).then(response => {
      let decodedToken = jwt(response.data)
      decodedToken['token'] = `Bearer ${response.data}`
      commit('authUser', decodedToken)
      localStorage.setItem('token', response.data)
      localStorage.setItem('expirationTime', decodedToken['exp'])
      return Promise.resolve()
    }).catch(error => {
      commit('clearAuthData')
      return Promise.reject(error)
    })
  },
  tryAutoLogin ({commit}) {
    const token = localStorage.getItem('token')
    if (!token) {
      return
    }
    const expirationDate = localStorage.getItem('expirationTime')
    const dateNow = new Date()
    if (dateNow.getTime() / 1000 >= expirationDate) {
      return
    }
    let decodedToken = jwt(token)
    decodedToken['token'] = `Bearer ${token}`
    commit('authUser', decodedToken)
  },
  logout ({commit}) {
    commit('clearAuthData')
    router.push('/')
    localStorage.clear()
  }
}

export default{
  state,
  mutations,
  getters,
  actions
}
