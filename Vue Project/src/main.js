import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './registerServiceWorker'
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
import colors from 'vuetify/es5/util/colors'
import 'babel-polyfill'
import moment from 'moment'
import BlockUI from 'vue-blockui'
 
Vue.use(Vuetify, {
  theme: {
    primary: String(colors.grey.lighten3),
    accent: String(colors.blueGrey.lighten5),
    secondary: String(colors.grey.lighten2),
    info: String(colors.teal.lighten1),
    warning: String(colors.amber.base),
    error: String(colors.deepOrange.accent4),
    success: String(colors.green.accent3)
  }
})

Vue.use(BlockUI)

Vue.filter('formatDate', function(value) {
  if (value) {
    return moment(value).format('MM/DD/YYYY')
  }
});

Vue.config.productionTip = false

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
