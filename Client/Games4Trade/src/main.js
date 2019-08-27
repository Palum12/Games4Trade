import Vue from 'vue'
import App from './App.vue'
import router from './router/router'
import store from './store/store'
import axios from 'axios'
import VueSweetAlert2 from 'vue-sweetalert2'
import Vuelidate from 'vuelidate'
import {Tabs, Tab} from 'vue-tabs-component'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
Vue.config.productionTip = false

Vue.use(VueSweetAlert2)
Vue.use(Vuelidate)
Vue.component('tabs', Tabs)
Vue.component('tab', Tab)
axios.defaults.baseURL = process.env.VUE_APP_API_URL

axios.interceptors.request.use((config) => {
  let authToken = store.getters.getToken
  if (authToken) {
    config.headers['Authorization'] = authToken
  }
  return config
}, (error) => {
  return Promise.reject(error)
})

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
