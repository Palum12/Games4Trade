<template>
    <div>
        <h1>Witaj w serwisie Games4Trade!</h1>
    </div>
</template>

<script>
import axios from 'axios'
export default {
  name: 'HomePageAdvertisements',
  data () {
    return {
      areRecommended: false,
      nextPage: 1,
      isNextPage: true
    }
  },
  mounted () {
    if (this.$store.getters.isAuthenticated) {
      console.log('test')
    } else {
      this.areRecommended = true
    }
  }
}
</script>

<style scoped>

</style>
