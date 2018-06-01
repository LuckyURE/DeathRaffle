<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar light color="primary">
                <v-toolbar-title>Error Information</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <p v-html="errorText"></p>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn 
                    route to="/"
                    light 
                    color="secondary">
                    Return Home
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
  data () {
      return {
          errorText: ""
      }
  },
  mounted() {
      switch (this.$route.params.id) {
          case "1":
              this.errorText = `<p></p>Your account was immediately refunded with the full amount you sent,
                minus a small transaction fee.  It seems that you didn't send enough funds, equal to
                or greater than $10 USD in order to cover the price of your raffle ticket.</p>
                
                <p>Please check the current minimum amount required on our home page and try again.</p>
                
                <p>Thank you for choosing DeathRaffle and good luck!</p>`
              break;
      
          default:
            this.errorText = `An unknown error occured.  If you feel you have received this error
                by mistake please use the Contact Us link from the menu and let us know about it.
                
                Thank you for choosing DeathRaffle and good luck!`
              break;
      }
  }
}
</script>
