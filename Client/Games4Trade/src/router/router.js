import Vue from 'vue'
import Router from 'vue-router'
import Home from '../views/Home.vue'
import Admin from '../views/AdminPanel'
import Navbar from './navbar'
import Annoucements from './announcements'
import Advertisements from './advetisements'
import Users from './users'
import Messages from './messages'
Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    Users.CHANGE_PASSWORD,
    Users.USER_PANEL,
    Users.USER_ADVERTISEMENTS,
    Users.USER_PROFILE,
    Navbar.LOGIN,
    Navbar.SIGNUP,
    Annoucements.ADDANNOUNCEMENT,
    Annoucements.EDITANNOUNCEMENT,
    Annoucements.ANNOUNCEMENT,
    Advertisements.ADDADVERTISEMENT,
    Advertisements.EDITADVERTISEMENT,
    Advertisements.SHOWADVERTISEMENT,
    Advertisements.SEARCHADVERTISEMENTS,
    Messages.MESSAGES,
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
