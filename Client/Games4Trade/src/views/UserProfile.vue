<template>
    <div v-if="hasDataLoaded" class="no-gutters profile">
        <div class="row">
            <div class="col-3">
                <img :src="`http://localhost:5000/api/users/${user.id}/photo`">
                <button class="btn btn-success mt-2" v-if="showButtonAddToObserved">Zacznij obserwować
                </button>
                <button class="btn btn-warning mt-2" v-if="showButtonDeleteFromObserved">Przestań obserwować
                </button>
            </div>
            <div class="col-8 ml-3">
                <h3>{{ucFirst(user.login)}}</h3>
                <!-- tutaj gatunki i systemy-->
                <h5>Opis: </h5>
                <div style="white-space: pre-line;">
                    {{user.description}}
                </div>
            </div>
        </div>
        <div>
            <!--here will be users ads-->
        </div>
    </div>
</template>

<script>
import mixins from '../mixins/mixins'
import { mapGetters } from 'vuex'
export default {
  name: 'UserProfile',
  data () {
    return {
      hasDataLoaded: false,
      user: null
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
    }
  },
  async mounted () {
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
