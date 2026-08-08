<template>
    <div v-if="dataLoaded" class="row admin no-gutters ">
        <Tabs>
            <TabPanel title="Mój profil" class="tabs-height">
                <h5>W tym miejscu możesz modyfikować swój opis widoczny dla innych użytkowników, oraz swoje dane.</h5>
                <my-profile :user-id="userId" @somethingChanged="onSomethingChanged"></my-profile>
            </TabPanel>
            <TabPanel title="Moje preferencje" class="tabs-height">
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
            </TabPanel>
            <TabPanel title="Obserwowani użytkownicy" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać listę obserwowanych przez siebie użytkowników.<br>
                    Ogłoszenia obserwowanych użytkowników będą się pojawiały w rekomandowanych dla Ciebie ogłoszeniach
                    na stronie głównej.</h5>
                <observed-users :user-id="userId" class="scrollable"></observed-users>
            </TabPanel>
            <TabPanel title="Moje ogłoszenia" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać swoje ogłoszenia.</h5>
                <my-ads :user-id="userId"></my-ads>
            </TabPanel>
        </Tabs>
    </div>

</template>

<script>
import Tabs from '../../components/ui/Tabs.vue'
import TabPanel from '../../components/ui/TabPanel.vue'
import ObservedUsers from '../../components/users/ObservedUsers.vue'
import MyProfile from '../../components/users/MyProfile.vue'
import MyGenres from '../../components/users/MyGenres.vue'
import MySystems from '../../components/users/MySystems.vue'
import MyAds from '../../components/users/MyAdvertisements.vue'
import mixins from '../../mixins/mixins'

export default {
  name: 'UserPanel',
  components: {
    Tabs,
    TabPanel,
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
      userId: null
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
    .tabs-height {
        min-height: 400px;
    }

    .scrollable {
        min-height: 200px;
        height: 60vh;
        max-height: 100%;
        overflow-y: auto;
    }
</style>
