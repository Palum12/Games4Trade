import Vue from 'vue'
import Vuex from 'vuex'
import user from './modules/user'
import genre from './modules/genre'
import getters from './getters'
import actions from './actions'
import state from './state'

Vue.use(Vuex)

export default new Vuex.Store({
  state,
  getters,
  actions,
  modules: {
    user,
    genre
  }
})
