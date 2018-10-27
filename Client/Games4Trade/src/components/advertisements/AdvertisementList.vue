<template>
  <div>
      <div class="inner" v-for="advertisement in advertisementList" :key="advertisement.id">
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
      var sh = document.getElementById('inner').scrollHeight
      var st = document.getElementById('inner').scrollTop
      var oh = document.getElementById('inner').offsetHeight
      if (sh - st - oh + 1 < 2) {
        this.$emit('scroll-ended')
      }
    }
  },
  mounted () {
    document.getElementById('inner').addEventListener('scroll', this.scrollEnded)
  }
}
</script>

<style scoped>
    #inner {
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
