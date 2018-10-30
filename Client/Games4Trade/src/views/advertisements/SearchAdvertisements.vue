<template>
<div class="no-gutters">
    <p>Test szukajki: {{earlyQuery}}</p>
</div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
export default {
  name: 'SearchAdvertisement',
  components: {
    AdvertisementList
  },
  data () {
    return {
      advertisements: [],
      isNextPage: true,
      nextPage: 1,
      earlyQuery: ''
    }
  },
  mounted () {
    this.earlyQuery = this.$route.params.text
  }
}
</script>

<style scoped>

</style>
