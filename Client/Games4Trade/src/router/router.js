import Vue from 'vue'
import Router from 'vue-router'
import Home from '../views/Home.vue'
import Admin from '../views/AdminPanel'
import Navbar from './navbar/navbar'
import Annoucements from './announcements/announcements'
import Users from './users/users'
Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    Users.CHANGE_PASSWORD,
    Navbar.LOGIN,
    Navbar.SIGNUP,
    Annoucements.ADDANNOUNCEMENT,
    Annoucements.EDITANNOUNCEMENT,
    Annoucements.ANNOUNCEMENT,
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/admin',
      name: 'admin',
      component: Admin
    },
    {path: '*', redirect: '/'}
  ]
})
