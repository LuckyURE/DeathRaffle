import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import About from './views/About.vue'
import Ticket from './views/TicketSearch.vue'
import ContactUs from './views/ContactUs.vue'
import ErrorInfo from './views/Error.vue'
import Admin from './views/admin/Index.vue'
import AdminDashboard from './views/admin/Dashboard.vue'
import Logoff from './views/Logoff.vue'
import AuthGuard from './auth.guard'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/about',
      name: 'about',
      component: About
    },
    {
      path: '/ticket',
      name: 'Ticket Search',
      component: Ticket
    },
    {
      path: '/contactus',
      name: 'Contact Us',
      component: ContactUs
    },
    {
      path: '/error/:id',
      name: 'Error Information',
      component: ErrorInfo
    },
    {
      path: '/admin',
      name: 'Admin Signin',
      component: Admin
    },
    {
      path: '/admin/dashboard',
      name: 'Admin Dashboard',
      component: AdminDashboard,
      beforeEnter: AuthGuard
    },
    {
      path: '/Logoff',
      name: 'Logoff',
      component: Logoff
    }
  ]
})
