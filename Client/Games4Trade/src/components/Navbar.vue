<template>
    <div>
        <nav class="navbar sticky-top navbar-expand-lg navbar-light " id="mainNavbar">
            <router-link class="navbar-brand" to="/"><a>Games4Trade</a></router-link>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse " id="navbarSupportedContent">
                <ul class="navbar-nav mr-auto" >
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/advertisements/add" class="nav-link"><a>Dodaj ogłoszenie</a></router-link>
                    </li>
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/messages" class="nav-link"><a>Wiadomości</a></router-link>
                    </li>
                    <li class="nav-item">
                            <form class="form-inline">
                                <input
                                        class="form-control ml-5 mr-sm-2"
                                        type="search"
                                        placeholder="Szukaj ogłoszeń"
                                        aria-label="Search">
                                <button class="btn btn-outline-light my-2 my-sm-0" type="submit">Szukaj</button>
                            </form>
                    </li>
                </ul>
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/userPanel" class="nav-link"><a>Panel użytkownika</a></router-link>
                    </li>
                    <li class="nav-item" v-if="isAdmin">
                        <router-link to="/admin" class="nav-link"><a>Panel administratora</a></router-link>
                    </li>
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
    ...mapGetters(['isAuthenticated', 'isAdmin'])
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
