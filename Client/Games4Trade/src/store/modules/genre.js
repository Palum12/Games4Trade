import axios from 'axios'

const state = {
  genres: []
}

const getters = {
  genres (state) {
    return state.genres
  }
}

const mutations = {
  setGenres (state, genres) {
    state.genres = genres
  }
}

const actions = {
  getGenres ({commit}) {
    return axios.get('/genres')
      .then(response => {
        commit('setGenres', response.data)
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
