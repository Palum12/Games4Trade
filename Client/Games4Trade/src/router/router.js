import Vue from 'vue'
import Router from 'vue-router'
import Home from '../views/Home.vue'
import Navbar from './navbar/navbar'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    Navbar.LOGIN,
    Navbar.SIGNUP,
    {
      path: '/',
      name: 'home',
      component: Home
    }
  ]
})
