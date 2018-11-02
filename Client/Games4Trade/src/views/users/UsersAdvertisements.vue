<template>
<div class="row no gutters user-ads">
    <div v-if="hasDataLoaded" class="col-12 mx-2 scrollable-ads">
        <h3>Ogłoszenia użytkownika {{user.login}}</h3>
        <advertisement-list
                :advertisement-list="advertisements"
                @scroll-ended="getMoreAdvertisements"></advertisement-list>
    </div>
</div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
import axios from 'axios'
export default {
  name: 'UsersAdvertisements',
  components: {
    AdvertisementList
  },
  data () {
    return {
      userId: null,
      user: null,
      hasDataLoaded: false,
      advertisements: [],
      isNextPage: true,
      page: 1
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
    this.userId = this.$route.params.id
    this.getAdvertisements()
    let vm = this
    this.$store.dispatch('getUser', vm.userId)
      .then(response => {
        vm.user = response.data
        vm.hasDataLoaded = true
      })
      .catch(error => {
        if (error.response.status === 404) {
          vm.$router.go(-1)
        }
      })
  }
}
</script>

<style scoped>
    .scrollable-ads {
        min-height: 200px;
        height: 80vh;
        max-height: 100%;
    }
    .user-ads {
        width: 100vw;
    }
</style>
