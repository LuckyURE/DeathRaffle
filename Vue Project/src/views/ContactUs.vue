<template>
  <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar light color="primary">
                <v-toolbar-title>Send Us A Message</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-form v-model="valid" ref="form" lazy-validation>
                  <v-text-field
                    v-model="form.name" 
                    prepend-icon="person" 
                    name="name" 
                    label="Name" 
                    type="text"
                    :rules="nameRules"
                    :counter="100"
                    required></v-text-field>
                  <v-text-field
                    v-model="form.email"
                    prepend-icon="email" 
                    name="email" 
                    label="Email Address" 
                    id="email" 
                    type="email"
                    :rules="emailRules"
                    :counter="254"
                    required></v-text-field>
                    <v-text-field
                    v-model="form.message"
                    name="message"
                    label="Message"
                    :rules="messageRules"
                    :counter="1000"
                    textarea
                    required
                ></v-text-field>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn 
                    @click="submit"
                    light 
                    color="secondary">
                    Send
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-flex>
          <v-snackbar
            :timeout=timeout
            :color=color
            v-model="snackbar"
            dark
          >
            {{ text }}
            <v-btn dark flat @click.native="snackbar = false">Close</v-btn>
          </v-snackbar>
        </v-layout>
</v-container>
</template>

<script>
export default {
  data() {
      return {
          form: {
              name: "",
              email: "",
              message: ""
          },
          valid: false,
          nameRules: [
            v => !!v || "Name is required",
            v => (v && v.length <= 100) || "Name must be less than 100 characters"
          ],
          emailRules: [
            v => !!v || 'E-mail is required',
            v => /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(v) || 'E-mail must be valid'
          ],
          messageRules: [
            v => !!v || "Message is required",
            v => (v && v.length <= 1000) || "Name must be less than 1000 characters"
          ],
          snackbar: false,
          timeout: 6000,
          color: '',
          text: ''
      }
  },
  methods: {
        submit() {
            if (this.$refs.form.validate()) {
                this.$store.dispatch('sendMessage', this.form)
                    .then(() => {
                        this.$refs.form.reset();
                        this.color = "cyan darken-2"
                        this.text = "We have received your message and will respond soon."
                        this.snackbar = true;
                    })
                    .catch(() => {
                        this.color = "error"
                        this.text = this.$store.getters.error.response.data
                        this.snackbar = true;
                    })
            } else {
                this.color = "error",
                this.text = "The form isn't valid, please resolve the errors and try again.",
                this.snackbar = true;
            }
        }
    }
}
</script>
