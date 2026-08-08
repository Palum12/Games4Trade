<template>
    <div>
        <nav class="navbar sticky-top navbar-expand-lg navbar-light" id="mainNavbar">
            <router-link class="navbar-brand" :to="{ name: 'Home' }">Games4Trade</router-link>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/advertisements/add" class="nav-link" id="addAddvertisement">Dodaj ogłoszenie</router-link>
                    </li>
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/messages" class="nav-link">Wiadomości</router-link>
                    </li>
                    <li class="nav-item">
                            <form class="d-flex search-form" @submit.prevent="goToSearch">
                                <input
                                        class="form-control me-2"
                                        type="search"
                                        id="searchInput"
                                        placeholder="Szukaj ogłoszeń"
                                        aria-label="Search"
                                        v-model="searchText">
                                <button
                                        id="searchButton"
                                        class="btn btn-outline-light"
                                        type="submit">Szukaj</button>
                            </form>
                    </li>
                </ul>
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item" v-if="isAuthenticated">
                        <router-link to="/userPanel" class="nav-link">Panel użytkownika</router-link>
                    </li>
                    <li class="nav-item" v-if="isAdmin">
                        <router-link to="/admin" class="nav-link">Panel administratora</router-link>
                    </li>
                    <li class="nav-item" v-if="!isAuthenticated">
                        <router-link to="/signup" class="nav-link">Utwórz konto</router-link>
                    </li>
                    <li class="nav-item" v-if="!isAuthenticated">
                        <router-link to="/login" class="nav-link">Zaloguj</router-link>
                    </li>
                    <li class="nav-item" v-if="isAuthenticated">
                        <button class="nav-link logout-button" type="button" @click="logout">Wyloguj</button>
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
  data () {
    return {
      searchText: null
    }
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'isAdmin'])
  },
  methods: {
    logout () {
      this.$store.dispatch('logout')
    },
    goToSearch () {
      this.$router.push({name: 'SearchAdvertisement', params: {text: this.searchText}})
    }
  }
}
</script>

<style scoped>
    #mainNavbar {
        background-color: #26bba6;
        box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
        -webkit-transform: translateZ(0);
        padding-inline: 1rem;
    }
    #mainNavbar a, .navbar {
        color: white;
    }
    .navbar-brand {
        white-space: nowrap;
    }
    .navbar-collapse {
        column-gap: 1rem;
        min-width: 0;
    }
    .navbar-nav {
        gap: 0.25rem;
    }
    .navbar-nav.ms-auto {
        flex-wrap: wrap;
        justify-content: flex-end;
    }
    .logout-button {
        border: 0;
        background: transparent;
        color: white;
        cursor: pointer;
        font: inherit;
        text-align: left;
    }
    #searchInput {
        width: min(230px, 35vw);
    }
    @media (max-width: 991.98px) {
        .navbar-collapse {
            padding-top: 0.75rem;
        }
        .navbar-nav.ms-auto {
            justify-content: flex-start;
        }
        .search-form {
            margin-block: 0.5rem;
        }
    }
</style>
