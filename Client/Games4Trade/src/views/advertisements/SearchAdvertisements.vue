<template>
    <div class="row no-gutters search">
            <div class="col-3">
                <advertisement-filter
                        :filter-criteria="filterCriteria"
                        @filter="search"
                        @clear="clear"
                ></advertisement-filter>
            </div>
            <div class="col-8 search">
                <div class="row mb-2">
                    <advertisement-sort
                            :sort-criteria="sortCriteria"
                            @filter="search"></advertisement-sort>
                </div>
                <div class="row scrollable-ads btn-block">
                    <advertisement-list class="scrollable-ads" :advertisement-list="advertisements"></advertisement-list>
                    <button
                            :disabled="!isNextPage"
                            type="button"
                            class="btn btn-primary btn-block"
                            @click="getNextPage">Pobierz więcej</button>
                </div>
            </div>
    </div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
import AdvertisementFilter from '../../components/advertisements/AdvertisementFilter'
import AdvertisementSort from '../../components/advertisements/AdvertisementSort'
import axios from 'axios'
import advetisements from '../../router/advetisements'
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
      this.nextPage = 2
      let url = 'advertisements?'
      if (this.query !== null && this.query !== '') {
        url = url + `search=${this.query}`
      }
      url = url + '&sort=' + this.sortCriteria.sort
      url = url + '&desc=' + this.sortCriteria.desc
      if (this.filterCriteria.stateId != null) {
        url = url + '&state=' + this.filterCriteria.stateId
      }
      axios.get(url)
        .then(response => {
          this.advertisements = response.data
        })
        .catch(() => {
          console.log('not found ?')
        })
    },
    getNextPage () {
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
      console.log('Im done!')
    }
  },
  mounted () {
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
        margin-left: 1vw !important;
        margin-right: 0 !important;
    }
</style>
