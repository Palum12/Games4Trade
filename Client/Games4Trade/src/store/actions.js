import axios from 'axios'
export default {
  setSpinnerLoading ({state}) {
    state.isSpinnerLoading = true
  },
  unsetSpinnerLoading ({state}) {
    state.isSpinnerLoading = false
  },
  getAnnouncements ({dispatch}) {
    dispatch('setSpinnerLoading')
    return new Promise((resolve, reject) => {
      axios.get('/announcements')
        .then(response => {
          dispatch('unsetSpinnerLoading')
          resolve(response)
        })
        .catch(error => {
          dispatch('unsetSpinnerLoading')
          reject(error)
        })
    })
  },
  getAnnouncement ({dispatch}, id) {
    dispatch('setSpinnerLoading')
    return new Promise((resolve, reject) => {
      axios.get(`/announcements/${id}`)
        .then(response => {
          dispatch('unsetSpinnerLoading')
          resolve(response)
        })
        .catch(error => {
          dispatch('unsetSpinnerLoading')
          reject(error)
        })
    })
  }
}
