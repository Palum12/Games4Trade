<template>
    <div class="row no-gutters">
            <div class="col-12 col-md-3 p-2">
                <advertisement-filter
                        :filter-criteria="filterCriteria"
                        @filter="search"
                        @clear="clear"
                ></advertisement-filter>
            </div>
            <div class="col-12 col-md-9 p-2">
                <div class="row no-gutters mb-2">
                    <advertisement-sort
                            :sort-criteria="sortCriteria"
                            @filter="search"></advertisement-sort>
                </div>
                <div v-if="advertisements.length > 0" class="row no-gutters scrollable-ads btn-block">
                    <advertisement-list class="scrollable-ads" :advertisement-list="advertisements"></advertisement-list>
                    <button
                            :disabled="!isNextPage"
                            type="button"
                            class="btn btn-primary btn-block"
                            @click="getNextPage">Pobierz więcej</button>
                </div>
                <div v-else>
                    <p>Niestety nie znaleziono pasujących ogłoszeń !</p>
                </div>
            </div>
    </div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
import AdvertisementFilter from '../../components/advertisements/AdvertisementFilter'
import AdvertisementSort from '../../components/advertisements/AdvertisementSort'
import axios from 'axios'
export default {
  name: 'SearchAdvertisement',
  components: {
    AdvertisementList,
    AdvertisementFilter,
    AdvertisementSort
  },
  data () {
    return {
      advertisements: [],
      sortCriteria: {
        sort: 'DateCreated',
        desc: true
      },
      filterCriteria: {
        type: null,
        stateId: null,
        systemIds: [],
        regionId: null,
        genreIds: []
      },
      isNextPage: true,
      nextPage: 1,
      pageSize: 10,
      query: ''
    }
  },
  watch: {
    '$route.params.text': function (newQuery) {
      this.query = newQuery
      this.search()
    }
  },
  methods: {
    clear () {
      this.filterCriteria = {
        type: null,
        stateId: null,
        systemIds: [],
        regionId: null,
        genreIds: []
      }
      this.sortCriteria = {
        sort: 'DateCreated',
        desc: true
      }
      this.search()
    },
    search () {
      this.isNextPage = true
      this.nextPage = 1
      this.advertisements = []
      this.getData()
    },
    getNextPage () {
      this.getData()
    },
    getData () {
      let url = 'advertisements?'
      if (this.query !== null && this.query !== '') {
        url = url + `search=${this.query}`
      }
      url = url + '&sort=' + this.sortCriteria.sort
      url = url + '&desc=' + this.sortCriteria.desc
      if (this.filterCriteria.stateId != null) {
        url = url + '&state=' + this.filterCriteria.stateId
      }
      if (this.filterCriteria.systemIds.length > 0) {
        this.filterCriteria.systemIds.forEach(x => {
          url = url + '&systems=' + x
        })
      }
      if (this.filterCriteria.type != null) {
        url = url + '&type=' + this.filterCriteria.type
        if (this.filterCriteria.type === 'game') {
          if (this.filterCriteria.genreIds.length > 0) {
            this.filterCriteria.genreIds.forEach(x => {
              url = url + '&genres=' + x
            })
          }
        }
        if (this.filterCriteria.type !== 'accessory') {
          if (this.filterCriteria.regionId != null) {
            url = url + '&region=' + this.filterCriteria.regionId
          }
        }
      }
      url = url + '&page=' + this.nextPage
      url = url + '&size=' + this.pageSize
      let vm = this
      console.log('url', url)
      axios.get(url)
        .then(response => {
          vm.advertisements.push(...response.data)
          if (response.data.length % vm.pageSize !== 0) {
            vm.isNextPage = false
          } else {
            vm.nextPage = vm.nextPage + 1
          }
        })
        .catch(() => {
          console.log('not found ?')
        })
    }
  },
  mounted () {
    console.log('mounted')
    this.query = this.$route.params.text
    this.search()
  }
}
</script>

<style scoped>
    .scrollable-ads {
        min-height: 200px;
        height: 80vh;
        max-height: 100%;
    }
    .search {
        padding-left: 1vw !important;
        padding-right: 0 !important;
    }
</style>
