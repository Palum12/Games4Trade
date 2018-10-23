import Vue from 'vue'
import Vuex from 'vuex'
import user from './modules/user'
import genre from './modules/genre'
import system from './modules/system'
import state from './modules/state'
import region from './modules/region'
import other from './other/other'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    other,
    user,
    genre,
    system,
    region,
    state
  }
})
