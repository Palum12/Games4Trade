<template>
    <div class="inner">
            <div class="list-group" v-for="announcement in announcements" :key="announcement.id">
                <router-link :to="'/announcement/'+announcement.id" tag="span">
                    <a class="list-group-item list-group-item-action flex-column align-items-start mb-1">
                        <div class="d-flex w-100 justify-content-between">
                            <h5 class="mb-1">{{announcement.title}}</h5>
                            <small>{{announcement.dateCreated.substring(0,10)}}</small>
                        </div>
                        <p class="mb-1">{{shortenString(announcement.content)}}</p>
                        <small>{{announcement.author}}</small>
                    </a>
                </router-link>
            </div>
    </div>
</template>

<script>
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
    }
  },
  mounted () {
    let vm = this
    this.$store.dispatch('getAnnouncements')
      .then(response => {
        vm.announcements = response.data
      })
  }
}
</script>

<style scoped>
.inner {
    overflow: hidden;
    overflow-y: auto;
}
</style>
