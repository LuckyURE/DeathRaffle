<template>
  <v-container
    fluid
    style="min-height: 0;"
    grid-list-lg>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card color="blue-grey darken-2" class="white--text">
          <v-container fluid grid-list-lg>
            <v-layout row>
              <v-flex xs12>
                <div>
                  <div class="headline">Whoah, what's a Death Raffle!?</div>
                  <div>
                    A Death Raffle is an on-going game where entrants place a bet on whether a currently 
                    living celebrity will die first in real life.</div>
                    </div>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card>
          </v-flex>
          <v-flex xs12>
            <v-card color="blue-grey darken-2" class="white--text">
              <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs12>
                    <div>
                      <div class="headline">You, and all those who play, must be mental right?</div>
                      <div>No, we never hope that someone will die.  However, it is a fact of life that we all die at
                        one time or another. We're just playing a light-hearted game of chance based on famous people
                        we know and love.
                      </div>
                    </div>
                          </v-flex>
                        </v-layout>
                      </v-container>
                    </v-card>
                  </v-flex>
                  <v-flex xs12>
                    <v-card color="blue-grey darken-2" class="white--text">
                      <v-container fluid grid-list-lg>
                        <v-layout row>
                          <v-flex xs12>
                            <div>
                              <div class="headline">How does it work?</div>
                              <div>
                                <p>
                                  All games are on-going and consist within the confines of a concept called a pool.
                                  Each pool contains a total of ten raffle tickets and remains open until all ten tickets are filled.
                                </p>
                                <p>
                                  Each ticket consists of a randomly selected famous person's name, age, short description, and the
                                  country in which they were born.
                                </p>
                                <p>
                                  Once all ten tickets have been assigned the pool will be locked and the game begins.
                                  The game continues until one of the famous people in the pool dies.
                                  Once confirmation has been made, by a game administrator, that a famous person within
                                  the pool has died the game is over.  The entrant assigned the raffle ticket that matches
                                  the newly deceased famous person is made the winner and a payout will be made.
                                </p>
                              </div>
                            </div>
                          </v-flex>
                        </v-layout>
                      </v-container>
                    </v-card>
                  </v-flex>

      <v-flex xs12>
        <v-card color="blue-grey darken-2" class="white--text">
          <v-container fluid grid-list-lg>
            <v-layout row>
              <v-flex xs12>
                <div>
                  <div class="headline">How do I get a raffle ticket for a chance to win?</div>
                  <div>
                    <p>
                      Our system makes use of the <a href="https://www.stellar.org" target="_blank">Stellar Payment Network</a>.  
                      You must first create and fund your Stellar wallet and then send the equivalent of $10 USD 
                      worth of Lumens (XLM) to our account.  See the <router-link to="/">home page</router-link> for details.
                    </p>
                    <p>
                      If you don't already have a Stellar wallet, just head over to the Stellar website and you'll
                      find lots of different wallets and exchangest to choose from
                      <a href="https://www.stellar.org/lumens/wallets/" target="_blank">here</a>.
                    </p>
                    <p>
                      Once a payment has been received our system will randomly select a famous person, assign it to
                      a raffle ticket and pool, and then send you a payment with a memo indicating your ticket number.
                      If you'd like to know what pool you are in and who your famous person is, just return to our site
                      and click on the Ticket Search menu item at the top of the page and enter your ticket number.
                    </p>
                  </div>
                </div>
              </v-flex>
            </v-layout>
          </v-container>
        </v-card>
      </v-flex>
                </v-layout>
              </v-container>
            </template>

            <script>
                export default {
                    name: "index"
                }
            </script>

            <style scoped>

            </style>