<template>
    <div>
        <nav class="navbar sticky-top navbar-expand-lg navbar-light" id="mainNavbar">
            <router-link class="navbar-brand" to="/"><a>Games4Trade</a></router-link>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse " id="navbarSupportedContent">
                <ul class="navbar-nav mr-auto" >
                    <li class="nav-item">
                        <a class="nav-link"  v-if="isAuthenticated">Test</a>
                    </li>
                </ul>
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item" v-if="!isAuthenticated">
                        <router-link to="/signup" class="nav-link"><a>Utwórz konto</a></router-link>
                    </li>
                    <li class="nav-item" v-if="!isAuthenticated">
                        <router-link to="/login" class="nav-link"><a>Zaloguj</a></router-link>
                    </li>
                    <li class="nav-item" v-if="isAuthenticated" @click="logout">
                        <a class="nav-link" style="cursor: pointer">Wyloguj</a>
                    </li>
                </ul>
            </div>
        </nav>
    </div>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  name: 'Navbar',
  computed: {
    ...mapGetters(['isAuthenticated'])
  },
  methods: {
    logout () {
      this.$store.dispatch('logout')
    }
  }
}
</script>

<style scoped>
    #mainNavbar {
        background-color: #26bba6;
        box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
        -webkit-transform: translateZ(0);
    }
    #mainNavbar a, .navbar {
        color: white;
    }
</style>
