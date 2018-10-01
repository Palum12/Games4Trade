import axios from 'axios'
export default {
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
