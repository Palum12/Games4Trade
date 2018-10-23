import axios from 'axios'

const state = {
  regions: []
}

const getters = {
  regions (state) {
    return state.regions
  }
}

const mutations = {
  setRegions (state, regions) {
    state.regions = regions
  }
}

const actions = {
  getRegions ({commit}) {
    return axios.get('/regions')
      .then(response => {
        commit('setRegions', response.data)
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
