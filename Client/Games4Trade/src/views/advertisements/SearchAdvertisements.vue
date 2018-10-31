<template>
    <div class="row no-gutters search">
            <div class="col-3">
                <advertisement-filter :filter-criteria="filterCriteria"></advertisement-filter>
            </div>
            <div class="col-8 search">
                <div class="row mb-2">
                    <advertisement-sort :sort-criteria="sortCriteria"></advertisement-sort>
                </div>
                <div class="row btn-block">
                    <advertisement-list class="scrollable-ads" :advertisement-list="advertisements"></advertisement-list>
                    <button type="button" class="btn btn-primary btn-block" @click="getNextPage">Pobierz więcej</button>
                </div>
            </div>
    </div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
import AdvertisementFilter from '../../components/advertisements/AdvertisementFilter'
import AdvertisementSort from '../../components/advertisements/AdvertisementSort'
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
      earlyQuery: ''
    }
  },
  methods: {
    search () {
      this.advertisements.push({
        id: 1,
        title: 'No elo',
        dateCreated: '2008-03-03',
        price: 200,
        exchangeActive: true
      })
      this.advertisements.push({
        id: 2,
        title: 'No elo 2',
        dateCreated: '2008-03-03',
        price: 300,
        exchangeActive: false
      })
    },
    getNextPage () {
      console.log('Im done!')
    }
  },
  mounted () {
    this.earlyQuery = this.$route.params.text
    this.search()
  }
}
</script>

<style scoped>
    .scrollable-ads {
        min-height: 200px;
        height: 72vh;
        max-height: 100%;
    }
    .search {
        margin-left: 1vw !important;
        margin-right: 0 !important;
    }
</style>
