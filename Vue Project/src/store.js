import Vue from 'vue'
import Vuex from 'vuex'
import firebase from 'firebase'
import 'firebase/firestore'
import axios from 'axios'

Vue.use(Vuex)

//axios.defaults.baseURL = `http://deathraffle.com/`;
axios.defaults.baseURL = `http://localhost:5002/`;
axios.defaults.headers.delete['Accept'] = 'application/json';
axios.defaults.headers.delete['Content-Type'] = 'application/json';

var config = {
  apiKey: "Insert Your Own Key",
  authDomain: "Insert Your Own Domain",
  databaseURL: "Insert Your Own Domain Url",
  projectId: "Insert Project ID",
  storageBucket: "Insert Storage Bucket",
  messagingSenderId: "Insert Sender ID"
};

firebase.initializeApp(config)

export default new Vuex.Store({
  state: {
    lumensRequired: 0,
    db: firebase.firestore(),
    user: null,
    pools: [
      {
        id: 'id#',
        closeDate: new Date(),
        createDate: new Date(),
        lockDate: new Date(),
        winningTicket: '',
        ticketCount: 0
      }
    ],
    livingCelebs: [],
    appInfo: {
      wallet: 'somewalletaddress',
      lumensRequired: 33
    },
    ticket: {
      id: 0,
      celebrityId: 'some_celeb_id',
      playerAddress: 'some_player_address',
      poolId: 0
    },
    navItems: [
      { icon: 'info', title: 'About', link: '/About' },
      { icon: 'search', title: 'Ticket Search', link: '/Ticket' },
      { icon: 'email', title: 'Contact Us', link: '/ContactUs' }
    ],
    loading: false,
    loadingMessage: "Please wait while your request is processed..",
    error: null
  },
  mutations: {
    setLumensRequired (state, payload) {
      state.lumensRequired = payload
    },
    setPools (state, payload) {
      state.pools = payload
    },
    setAppInfo (state, payload) {
      state.appInfo = payload
    },
    setUser (state, payload) {
      state.user = payload
      if(payload == null) {
        state.navItems.pop()
      }
    },
    setTicket (state, payload) {
      state.ticket = payload
    },
    setLivingCelebs (state, payload) {
      state.livingCelebs = payload
    },
    setNavItems (state) {
      state.navItems.push({
        icon: 'exit_to_app',
        title: 'Logoff',
        link: '/Logoff'
      })
    },
    setLoading (state, payload) {
      state.loading = payload.loading
      state.loadingMessage = payload.message
    },
    setError (state, payload) {
      state.error = payload
    },
    clearError (state) {
      state.error = null
    }
  },
  actions: {
    getLumensRequired ({commit}) {
      axios.get(`https://api.coinmarketcap.com/v1/ticker/stellar/?convert=usd`)
        .then((result) => {
          let numRequired = 10.00 / result.data[0].price_usd
          commit('setLumensRequired', numRequired.toFixed(2))
        })
    },
    getPools ({commit}) {
      commit('setLoading', {loading: true, message: "Please wait while we get the latest pool information.."})
      commit('clearError')
      axios.get(`/api/pool/poolDashboard`)
        .then((result) => {
          commit('setPools', result.data)
          commit('setLoading', {loading: false, message: ""})
        })
        .catch((error) => {
          commit('setError', error)
          commit('setLoading', {loading: false, message: ""})
        })
    },
    getAppInfo ({commit}) {
      axios.get(`/api/appInfo`)
        .then((result) => {
          commit('setAppInfo', result.data)
        })
        .catch((error) => {
          commit('setError', error)
        })
    },
    signUserIn ({commit}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while we sign you in.."})
      commit('clearError')
      firebase.auth().signInWithEmailAndPassword(payload.email, payload.password)
        .then(
          user => {
            commit('setLoading', {loading: false, message: ""})
            const newUser = {
              id: user.uid
            };
            commit('setUser', newUser)
            commit('setNavItems')
          }
        )
        .catch(error => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
        })
    },
    findTicket ({commit}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while I find your ticket.."})
      commit('clearError')
      return new Promise((resolve, reject) => {
        axios.get(`/api/ticket/${payload}`)
        .then((result) => {
          commit('setTicket', result.data)
          commit('setLoading', {loading: false, message: ""})
          resolve()
        })
        .catch((error) => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
          reject()
        })
      })
    },
    getLivingCelebs ({commit}) {
      commit('setLoading', {loading: true, message: "Please wait while I load the celebrity information for you.."})
      commit('clearError')
      axios.get(`/api/celebrity/getLiving`)
      .then((result) => {
        commit('setLivingCelebs', result.data)
        commit('setLoading', {loading: false, message: ""})
      })
    },
    addCeleb ({commit, dispatch}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while I add your new celebrity.."})
      commit('clearError')
      return new Promise((resolve, reject) => {
        axios.post(`/api/celebrity`, payload)
        .then(() => {
          commit('setLoading', {loading: false, message: ""})
          dispatch('getLivingCelebs')
          resolve()
        })
        .catch((error) => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
          reject()
        })
      })
    },
    removeCeleb ({commit, dispatch}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while I delete the celebrity.."})
      commit('clearError')
      return new Promise((resolve, reject) => {
        axios({
          method: "delete",
          url: "/api/celebrity",
          data: payload
        })
        .then(() => {
          commit('setLoading', {loading: false, message: ""})
          dispatch('getLivingCelebs')
          resolve()
        })
        .catch((error) => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
          reject()
        })
      })
    },
    markDead ({commit, dispatch}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while I mark this celebrity as dead and close the pools.."})
      commit('clearError')
      return new Promise((resolve, reject) => {
        axios({
          method: "put",
          url: "/api/celebrity/markDead",
          data: payload
        })
        .then(() => {
          commit('setLoading', {loading: false, message: ""})
          dispatch('getLivingCelebs')
          resolve()
        })
        .catch((error) => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
          reject()
        })
      })
    },
    sendMessage ({commit}, payload) {
      commit('setLoading', {loading: true, message: "Please wait while I send your message to the DeathRaffle team.."})
      commit('clearError')
      return new Promise((resolve, reject) => {
        axios({
          method: "post",
          url: "/api/appInfo/sendMessage",
          data: payload
        })
        .then(() => {
          commit('setLoading', {loading: false, message: ""})
          resolve()
        })
        .catch((error) => {
          commit('setLoading', {loading: false, message: ""})
          commit('setError', error)
          reject()
        })
      })
    },
    autoSignIn ({commit}, payload) {
      commit('setUser', {id: payload.uid})
    },
    logout ({commit}) {
      firebase.auth().signOut()
      commit('setUser', null)
    },
    clearError ({commit}) {
      commit('clearError')
    }
  },
  getters: {
    lumensRequired (state) {
      return state.lumensRequired
    },
    pools (state) {
      return state.pools
    },
    appInfo (state) {
      return state.appInfo
    },
    ticket (state) {
      return state.ticket
    },
    user (state) {
      return state.user
    },
    livingCelebs (state) {
      return state.livingCelebs
    },
    navItems (state) {
      return state.navItems
    },
    loading (state) {
      return state.loading
    },
    loadingMessage (state) {
      return state.loadingMessage
    },
    error (state) {
      return state.error
    }
  }
})
