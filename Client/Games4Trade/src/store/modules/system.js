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
    return new Promise((resolve, reject) => {
      axios.get('/systems')
        .then(response => {
          commit('setSystems', response.data)
          resolve()
        })
        .catch(error => {
          reject(error)
        })
    })
  }
}

export default{
  state,
  mutations,
  getters,
  actions
}
