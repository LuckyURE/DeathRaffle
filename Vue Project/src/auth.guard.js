import store from './store'
import firebase from 'firebase'

export default (to, from, next) => {
    if(store.getters.user) {
        next()
    } else {
        next('/admin')
    }
}

firebase.auth().onAuthStateChanged((user) => {
    if(user) {
      store.dispatch('autoSignIn', user)
    }
  })