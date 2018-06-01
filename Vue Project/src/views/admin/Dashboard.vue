<template>
  <v-container grid-list-md text-xs-center>
    <v-layout row wrap>
      <v-flex xs5>
        <v-card light color="secondary">
          <v-card-title>
      Celebrities ({{ items.length }})
      <v-spacer></v-spacer>
      <v-text-field
        append-icon="search"
        label="Search"
        single-line
        hide-details
        v-model="search"
      ></v-text-field>
    </v-card-title>
    <v-data-table
      :headers="headers"
      :items="items"
      :search="search"
    >
      <template slot="items" slot-scope="props">
        <td class="text-xs-left">
          <v-tooltip right lazy max-width="500">
            <span slot="activator">{{ props.item.firstName }}</span>
            <span>{{ props.item.birthYear}}: {{ props.item.description}}</span>
          </v-tooltip>
        </td>
        <td class="text-xs-left">{{ props.item.lastName }}</td>
        <td class="text-xs-left">
          <v-btn flat icon color="red" @click="removeCeleb(props.item)">
            <v-icon>delete</v-icon>
          </v-btn>
        <td class="text-xs-left">
          <v-btn flat icon color="green" @click="markDead(props.item)">
            <v-icon>power_settings_new</v-icon>
          </v-btn>
        </td>
      </template>
      <v-alert slot="no-results" :value="true" color="error" icon="warning">
        Your search for "{{ search }}" found no results.
      </v-alert>
    </v-data-table>
        </v-card>
      </v-flex>
      <v-flex xs7>
        <v-card light color="secondary">
          <add-celeb-form></add-celeb-form>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import AddCelebForm from '@/components/AddCelebForm.vue'

  export default {
    components: {
      AddCelebForm
    },
    mounted () {
        this.$store.dispatch('getLivingCelebs')
    },
    data () {
      return {
        search: '',
        headers: [
          {
            text: 'Celebrity',
            align: 'left',
            sortable: false,
            value: 'firstName'
          },
          { text: 'Last Name', value: 'lastName' },
          { text: 'Delete', value: '' },
          { text: 'Mark Dead', value: '' }
        ]
      }
    },
    methods: {
      removeCeleb(celeb) {
        this.$store.dispatch('removeCeleb', celeb)
            .then(() => {
                this.color = "success"
                this.text = "Celebrity successfully removed!"
                this.snackbar = true;
            })
            .catch(() => {
                this.color = "error"
                this.text = this.$store.getters.error.message
                this.snackbar = true;
            })
        },
        markDead(celeb) {
          this.$store.dispatch('markDead', celeb)
            .then(() => {
                this.color = "success"
                this.text = "Celebrity successfully marked as dead!"
                this.snackbar = true;
            })
            .catch(() => {
                this.color = "error"
                this.text = this.$store.getters.error.message
                this.snackbar = true;
            })
        }
    },
    computed: {
      items () {
        return this.$store.getters.livingCelebs
      }
    }
  }
</script>