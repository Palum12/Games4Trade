import axios from 'axios'

const state = {
  systems: []
}

const getters = {
  systems (state) {
    return state.systems
  }
}

const mutations = {
  setSystems (state, systems) {
    state.systems = systems
  }
}

const actions = {
  getSystems ({commit}) {
    return axios.get('/systems')
      .then(response => {
        commit('setSystems', response.data)
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
