<template>
  <v-form v-model="valid" ref="form" lazy-validation>
    <v-container grid-list-xl fluid>
        <v-layout wrap>
            <v-flex xs12 sm6>
                <v-text-field
                label="Title"
                v-model="celeb.title"
                :rules="titleRules"
                :counter="20"
                autofocus
                ></v-text-field>
            </v-flex>
            <v-flex xs12 sm6>
                <v-text-field
                label="First Name"
                v-model="celeb.firstName"
                :rules="nameRules"
                :counter="30"
                required
                ></v-text-field>
            </v-flex>
            <v-flex xs12 sm6>
                <v-text-field
                label="Middle Name"
                v-model="celeb.middleName"
                :rules="notRequiredNameRules"
                :counter="30"
                ></v-text-field>
            </v-flex>
            <v-flex xs12 sm6>
                <v-text-field
                label="Last Name"
                v-model="celeb.lastName"
                :rules="nameRules"
                :counter="30"
                required
                ></v-text-field>
            </v-flex>
            <v-flex xs12 sm6>
                <v-text-field
                label="Suffix"
                v-model="celeb.suffix"
                :rules="suffixRules"
                :counter="10"
                ></v-text-field>
            </v-flex>
            <v-flex xs12 sm6>
                <v-text-field
                    label="Birth Year"
                    required
                    :rules="birthYearRules"
                    v-model="celeb.birthYear"
                    ref="birthYear"
                    placeholder="1970"
                ></v-text-field>
            </v-flex>
            <v-flex xs12>
                <v-text-field
                    name="description"
                    label="Description"
                    v-model="celeb.description"
                    :rules="descriptionRules"
                    :counter="150"
                    textarea
                    required
                ></v-text-field>
            </v-flex>
            <v-flex xs12>
                <v-select
                    autocomplete
                    label="Country"
                    placeholder="Select..."
                    :rules="[() => !!celeb.country || 'This field is required']"
                    :items="countries"
                    v-model="celeb.country"
                    ref="country"
                    required
                ></v-select>
            </v-flex>
        </v-layout>
    </v-container>

    <v-btn
      @click="submit"
      :disabled="!valid"
    >
      Add Celebrity
    </v-btn>
    <v-btn @click="clear">Clear</v-btn>
    <v-snackbar
      :timeout=timeout
      :color=color
      v-model="snackbar"
    >
      {{ text }}
      <v-btn dark flat @click.native="snackbar = false">Close</v-btn>
    </v-snackbar>
  </v-form>
</template>

<script>
export default {
    data: () => ({
        countries: [
        "Afghanistan",
        "Albania",
        "Algeria",
        "Andorra",
        "Angola",
        "Anguilla",
        "Antigua &amp; Barbuda",
        "Argentina",
        "Armenia",
        "Aruba",
        "Australia",
        "Austria",
        "Azerbaijan",
        "Bahamas",
        "Bahrain",
        "Bangladesh",
        "Barbados",
        "Belarus",
        "Belgium",
        "Belize",
        "Benin",
        "Bermuda",
        "Bhutan",
        "Bolivia",
        "Bosnia &amp; Herzegovina",
        "Botswana",
        "Brazil",
        "British Virgin Islands",
        "Brunei",
        "Bulgaria",
        "Burkina Faso",
        "Burundi",
        "Cambodia",
        "Cameroon",
        "Cape Verde",
        "Cayman Islands",
        "Chad",
        "Chile",
        "China",
        "Colombia",
        "Congo",
        "Cook Islands",
        "Costa Rica",
        "Cote D Ivoire",
        "Croatia",
        "Cruise Ship",
        "Cuba",
        "Cyprus",
        "Czech Republic",
        "Denmark",
        "Djibouti",
        "Dominica",
        "Dominican Republic",
        "Ecuador",
        "Egypt",
        "El Salvador",
        "Equatorial Guinea",
        "Estonia",
        "Ethiopia",
        "Falkland Islands",
        "Faroe Islands",
        "Fiji",
        "Finland",
        "France",
        "French Polynesia",
        "French West Indies",
        "Gabon",
        "Gambia",
        "Georgia",
        "Germany",
        "Ghana",
        "Gibraltar",
        "Greece",
        "Greenland",
        "Grenada",
        "Guam",
        "Guatemala",
        "Guernsey",
        "Guinea",
        "Guinea Bissau",
        "Guyana",
        "Haiti",
        "Honduras",
        "Hong Kong",
        "Hungary",
        "Iceland",
        "India",
        "Indonesia",
        "Iran",
        "Iraq",
        "Ireland",
        "Isle of Man",
        "Israel",
        "Italy",
        "Jamaica",
        "Japan",
        "Jersey",
        "Jordan",
        "Kazakhstan",
        "Kenya",
        "Kuwait",
        "Kyrgyz Republic",
        "Laos",
        "Latvia",
        "Lebanon",
        "Lesotho",
        "Liberia",
        "Libya",
        "Liechtenstein",
        "Lithuania",
        "Luxembourg",
        "Macau",
        "Macedonia",
        "Madagascar",
        "Malawi",
        "Malaysia",
        "Maldives",
        "Mali",
        "Malta",
        "Mauritania",
        "Mauritius",
        "Mexico",
        "Moldova",
        "Monaco",
        "Mongolia",
        "Montenegro",
        "Montserrat",
        "Morocco",
        "Mozambique",
        "Namibia",
        "Nepal",
        "Netherlands",
        "Netherlands Antilles",
        "New Caledonia",
        "New Zealand",
        "Nicaragua",
        "Niger",
        "Nigeria",
        "Norway",
        "Oman",
        "Pakistan",
        "Palestine",
        "Panama",
        "Papua New Guinea",
        "Paraguay",
        "Peru",
        "Philippines",
        "Poland",
        "Portugal",
        "Puerto Rico",
        "Qatar",
        "Reunion",
        "Romania",
        "Russia",
        "Rwanda",
        "Saint Pierre &amp; Miquelon",
        "Samoa",
        "San Marino",
        "Satellite",
        "Saudi Arabia",
        "Senegal",
        "Serbia",
        "Seychelles",
        "Sierra Leone",
        "Singapore",
        "Slovakia",
        "Slovenia",
        "South Africa",
        "South Korea",
        "Spain",
        "Sri Lanka",
        "St Kitts &amp; Nevis",
        "St Lucia",
        "St Vincent",
        "St. Lucia",
        "Sudan",
        "Suriname",
        "Swaziland",
        "Sweden",
        "Switzerland",
        "Syria",
        "Taiwan",
        "Tajikistan",
        "Tanzania",
        "Thailand",
        "Timor L'Este",
        "Togo",
        "Tonga",
        "Trinidad &amp; Tobago",
        "Tunisia",
        "Turkey",
        "Turkmenistan",
        "Turks &amp; Caicos",
        "Uganda",
        "Ukraine",
        "United Arab Emirates",
        "United Kingdom",
        "United States",
        "Uruguay",
        "Uzbekistan",
        "Venezuela",
        "Vietnam",
        "Virgin Islands (US)",
        "Yemen",
        "Zambia",
        "Zimbabwe"
        ],
        valid: true,
        celeb: {
        title: "",
        firstName: "",
        middleName: "",
        lastName: "",
        suffix: "",
        birthYear: null,
        description: "",
        country: ""
        },
        titleRules: [
        v => v !== null ? v.length <= 20 || "Title must be less than 20 characters" : true
        ],
        nameRules: [
        v => !!v || "Name is required",
        v => (v && v.length <= 30) || "Name must be less than 30 characters"
        ],
        notRequiredNameRules: [
        v => v !== null ? v.length <= 30 || "Name must be less than 30 characters" : true
        ],
        suffixRules: [
        v => v !== null ? v.length <= 10 || "Suffix must be less than 10 characters" : true
        ],
        birthYearRules: [
        v => !!v || "Birth Year is required",
        v => (v && !isNaN(parseFloat(v)) && isFinite(v)) || "Birth Year must be a number",
        v =>
            (v && v.length == 4) ||
            "Age must be a four digit number"
        ],
        descriptionRules: [
        v => !!v || "Description is required",
        v => (v && v.length <= 150) || "Name must be less than 150 characters"
        ],
        snackbar: false,
        timeout: 6000,
        color: '',
        text: ''
    }),

    methods: {
        submit() {
            if (this.$refs.form.validate()) {
                this.$store.dispatch('addCeleb', this.celeb)
                    .then(() => {
                        this.$refs.form.reset();
                        this.color = "success"
                        this.text = "Celebrity successfully added!"
                        this.snackbar = true;
                    })
                    .catch(() => {
                        this.color = "error"
                        this.text = this.$store.getters.error.response.data
                        this.snackbar = true;
                    })
            }
        },
        clear() {
        this.$refs.form.reset();
        }
    },

    computed: {
        errors () {
            return this.$store.getters.error
        }
    }
};
</script>