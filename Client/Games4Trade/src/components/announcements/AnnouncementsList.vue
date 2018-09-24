<template>
    <div class="inner">
            <div class="list-group" v-for="announcement in announcements" :key="announcement.id">
                <router-link :to="'/announcement/'+announcement.id" tag="span">
                    <a class="list-group-item list-group-item-action flex-column align-items-start mb-1">
                        <div class="d-flex w-100 justify-content-between">
                            <h5 class="mb-1">{{announcement.title}}</h5>
                                <small>{{announcement.dateCreated.substring(0,10)
                                    + ' ' + announcement.dateCreated.substring(11,16)}}</small>
                        </div>
                        <p class="mb-1">{{shortenString(announcement.content)}}</p>
                        <small>{{announcement.author}}</small>
                    </a>
                    <button v-if="isAdminLook" class="btn btn-info mt-1 mb-3" @click="modify(announcement.id)">Modyfikuj</button>
                    <button v-if="isAdminLook" class="btn btn-danger mt-1 mb-3 mx-2" @click="remove(announcement.id)">X</button>
                </router-link>
            </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'AnnouncementsList',
  data () {
    return {
      announcements: []
    }
  },
  methods: {
    shortenString (content) {
      if (content.length > 97) {
        return content.substring(0, 97) + '...'
      }
      return content
    },
    remove (id) {
      this.$store.dispatch('setSpinnerLoading')
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`announcements/${id}`)
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getAnnouncementsFromServer()
            })
            .catch(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.errorPopUp(vm)
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    getAnnouncementsFromServer () {
      let vm = this
      this.$store.dispatch('getAnnouncements')
        .then(response => {
          vm.announcements = response.data
        })
    }
  },
  computed: {
    isAdminLook () {
      return this.$route.path.indexOf('admin') > 0
    }
  },
  mounted () {
    this.getAnnouncementsFromServer()
  }
}
</script>

<style scoped>
.inner {
    overflow: hidden;
    overflow-y: auto;
}
</style>
