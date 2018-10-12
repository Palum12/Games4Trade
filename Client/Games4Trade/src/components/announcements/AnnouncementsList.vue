<template>
    <div id="inner">
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
                    <button v-if="isAdminLook" class="btn btn-info mt-1 mb-2" @click="modify(announcement.id)">Modyfikuj</button>
                    <button v-if="isAdminLook" class="btn btn-danger mt-1 mb-2 mx-2" @click="remove(announcement.id)">X</button>
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
      announcements: [],
      isNextPage: true,
      nextPage: 2
    }
  },
  methods: {
    shortenString (content) {
      if (content.length > 97) {
        return content.substring(0, 97) + '...'
      }
      return content
    },
    modify (id) {
      this.$router.push({name: 'EditAnnouncement', params: {id: id}})
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
              vm.announcements = vm.announcements.filter(el => el.id !== id)
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
    getAnnouncements () {
      let vm = this
      this.$store.dispatch('setSpinnerLoading')
      axios.get('/announcements/page/1')
        .then(response => {
          vm.$store.dispatch('unsetSpinnerLoading')
          vm.announcements = response.data
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    getNextPageAnnouncements () {
      let vm = this
      axios.get(`/announcements/page/${this.nextPage}`)
        .then(response => {
          vm.announcements.push(...response.data)
          let data = response.data
          if (data.length === 0) {
            vm.isNextPage = false
          } else {
            vm.nextPage = vm.nextPage + 1
          }
        })
    },
    scrollEnded () {
      var sh = document.getElementById('inner').scrollHeight
      var st = document.getElementById('inner').scrollTop
      var oh = document.getElementById('inner').offsetHeight
      if (sh - st - oh + 1 < 2) {
        if (this.isNextPage) {
          this.getNextPageAnnouncements()
        }
      }
    }
  },
  computed: {
    isAdminLook () {
      return this.$route.path.indexOf('admin') > 0
    }
  },
  mounted () {
    this.getAnnouncements()
    document.getElementById('inner').addEventListener('scroll', this.scrollEnded)
  }
}
</script>

<style scoped>
#inner {
    overflow: hidden;
    overflow-y: auto;
    -webkit-transform: translate3d(0, 0, 0);
}
</style>
