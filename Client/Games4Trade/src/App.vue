<template>
  <div id="app">
      <navbar style="margin-bottom: 2vh"/>
      <router-view/>
  </div>
</template>

<script>
import Navbar from './components/Navbar'
export default {
  components: {Navbar},
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
    min-height: 100vh;
}

</style>
