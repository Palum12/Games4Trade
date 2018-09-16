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
    return new Promise((resolve, reject) => {
      axios.get('/genres')
        .then(response => {
          commit('setGenres', response.data)
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
