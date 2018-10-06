<template>
    <div class="row admin no-gutters ">
        <tabs :options="{ useUrlFragment: false }">
            <tab name="Gatunki i Systemy" class="tabs-height">
                <div class="row">
                    <div class="col-md-6 col-12">
                        <genres></genres>
                    </div>
                    <div class="col-md-6 col-12">
                        <systems></systems>
                    </div>
                </div>
            </tab>
            <tab name="Ogłoszenia dla społeczności" class="tabs-height">
                <announcements-list class="announcements mb-1"></announcements-list>
                <button class="btn btn-success btn-block" @click="addNewAnnouncement">Dodaj nowe ogłoszenie !</button>
            </tab>
        </tabs>
    </div>

</template>

<script>
import genres from '../components/admin/Genres'
import systems from '../components/admin/Systems'
import announcementsList from '../components/announcements/AnnouncementsList'
import 'vue-tabs-component/docs/resources/tabs-component.css'
export default {
  name: 'AdminPanel',
  components: {
    genres,
    systems,
    announcementsList
  },
  methods: {
    addNewAnnouncement () {
      this.$router.push({name: 'AddAnnouncement'})
    }
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAdmin) {
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
