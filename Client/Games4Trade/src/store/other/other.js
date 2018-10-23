import axios from 'axios'

const state = {
  isSpinnerLoading: false
}

const getters = {
  isSpinnerLoading: state => state.isSpinnerLoading
}

const actions = {
  setSpinnerLoading ({state}) {
    state.isSpinnerLoading = true
  },
  unsetSpinnerLoading ({state}) {
    state.isSpinnerLoading = false
  },
  getAnnouncement ({dispatch}, id) {
    dispatch('setSpinnerLoading')
    return axios.get(`/announcements/${id}`)
      .then(response => {
        dispatch('unsetSpinnerLoading')
        return Promise.resolve(response)
      })
      .catch(error => {
        dispatch('unsetSpinnerLoading')
        return Promise.reject(error)
      })
  }
}

export default{
  state,
  getters,
  actions
}
