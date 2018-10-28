<template>
  <div id="innerList">
      <div v-for="advertisement in advertisementList" :key="advertisement.id">
          <advertisement-header :advertisement="advertisement"></advertisement-header>
      </div>
  </div>
</template>

<script>
import AdvertisementHeader from './AdvertisementHeader'
export default {
  name: 'AdvertisementList',
  components: {
    AdvertisementHeader
  },
  props: ['advertisementList'],
  methods: {
    scrollEnded () {
      let sh = document.getElementById('innerList').scrollHeight
      let st = document.getElementById('innerList').scrollTop
      let oh = document.getElementById('innerList').offsetHeight
      if (sh - st - oh + 1 < 2) {
        this.$emit('scroll-ended')
      }
    }
  },
  mounted () {
    document.getElementById('innerList').addEventListener('scroll', this.scrollEnded)
  }
}
</script>

<style scoped>
    #innerList {
        height: inherit;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
