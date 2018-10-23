<template>
    <div class="no-gutters">
        test
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
export default {
  name: 'AddAdvertisement',
  props: ['userId'],
  data () {
    return {
      title: '',
      description: '',
      discriminator: 'Game',
      exchangeActive: false,
      price: Number,
      stateId: 0,
      systemId: 0,
      regionId: 0,
      genreId: 0,
      developer: String,
      accessoryManufacturer: String,
      accessoryModel: String
    }
  },
  methods: {
    test () {
      console.log(this.regions)
    }
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states'])
  }
}
</script>

<style scoped>

</style>
