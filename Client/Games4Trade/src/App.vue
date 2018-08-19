<template>
  <div id="app">
      <navbar/>
      <div style="width: 800px; height: 1000px; background-color: red;">
          <p>Dupa</p>
      </div>
      <router-view/>
  </div>
</template>

<script>
import Navbar from './components/Navbar'
export default {
  components: {Navbar}
}
</script>

<style lang="scss">
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  background-color: whitesmoke;
}

</style>
