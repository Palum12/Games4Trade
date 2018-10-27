<template>
    <advertisement-list :advertisement-list="advertisements" @scroll-ended="getMoreAdvertisements"></advertisement-list>
</template>

<script>
import axios from 'axios'
import AdvertisementList from '../advertisements/AdvertisementList'
export default {
  name: 'MyAdvertisements',
  props: {
    userId: Number
  },
  components: {
    AdvertisementList
  },
  data () {
    return {
      advertisements: [],
      page: 0,
      isNextPage: true
    }
  },
  methods: {
    getAnnouncements () {
      let vm = this
      this.$store.dispatch('setSpinnerLoading')
      axios.get(`/users/${vm.userId}/advertisements?page=1`)
        .then(response => {
          vm.$store.dispatch('unsetSpinnerLoading')
          vm.advertisements = response.data
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    getMoreAdvertisements () {
      console.log('Im done')
    }
  },
  mounted () {
    this.getAnnouncements()
  }
}
</script>

<style scoped>

</style>
