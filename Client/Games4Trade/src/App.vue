<template>
  <div id="app">
      <navbar style="margin-bottom: 2vh"/>
      <router-view/>
      <div v-show="isSpinnerLoading" class="overlay">
          <div class="loading-spinner">
              <loader :loading="isSpinnerLoading"></loader>
          </div>
      </div>
  </div>
</template>

<script>
import Navbar from './components/Navbar'
import Loader from 'vue-spinner/src/PacmanLoader'
import {mapGetters} from 'vuex'
export default {
  components: {Navbar, Loader},
  computed: {
    ...mapGetters(['isSpinnerLoading'])
  },
  created () {
    this.$store.dispatch('tryAutoLogin')
  }
}
</script>

<style lang="scss">
#app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    background-color: whitesmoke;
    min-height: 90vh;
}

.form{
    background-color: whitesmoke;
    border: solid 1px #26bba6;
}
.overlay {
    background: rgba(255, 255, 255, 0.4);
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
}
.loading-spinner {
    position: absolute;
    top: 50%;
    left: 50%;
    -webkit-transform: translateX(-50%) translateY(-50%);
    -moz-transform: translateX(-50%) translateY(-50%);
    transform: translateX(-50%) translateY(-50%);
}
</style>
