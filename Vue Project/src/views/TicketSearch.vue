<template>
  <v-content>
    <v-container fluid fill-height grid-list-lg>
      <v-layout align-center justify-center row wrap>
        <v-flex xs12 sm8 md4>
          <v-card class="elevation-12">
            <v-toolbar light color="primary">
              <v-toolbar-title>Find Your Raffle Ticket</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
              <v-form @submit.prevent="findTicket">
                <v-text-field
                  prepend-icon="confirmation_number"
                  name="search"
                  v-model="search"
                  label="Enter Ticket Number"
                  type="text"
                  required
                  max="20"
                  :rules="[() => search.length > 0 || 'You must enter a ticket number']">

                </v-text-field>
              </v-form>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                light
                color="primary"
                @click="findTicket"
                :loading="loading"
                :disabled="loading">
                Search
                <span slot="findTicket" class="custom-loader">
                    <v-icon light>cached</v-icon>
                  </span>
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-flex>

        <v-dialog v-model="ticketModal" scrollable max-width="500px">
          <v-card>
            <table>
              <caption>Raffle Ticket Information</caption>
              <tbody>
              <tr>
                <td scope="row"><strong>Ticket ID</strong></td>
                <td>{{ticket.ticketId}}</td>
              </tr>
              <tr>
                <td scope="row"><strong>Pool ID</strong></td>
                <td>{{ticket.poolId}}</td>
              </tr>
              <tr>
                <td scope="row"><strong>Player Address</strong></td>
                <td class="truncate">{{ticket.playerAddress}}</td>
              </tr>
              <tr>
                <td scope="row"><strong>Name</strong></td>
                <td v-if="Object.keys(ticket).length !== 0">
                  {{ ticket.celebrity.title || ''}} {{ticket.celebrity.firstName}} {{ticket.celebrity.middleName || ''}}
                  {{ticket.celebrity.lastName}} {{ticket.celebrity.suffix || ''}}
                </td>
              </tr>
              <tr>
                <td scope="row"><strong>Age</strong></td>
                <td v-if="Object.keys(ticket).length !== 0">
                  {{ticket.celebrity.age}}
                </td>
              </tr>
              <tr>
                <td scope="row"><strong>Description</strong></td>
                <td v-if="Object.keys(ticket).length !== 0">
                  {{ticket.celebrity.description}}
                </td>
              </tr>
              <tr>
                <td scope="row"><strong>Country</strong></td>
                <td v-if="Object.keys(ticket).length !== 0">
                  {{ticket.celebrity.country}}
                </td>
              </tr>
              </tbody>
            </table>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                dark
                color="blue darken-1"
                flat
                @click.native="ticketModal = false">Close
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-layout>
    </v-container>
    <v-snackbar
      :timeout="snackbarTimeout"
      :color="snackbarColor"
      :vertical="snackbarMode"
      v-model="snackbar"
      dark>
      {{ snackbarText }}
      <v-btn dark flat @click.native="snackbar = false">Close</v-btn>
    </v-snackbar>
  </v-content>
</template>

<script>
export default {
  data() {
    return {
      loading: false,
      search: "",
      ticketModal: false,
      ticket: {},
      snackbar: false,
      snackbarColor: "error",
      snackbarMode: "vertical",
      snackbarTimeout: 4000,
      snackbarText: ""
    };
  },
  methods: {
    findTicket () {
      if(this.search == undefined || this.search == "") {
        return
      }
      
      this.$store.dispatch('findTicket', this.search)
        .then(() => {
          this.ticket = this.$store.getters.ticket;
          this.ticketModal = true;
          this.loading = this.$store.getters.loading;
        })
        .catch(() => {
          this.snackbar = true;
          const error = this.$store.getters.error;
          switch (error.response.status) {
            case 404:
              this.snackbarText =
                "That ticket was not found!  Please try again.";
              break;
            case 500:
              this.snackbarText =
                "There was an error processing your request.  Please try again.";
              break;
            default:
              this.snackbarText = error.response.statusText;
              break;
          }
          this.loading = this.$store.getters.loading;
        })
    }
  }
};
</script>

<style scoped>
.truncate {
  width: 200px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

table {
  border: 1px solid #ccc;
  border-collapse: collapse;
  margin: 0;
  padding: 0;
  width: 100%;
  table-layout: fixed;
}

table caption {
  font-size: 1.5em;
  margin: 0.5em 0 0.75em;
}

table tr {
  background: #f8f8f8;
  border: 1px solid #ddd;
  padding: 0.35em;
}

table th,
table td {
  padding: 0.625em;
  text-align: center;
}

table th {
  font-size: 0.85em;
  letter-spacing: 0.1em;
  text-transform: uppercase;
}

@media screen and (max-width: 600px) {
  table {
    border: 0;
  }

  table caption {
    font-size: 1.3em;
  }

  table thead {
    border: none;
    clip: rect(0 0 0 0);
    height: 1px;
    margin: -1px;
    overflow: hidden;
    padding: 0;
    position: absolute;
    width: 1px;
  }

  table tr {
    border-bottom: 3px solid #ddd;
    display: block;
    margin-bottom: 0.625em;
  }

  table td {
    border-bottom: 1px solid #ddd;
    display: block;
    font-size: 0.8em;
    text-align: right;
  }

  table td:before {
    /*
      * aria-label has no advantage, it won't be read inside a table
      content: attr(aria-label);
      */
    content: attr(data-label);
    float: left;
    font-weight: bold;
    text-transform: uppercase;
  }

  table td:last-child {
    border-bottom: 0;
  }
}

.custom-loader {
  animation: loader 1s infinite;
  display: flex;
}

@-moz-keyframes loader {
  from {
    transform: rotate(0);
  }
  to {
    transform: rotate(360deg);
  }
}

@-webkit-keyframes loader {
  from {
    transform: rotate(0);
  }
  to {
    transform: rotate(360deg);
  }
}

@-o-keyframes loader {
  from {
    transform: rotate(0);
  }
  to {
    transform: rotate(360deg);
  }
}

@keyframes loader {
  from {
    transform: rotate(0);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>