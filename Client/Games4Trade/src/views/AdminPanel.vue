<template>
    <div class="row admin no-gutters">
        <div class="col-6">
            <genres></genres>
        </div>
        <div class="col-6">
            <h1>TEST2</h1>
        </div>
    </div>
</template>

<script>
import genres from '../components/admin/Genres'
export default {
  name: 'AdminPanel',
  components: {
    genres
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
    .admin {
        min-height: 100%;
    }
</style>
