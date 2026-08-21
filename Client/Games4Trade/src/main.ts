import { createApp } from 'vue'
import axios from 'axios'
import VueSweetalert2 from 'vue-sweetalert2'

import App from './App.vue'
import router from './router'
import store from './store'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'
import 'sweetalert2/dist/sweetalert2.min.css'

const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api/'
axios.defaults.baseURL = apiUrl

axios.interceptors.request.use((config) => {
  const token = store.getters.getToken as string | null | undefined
  if (token) {
    config.headers.Authorization = token
  }
  return config
})

const app = createApp(App)

app.use(store)
app.use(router)
app.use(VueSweetalert2)

app.mount('#app')
