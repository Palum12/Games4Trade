<template>
    <div v-if="dataLoaded" class="row admin no-gutters ">
        <tabs :options="{ useUrlFragment: false }">
            <tab name="Mój profil" class="tabs-height">
                <h5>W tym miejscu możesz modyfikować swój opis widoczny dla innych użytkowników, oraz swoje dane.</h5>
                <my-profile :user-id="userId" @somethingChanged="onSomethingChanged"></my-profile>
            </tab>
            <tab name="Moje preferencje" class="tabs-height">
                <h5>W tym miejscu wybierz jakie gatunki gier oraz systemy Cię interesują.</h5>
                <h5>Kliknij przycisk na dole aby zapisać wszelkie zmiany</h5>
                <div class="row">
                    <div class="col-md-6 col-12">
                        <my-genres :user-id="userId"></my-genres>
                    </div>
                    <div class="col-md-6 col-12">
                        <my-systems :user-id="userId"></my-systems>
                    </div>
                </div>
            </tab>
            <tab name="Obserwowani użytkownicy" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać listę obserwowanych przez siebie użytkowników.</h5>
                <observed-users :user-id="userId" class="users"></observed-users>
            </tab>
            <tab name="Moje ogłoszenia" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać swoje ogłoszenia.</h5>
                <my-ads></my-ads>
            </tab>
        </tabs>
    </div>

</template>

<script>
import 'vue-tabs-component/docs/resources/tabs-component.css'
import ObservedUsers from '../components/users/ObservedUsers'
import MyProfile from '../components/users/MyProfile'
import MyGenres from '../components/users/MyGenres'
import MySystems from '../components/users/MySystems'
import MyAds from '../components/users/MyAdvertisements'
import mixins from '../mixins/mixins'

export default {
  name: 'UserPanel',
  components: {
    MyProfile,
    MyGenres,
    MySystems,
    MyAds,
    ObservedUsers
  },
  data () {
    return {
      dataLoaded: false,
      hasUnSavedChanges: false,
      userId: Number
    }
  },
  methods: {
    onSomethingChanged (value) {
      this.hasUnSavedChanges = value
    }
  },
  mounted () {
    var vm = this
    this.$store.dispatch('getUserId')
      .then(response => {
        vm.userId = response.data
        vm.dataLoaded = true
      })
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAuthenticated) {
        next()
      } else {
        next('/')
      }
    })
  },
  async beforeRouteLeave (to, from, next) {
    if (this.hasUnSavedChanges) {
      let vm = this
      await mixins.methods.confirmationLeaveDialog(vm)
        .then(() => next())
        .catch(() => {
          next(false)
        }
        )
    } else {
      next()
    }
  }
}
</script>

<style scoped>
    .tabs-component {
        width: 100vw;
        margin-top: 0;
        margin-bottom: 0;
        margin-left: 1vw;
        margin-right: 1vw;
    }

    .tabs-component >>> .tabs-component-panels{
        padding-top: 2em !important;
        padding-bottom: 2em !important;
    }
    .tabs-height {
        height: 73vh !important;
    }

    .users {
        min-height: 200px;
        height: 72vh;
        max-height: 100%;
    }
</style>
