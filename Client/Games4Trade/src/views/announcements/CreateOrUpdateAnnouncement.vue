<template>
<div class="no-gutters">
   <p>{{announcement}}</p>
</div>
</template>

<script>
export default {
  name: 'CreateOrUpdateAnnouncement',
  data () {
    return {
      announcement: {
        title: '',
        content: '',
        author: '',
        dateCreated: ''
      }
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
  },
  mounted () {
    if (this.$route.params.id) {
      let vm = this
      let id = this.$route.params.id
      this.$store.dispatch('getAnnouncement', id)
        .then(response => {
          vm.announcement = response.data
          vm.announcement.dateCreated = vm.announcement.dateCreated.substring(0, 10) + ' ' +
            vm.announcement.dateCreated.substring(11, 16)
        })
    }
  }
}
</script>

<style scoped>

</style>
