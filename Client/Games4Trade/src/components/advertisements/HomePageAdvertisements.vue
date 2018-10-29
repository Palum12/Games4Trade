<template>
    <div>
        <h1>Witaj w serwisie Games4Trade!</h1>
        <div v-if="areRecommended">
            <h3>Oto ogłoszenia polecane dla Ciebie !</h3>
        </div>
        <div v-else>
            <h3>Oto najnowsze ogłoszenia !</h3>
        </div>
        <div class="scrollable-ads" >
            <advertisement-list
                    :advertisementList="advertisements"
                    @scroll-ended="getMoreAdvertisements"></advertisement-list>
        </div>
    </div>
</template>

<script>
import axios from 'axios'
import AdvertisementList from './AdvertisementList'
export default {
  name: 'HomePageAdvertisements',
  components: {
    AdvertisementList
  },
  data () {
    return {
      areRecommended: false,
      userId: null,
      nextPage: 1,
      pageSize: 10,
      isNextPage: true,
      advertisements: []
    }
  },
  computed: {
    urlToGet () {
      if (this.areRecommended) {
        return `/users/${this.userId}/advertisements/recommended?page=${this.nextPage}`
      } else {
        return `/advertisements/?page=${this.nextPage}&size=${this.pageSize}&desc=true`
      }
    }
  },
  methods: {
    getMoreAdvertisements () {
      if (this.isNextPage) {
        let vm = this
        axios.get(this.urlToGet)
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
    let vm = this
    if (this.$store.getters.isAuthenticated) {
      this.$store.dispatch('getUserId')
        .then(response => {
          vm.userId = response.data
        }).then(() => {
          axios.get(`/users/${vm.userId}/advertisements/recommended?page=1`)
            .then(response => {
              if (response.data.length > 0) {
                vm.areRecommended = true
                vm.advertisements = response.data
                vm.nextPage = vm.nextPage + 1
              } else {
                vm.areRecommended = false
                axios.get(`/advertisements/?page=${vm.nextPage}&size=${vm.pageSize}&desc=true`)
                  .then(response => {
                    vm.advertisements = response.data
                    vm.nextPage = vm.nextPage + 1
                  })
              }
            })
        })
    } else {
      vm.areRecommended = false
      axios.get(`/advertisements/?page=${vm.nextPage}&size=${vm.pageSize}&desc=true`)
        .then(response => {
          vm.advertisements = response.data
        })
    }
  }
}
</script>

<style scoped>
    .scrollable-ads {
        min-height: 200px;
        height: 72vh;
        max-height: 100%;
    }
</style>
