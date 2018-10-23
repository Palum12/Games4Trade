import axios from 'axios'

const state = {
  states: []
}

const getters = {
  states (state) {
    return state.states
  }
}

const mutations = {
  setStates (state, states) {
    state.states = states
  }
}

const actions = {
  getStates ({commit}) {
    return axios.get('/states')
      .then(response => {
        commit('setStates', response.data)
        return Promise.resolve()
      })
      .catch(error => {
        return Promise.reject(error)
      })
  }
}

export default{
  state,
  mutations,
  getters,
  actions
}
