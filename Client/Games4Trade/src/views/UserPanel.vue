<template>
    <div class="row admin no-gutters ">
        <tabs :options="{ useUrlFragment: false }">
            <tab name="Mój profil" class="tabs-height">
                <h5>W tym miejscu możesz modyfikować swój opis widoczny dla innych użytkowników, oraz swoje dane.</h5>
            </tab>
            <tab name="Moje preferencje" class="tabs-height">
                <h5>W tym miejscu wybierz jakie gatunki gier oraz systemy Cię interesują.</h5>
            </tab>
            <tab name="Obserwowani użytkownicy" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać listę obserwowanych przez siebie użytkowników.</h5>
            </tab>
            <tab name="Moje ogłoszenia" class="tabs-height">
                <h5>W tym miejscu możesz przeglądać swoje ogłoszenia.</h5>
            </tab>
        </tabs>
    </div>

</template>

<script>
import 'vue-tabs-component/docs/resources/tabs-component.css'
import ObservedUsers from '../components/users/ObservedUsers'
import MyProfile from '../components/users/MyProfile'
import MyPreferences from '../components/users/MyPreferences'
import MyAnnouncements from '../components/users/MyAnnouncements'

export default {
  name: 'UserPanel',
  components: {
    MyProfile
  },
  methods: {},
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAuthenticated) {
        next()
      } else {
        next('/')
      }
    })
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
    .announcements {
        min-height: 200px;
        height: 70vh;
        max-height: 90%;
    }
    .tabs-height {
        height: 73vh !important;
    }
</style>
