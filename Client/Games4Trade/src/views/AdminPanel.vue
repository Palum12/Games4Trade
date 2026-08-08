<template>
    <div class="row admin no-gutters ">
        <Tabs>
            <TabPanel title="Gatunki i Systemy" class="tabs-height">
                <div class="row">
                    <div class="col-md-6 col-12">
                        <genres></genres>
                    </div>
                    <div class="col-md-6 col-12">
                        <systems></systems>
                    </div>
                </div>
            </TabPanel>
            <TabPanel title="Ogłoszenia dla społeczności" class="tabs-height">
                <announcements-list class="announcements mb-1"></announcements-list>
                <button class="btn btn-success w-100" @click="addNewAnnouncement">Dodaj nowe ogłoszenie !</button>
            </TabPanel>
        </Tabs>
    </div>

</template>

<script>
import Tabs from '../components/ui/Tabs.vue'
import TabPanel from '../components/ui/TabPanel.vue'
import genres from '../components/admin/Genres.vue'
import systems from '../components/admin/Systems.vue'
import announcementsList from '../components/announcements/AnnouncementsList.vue'
export default {
  name: 'AdminPanel',
  components: {
    Tabs,
    TabPanel,
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
    .announcements {
        min-height: 200px;
        height: 70vh;
        max-height: 90%;
    }
    .tabs-height {
        min-height: 400px;
    }
</style>
