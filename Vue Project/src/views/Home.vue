<template>
  <v-container>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card class="primary">
          <v-container fluid>
            <v-layout row>
            <v-flex xs5 sm4 md3>
              <img class="papa" src="img/svg_graphics/danger-skull.svg" height="128" width="128">
            </v-flex>
            <v-flex xs7 sm8 md9>
              <v-card-title primary-title><h1>Welcome to Death Raffle - DEMO ONLY SITE</h1></v-card-title>
              <div>
                <p>You've heard of 50/50 drawings, you've seen the silent auctions for baskets full of junk, and you've probably even walked the cake walk.  Boring!</p>
                <p>It's time to change things up and join in on the fun game of Death Raffle.  No accounts, no usernames, no passwords, no hassle.</p>
                <p>WARNING:  THIS IS  DEMO ONLY SITE AND IS NOT LIVE - DO NOT SEND LIVE STELLAR (XML)</p>
              </div>
            </v-flex>
            </v-layout>
          </v-container>
        </v-card>
      </v-flex>
    </v-layout>

      <v-container grid-list-md text-xs-center>
        <v-layout>
          <v-flex xs12>
            <v-card light color="secondary">
              <v-card-title><h3>Get Your Raffle Ticket & Win!</h3></v-card-title>
              <v-card-text class="px-0">
                <v-flex xs12 md10 offset-md1>
                <p class="text-xs-left">
                  Send 50 Lumens (XLM) to GC4SFDYTS5IF7UJOJRSPAUW4MOTPTF67T67Q2YEH7NCKX4WEAYCLA4D4
                </p>
                <p class="text-xs-left">
                  Once received you will get a return payment with your raffle entry!  It's that easy!
                </p>
                </v-flex>
              </v-card-text>
            </v-card>
          </v-flex>
        </v-layout>
        <v-layout>
          <v-flex xs12>
            <v-card light color="secondary">
              <v-card-title><h3>Current Active Pools - Total: {{totalActivePools}}</h3></v-card-title>
              <v-data-table
                :headers="headers"
                :items="pools"
                :loading="tableLoading"
                rows-per-page-text="Pools per page:"
                class="elevation-1"
              >
                <v-progress-linear slot="progress" color="blue" indeterminate></v-progress-linear>
                <template slot="items" slot-scope="props">
                  <td class="text-xs-left">{{ props.item.poolId }}</td>
                  <td class="text-xs-left">{{ props.item.ticketCount }}</td>
                  <td class="text-xs-left">{{ props.item.createDate | formatDate }}</td>
                  <td class="text-xs-left">
                      <span v-if="props.item.gameStarted">
                        YES
                      </span>
                    <span v-else>NO</span>
                  </td>
                </template>
                <template slot="no-data">
                  <v-alert :value="true" color="info" icon="warning">
                    Sorry, no games are active at this time.
                  </v-alert>
                </template>
                <template slot="pageText" slot-scope="props">
                Pools {{ props.pageStart }} - {{ props.pageStop }} of {{ props.itemsLength }}
                </template>
              </v-data-table>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
  </v-container>
</template>

<script>
  export default {
    created() {
      this.$store.dispatch('getPools')
    },
    data() {
      return {
        headers: [
          {
            text: 'Pool #',
            align: 'left',
            sortable: false,
            value: 'poolId'
          },
          { text: '# of Tickets', value: 'ticketCount', sortable: false },
          { text: 'Created', value: 'createDate', sortable: false },
          { text: 'Started', value: 'gameStarted', sortable: false }
        ]
      }
    },
    computed: {
      pools () {
        return this.$store.getters.pools
      },
      tableLoading () {
        return this.$store.getters.loading
      },
      totalActivePools () {
        return this.$store.getters.pools.length
      }
    }
  }
</script>
