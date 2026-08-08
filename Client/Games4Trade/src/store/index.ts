import { createStore } from 'vuex'

import other from './other/other'
import genre from './modules/genre'
import region from './modules/region'
import stateModule from './modules/state'
import system from './modules/system'
import user from './modules/user'

const store = createStore({
  modules: {
    other,
    user,
    genre,
    system,
    region,
    state: stateModule
  }
})

export default store
