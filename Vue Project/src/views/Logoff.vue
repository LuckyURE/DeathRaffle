<template>
  <v-container>
      <v-layout>
          <v-flex xs12 sm6 offset-sm3>
            <p>You have been logged out.</p>
          </v-flex>
      </v-layout>
  </v-container>
</template>

<script>
export default {
  mounted () {
      this.$store.dispatch('logout')
  }
}
</script>
