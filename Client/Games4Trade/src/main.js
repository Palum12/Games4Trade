import Vue from 'vue'
import App from './App.vue'
import router from './router/router'
import store from './store/store'
import axios from 'axios'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
Vue.config.productionTip = false

axios.defaults.baseURL = 'https://localhost:44303/api/'

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
