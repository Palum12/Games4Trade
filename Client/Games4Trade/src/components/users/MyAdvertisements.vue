<template>
    <div class="scrollable-ads">
        <advertisement-list
                :advertisement-list="advertisements"
                @scroll-ended="getMoreAdvertisements"
                ></advertisement-list>
    </div>
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
      page: 1,
      isNextPage: true
    }
  },
  methods: {
    getAdvertisements () {
      let vm = this
      this.$store.dispatch('setSpinnerLoading')
      axios.get(`/users/${vm.userId}/advertisements?page=1`)
        .then(response => {
          vm.$store.dispatch('unsetSpinnerLoading')
          vm.advertisements = response.data
          vm.nextPage = 2
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    getMoreAdvertisements () {
      if (this.isNextPage) {
        let vm = this
        axios.get(`/users/${vm.userId}/advertisements?page=${this.nextPage}`)
          .then(response => {
            vm.advertisements.push(...response.data)
            if (response.data.length === 0) {
              vm.isNextPage = false
            } else {
              vm.nextPage = vm.nextPage + 1
            }
          })
      }
    }
  },
  mounted () {
    this.getAdvertisements()
  }
}
</script>

<style scoped>
    .scrollable-ads {
        min-height: 200px;
        height: 70vh;
        max-height: 100%;
    }
</style>
