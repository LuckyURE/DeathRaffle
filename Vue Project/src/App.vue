<template>
    <v-app>
       <BlockUI v-if="loading" :message="message" :url="loader"></BlockUI>
      <v-navigation-drawer temporary absolute v-model="sideNav">
        <v-list>
          <v-list-tile
            v-for="item in menuItems"
            :key="item.title"
            router
            :to="item.link">
            <v-list-tile-action>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              {{ item.title }}
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
      </v-navigation-drawer>
      <v-toolbar fixed app>
        <v-toolbar-side-icon
          @click.native.stop="sideNav = !sideNav"
          class="hidden-sm-and-up"></v-toolbar-side-icon>
        <v-toolbar-title>
          <router-link to="/" tag="span" style="cursor: pointer">Death Raffle</router-link>
        </v-toolbar-title>
        <v-spacer></v-spacer>
        <v-toolbar-items class="hidden-xs-only">
          <v-btn
            flat
            v-for="item in menuItems"
            :key="item.title"
            router
            :to="item.link">
            <v-icon left>{{ item.icon }}</v-icon>
            {{ item.title }}
          </v-btn>
        </v-toolbar-items>
      </v-toolbar>
      <v-content>
        <router-view></router-view>
      </v-content>
    </v-app>
</template>

<script>
import Loader from './assets/loader.gif'

  export default {
    data() {
      return {
        sideNav: false,
        loader: Loader
      }
    },
    computed: {
      menuItems () {
        return this.$store.getters.navItems
      },
      loading () {
        return this.$store.getters.loading
      },
      message () {
        return this.$store.getters.loadingMessage
      }
    }
  }
</script>