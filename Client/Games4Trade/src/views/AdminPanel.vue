<template>
    <div class="row admin no-gutters">
        <div class="col-md-6 col-12">
            <genres></genres>
        </div>
        <div class="col-md-6 col-12">
            <systems></systems>
        </div>
    </div>
</template>

<script>
import genres from '../components/admin/Genres'
import systems from '../components/admin/Systems'
export default {
  name: 'AdminPanel',
  components: {
    genres,
    systems
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
