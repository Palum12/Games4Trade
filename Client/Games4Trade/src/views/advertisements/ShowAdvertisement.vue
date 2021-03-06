<template>
<div v-if="hasDataLoaded" class="no-gutters advertisement">
    <div class="row">
        <div class="col-12 col-md-7">
            <div v-if="advertisement.photos.length > 1" class="gallery">
                <vue-flux
                        :options="fluxOptions"
                        :images="images"
                        :transitions="fluxTransitions"
                        ref="slider">
                    <flux-controls slot="controls"></flux-controls>
                    <flux-pagination slot="pagination"></flux-pagination>
                </vue-flux>
            </div>
            <div v-else-if="advertisement.photos.length === 1">
                <img :src="images[0]">
            </div>
            <div v-else>
                <img src="../../assets/no_image_available.svg"/>
            </div>
        </div>
        <div class="col-12 col-md-5">
            <p>Dodane przez: <router-link :to="`/users/${advertisement.user.id}`"
                                          exact
                                          tag="a">{{advertisement.user.login}}</router-link></p>
            <h5>Cena: {{advertisement.price}}zł</h5>
            <p v-if="advertisement.exchangeActive">Wymiana jest możliwa</p>
            <p v-if="advertisement.phoneNumber != null">Numer kontaktowy: {{advertisement.phoneNumber}}</p>
            <p v-if="advertisement.email != null">Adres email kontaktowy: {{advertisement.email}}</p>
            <p>Stan: {{advertisement.state.value}}</p>
            <p>System: {{advertisement.system.manufacturer + ' ' + advertisement.system.model}}</p>
            <div v-if="advertisement.discriminator==='Game'">
                <p>Kategoria ogłoszenia: Gra</p>
                <p v-if="advertisement.developer != null">Producent: {{advertisement.developer}}</p>
                <p>Gatunek: {{advertisement.genre.value}}</p>
            </div>
            <div v-else-if="advertisement.discriminator==='Console'">
                <p>Kategoria ogłoszenia: Konsola</p>
            </div>
            <div v-else-if="advertisement.discriminator==='Accessory'">
                <p>Kategoria ogłoszenia: Akcesorium</p>
                <p>Producent: {{advertisement.accessoryManufacturer}}</p>
                <p>Model: {{advertisement.accessoryModel}}</p>
            </div>
            <p v-if="advertisement.dateReleased != null">Data wydania: {{advertisement.dateReleased.substring(0, 10)}}</p>
            <div v-if="!isOwner && $store.getters.isAuthenticated">
                <button type="button" class="btn btn-primary btn-block" @click="sendMessage">Napisz do użytkownika!</button>
            </div>
        </div>
    </div>
    <div class="row m-1">
        <h2>{{advertisement.title}}</h2>
    </div>
    <div class="row mt-1 container-fluid" style="white-space: pre-line;">
        {{advertisement.description}}
    </div>
    <div class="row m-1 d-flex justify-content-between">
        <button class="btn btn-info" type="button" @click="$router.go(-1)">Powrót</button>
        <button v-if="isOwner"
                type="button"
                @click="$router.push(`/advertisements/${advertisement.id}/edit`)"
                class="btn btn-warning">Modyfikuj</button>
        <button
                v-if="!isOwner && $store.getters.isAdmin"
                class="btn btn-danger"
                type="button"
                @click="deleteAd">Usuń administracyjnie</button>
    </div>
</div>
</template>

<script>
import axios from 'axios'
import mixnis from '../../mixins/mixins'
import { VueFlux, FluxControls, FluxPagination, Transitions } from 'vue-flux'
export default {
  name: 'ShowAdvertisement',
  components: {
    VueFlux,
    FluxControls,
    FluxPagination
  },
  data () {
    return {
      fluxOptions: {
        autoplay: true,
        enableGestures: true
      },
      fluxTransitions: {
        transitionBook: Transitions.transitionSwipe
      },
      userId: null,
      hasDataLoaded: false,
      isOwner: false,
      advertisement: null
    }
  },
  computed: {
    images () {
      return this.advertisement.photos.map(x => `http://localhost:5000/api/advertisements/${this.advertisement.id}/photos/${x.id}`)
    }
  },
  methods: {
    sendMessage () {
      let vm = this
      this.$swal({
        title: 'Wpisz treść wiadomości',
        input: 'textarea',
        inputPlaceholder: 'Tutaj wpisz treść wiadomości...',
        inputAttributes: {
          autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Wyślij',
        showLoaderOnConfirm: true,
        preConfirm: (message) => {
          axios.post('Messages', {receiverId: vm.advertisement.userId, content: message})
            .then(() => {
              vm.$swal({
                title: 'Wiadmość została wysłana!',
                type: 'success'
              })
            })
            .catch(() => {
              mixnis.methods.errorPopUp(vm)
            })
        }
      })
    },
    deleteAd () {
      let vm = this
      this.$swal({
        type: 'warning',
        title: 'Czy jesteś tego pewien ?',
        input: 'textarea',
        inputPlaceholder: 'Tutaj wpisz treść wiadomości z przyczyną usunięcia ogłoszenia,' +
          ' która zostanie wysłana na email użytkownika.',
        inputAttributes: {
          autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Usuń!',
        showLoaderOnConfirm: true,
        preConfirm: (message) => {
          axios.delete(`advertisements/${vm.advertisement.id}?reason=${message}`)
            .then(() => {
              vm.$swal({
                title: 'Ogłosznie zostało usunięte.',
                text: 'Użtykownik został powiadomiony o usunięciu jego ogłoszenia',
                type: 'success'
              })
              vm.$router.go(-1)
            })
            .catch(() => {
              mixnis.methods.errorPopUp(vm)
            })
        }
      })
    }
  },
  async mounted () {
    let vm = this
    if (this.$store.getters.isAuthenticated) {
      await this.$store.dispatch('getUserId')
        .then(response => {
          vm.userId = response.data
        })
    }
    let id = this.$route.params.id
    await axios.get(`advertisements/${id}`)
      .then(response => {
        vm.advertisement = response.data
        vm.isOwner = vm.$store.getters.isAuthenticated && vm.userId === response.data.userId
        vm.hasDataLoaded = true
      })
      .catch(() => {
        vm.$router.push('/')
      })
  }
}
</script>

<style scoped>
    img {
        min-height: 200px;
        height: 45vh;
        max-height: 90%;
        width: 60vh;
        object-fit: contain ;
    }
    .gallery{
        min-height: 300px;
        height: 45vh;
        max-height: 90%;
        min-width: 400px;
        width: 60vh;
        max-width: 90%;
        overflow: hidden;
        overflow-y: auto;
    }
    .advertisement {
        margin: 0 2vw;
        padding-bottom: 2vh;
        width: 90vw;
        text-justify: newspaper;
    }
</style>
