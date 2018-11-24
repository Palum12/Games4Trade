<template>
    <div v-if="hasDataLoaded" class="no-gutters profile">
        <div class="row">
            <div class="col-3">
                <img :src="`http://localhost:5000/api/users/${user.id}/photo`">
                <button
                        class="btn btn-success mt-2"
                        v-if="showButtonAddToObserved"
                        @click="startObserving">Zacznij obserwować
                </button>
                <button
                        class="btn btn-warning mt-2"
                        v-if="showButtonDeleteFromObserved"
                        @click="stopObserving">Przestań obserwować
                </button>
            </div>
            <div class="col-8 ml-3">
                <h3>{{ucFirst(user.login)}}</h3>
                <p>
                    {{prepareGenres(user.likedGenres)}}<br>
                    {{prepareSystems(user.interestingSystems)}}
                </p>
                <h5>Opis: </h5>
                <div style="white-space: pre-line;">
                    {{prepareDescription(user.description)}}
                </div>
            </div>
        </div>
        <div class="mt-4">
            <h4>Najnowsze ogłoszenia użytkownika: <router-link :to="`${user.id}/advertisements`" tag="a">tutaj</router-link></h4>
        </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import { mapGetters } from 'vuex'
import axios from 'axios'
export default {
  name: 'UserProfile',
  data () {
    return {
      noDescriptionMessage: 'Wygląda na to, że ten użytkonik nie posiada jeszcze opisu!',
      noGenresMessage: 'Ten użytkonik nie polubił żadnych gatunków!',
      noSystemsMessage: 'Tego użytkownika nie interesują żadne systemy!',
      hasDataLoaded: false,
      user: null,
      currentUserId: null
    }
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'getCurrentLogin']),
    showButtonAddToObserved () {
      return this.isAuthenticated && !this.user.isUserObserved && (this.user.login !== this.getCurrentLogin)
    },
    showButtonDeleteFromObserved () {
      return this.isAuthenticated && this.user.isUserObserved && (this.user.login !== this.getCurrentLogin)
    }
  },
  methods: {
    ucFirst (string) {
      return string.charAt(0).toUpperCase() + string.slice(1)
    },
    prepareDescription (descrption) {
      if (descrption == null) {
        return this.noDescriptionMessage
      }
      return descrption
    },
    prepareGenres (genres) {
      if (genres == null || genres.length === 0) {
        return this.noGenresMessage
      }
      return 'Lubię gatunki: ' + genres.join(', ')
    },
    prepareSystems (systems) {
      if (systems == null || systems.length === 0) {
        return this.noSystemsMessage
      }
      return 'Interesują mnie: ' + systems.join(', ')
    },
    stopObserving () {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`/users/${vm.currentUserId}/observed/`,
            {
              data:
                {
                  ObservingUserId: vm.currentUserId,
                  ObservedUserId: vm.user.id
                }
            })
            .then(() => {
              mixins.methods.simpleSuccessPopUp(vm)
              vm.user.isUserObserved = false
            })
            .catch(() => {
              mixins.methods.errorPopUp(vm)
            })
        })
    },
    startObserving () {
      let vm = this
      axios.post(`/users/${vm.currentUserId}/observed/`,
        {
          ObservingUserId: vm.currentUserId,
          ObservedUserId: vm.user.id
        }
      )
        .then(() => {
          mixins.methods.simpleSuccessPopUp(vm)
          vm.user.isUserObserved = true
        })
        .catch(() => {
          mixins.methods.errorPopUp(vm)
        })
    },
    async getData () {
      let id = this.$route.params.id
      let vm = this
      await this.$store.dispatch('getUser', id)
        .then(response => {
          vm.user = response.data
          vm.hasDataLoaded = true
        })
        .catch(error => {
          if (error.response.status === 404) {
            vm.$router.go(-1)
          }
        })
      this.$store.dispatch('getUserId')
        .then(response => {
          vm.currentUserId = response.data
        })
    }
  },
  async mounted () {
    await this.getData()
  }
}
</script>

<style scoped>
    img {
        width: 17vw;
        height: 17vw;
        object-fit: contain;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
    .profile {
        margin: 0 2vw;
        width: 90vw;
        height: 90vh;
        text-justify: newspaper;
    }
</style>
