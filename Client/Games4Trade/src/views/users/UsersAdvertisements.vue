<template>
<div>
{{userId}}
</div>
</template>

<script>
import AdvertisementList from '../../components/advertisements/AdvertisementList'
import axios from 'axios'
export default {
  name: 'UsersAdvertisements',
  data () {
    return {
      userId: null,
      isNextPage: true,
      nextPage: 1
    }
  },
  mounted () {
    this.userId = this.$route.params.id
  }
}
</script>

<style scoped>

</style>
